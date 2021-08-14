using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using System;
using System.IO;
using System.Threading.Tasks;

namespace VoicemailConsoleAppTester
{
    class Program
    {

        static string CognitiveKey = "XXXXXXXXXXXXXXXXXXXXXXX";
        static string CognitiveRegion = "centralus";

        static SpeechConfig config = SpeechConfig.FromSubscription(CognitiveKey, CognitiveRegion);

      

        static async Task Main()
        {
            string filePath = @"C:\Temp\good.wav";
            //string filePath = @"C:\Temp\bad.wav";
            //string filePath = @"C:\Temp\vm.wav";

            
            // Try this and compare against UsingVMProcessor
            await ResampleAndDetect(filePath);

            //await UsingVMProcessor(filePath);
            
            
            Console.WriteLine("Please press <Return> to continue.");
            Console.ReadLine();
        }

        /// <summary>
        /// Resample the incoming wav file to standardize the data being passed to Cognitive Services
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        async static Task ResampleAndDetect(string filePath)
        {
            using var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            /*
             * didn't need to read until end, but helper class still here if you need it
            var fileBytes = Helpers.ReadSreamToEnd(fileStream);
            using var copyStream = new MemoryStream(fileBytes);
            using var stream = await ResampleWav.ResampleWavStream(copyStream);
            */
            using var stream = await ResampleWav.ResampleWavStream(fileStream);
            var format = AudioStreamFormat.GetWaveFormatPCM(8000, 16, 1);
            var callback = new AudioInputCallback(stream);
            using var audioConfig = AudioConfig.FromStreamInput(callback, format);
            using var reckognizer = new SpeechRecognizer(config, audioConfig);

            var result = await reckognizer.RecognizeOnceAsync();
            Console.WriteLine(result.Text);

        }
        /// <summary>
        /// Uses the VMProcessor class that I use from my project
        /// </summary>
        /// <param name="filePath">Path to wav file on hard drive</param>
        /// <returns></returns>
        async static Task UsingVMProcessor(string filePath)
        {
            using var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            /*
             * didn't need to read until end, but helper class still here if you need it
            var fileBytes = Helpers.ReadSreamToEnd(fileStream);
            using var copyStream = new MemoryStream(fileBytes);
            using var stream = await ResampleWav.ResampleWavStream(copyStream);
            */
            using var stream = await ResampleWav.ResampleWavStream(fileStream);
            var processor = new VoicemailProcessor(config);
            var translation = await processor.ProcessVM(stream);
            Console.WriteLine(translation);

        }

        /// <summary>
        /// Unused - Just here for example
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static async Task RecognizeSpeechAsyncGitHubExample(string filePath)
        {
            using (var audioConfig = AudioConfig.FromWavFileInput(filePath))
            using (var recognizer = new SpeechRecognizer(config, audioConfig))
            {
                var result = await recognizer.RecognizeOnceAsync();

                if (result.Reason == ResultReason.RecognizedSpeech)
                {
                    Console.WriteLine($"We recognized: {result.Text}");
                }
                else if (result.Reason == ResultReason.NoMatch)
                {
                    Console.WriteLine($"NOMATCH: Speech could not be recognized.");
                }
                else if (result.Reason == ResultReason.Canceled)
                {
                    var cancellation = CancellationDetails.FromResult(result);
                    Console.WriteLine($"CANCELED: Reason={cancellation.Reason}");

                    if (cancellation.Reason == CancellationReason.Error)
                    {
                        Console.WriteLine($"CANCELED: ErrorCode={cancellation.ErrorCode}");
                        Console.WriteLine($"CANCELED: ErrorDetails={cancellation.ErrorDetails}");
                        Console.WriteLine($"CANCELED: Did you update the subscription info?");
                    }
                }
            }
        }

    }
}
