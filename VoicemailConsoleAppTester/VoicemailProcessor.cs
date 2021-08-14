using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace VoicemailConsoleAppTester
{
    public class VoicemailProcessor
    {
        private readonly SpeechConfig config;

        public VoicemailProcessor(SpeechConfig speechConfig)
        {
            this.config = speechConfig;
        }

        public async Task<string> ProcessVM(Stream stream)
        {
            if (stream is null)
            {
                return "";
            }

            // Implementation of PullAudioInputStreamCallback
            var callback = new AudioInputCallback(stream);
            AudioStreamFormat format = AudioStreamFormat.GetWaveFormatPCM(8000, 16, 1);
            AudioConfig audioConfig = AudioConfig.FromStreamInput(callback, format);

            var output = new StringBuilder();

            using (var recognizer = new SpeechRecognizer(config, audioConfig))
            {

                var stopRecognition = new TaskCompletionSource<int>();

                recognizer.Recognizing += (s, e) =>
                {
                    // Console.WriteLine($"RECOGNIZING: Text={e.Result.Text}");
                };

                recognizer.Recognized += (s, e) =>
                {
                    if (e.Result.Reason == ResultReason.RecognizedSpeech)
                    {
                        //Console.WriteLine($"RECOGNIZED: Text={e.Result.Text}");
                        output.Append(e.Result.Text);
                    }
                    else if (e.Result.Reason == ResultReason.NoMatch)
                    {
                        // should be a logger
                        //Console.WriteLine($"NOMATCH: Speech could not be recognized.");
                    }
                };

                recognizer.Canceled += (s, e) =>
                {
                    Console.WriteLine($"CANCELED: Reason={e.Reason}");

                    if (e.Reason == CancellationReason.Error)
                    {
                        Console.WriteLine($"CANCELED: ErrorCode={e.ErrorCode}");
                        Console.WriteLine($"CANCELED: ErrorDetails={e.ErrorDetails}");
                        Console.WriteLine($"CANCELED: Did you update the subscription info?");
                    }

                    stopRecognition.TrySetResult(0);
                };

                recognizer.SessionStopped += (s, e) =>
                {
                    Console.WriteLine("\n    Session stopped event.");
                    stopRecognition.TrySetResult(0);
                };


                // Starts continuous recognition. Uses StopContinuousRecognitionAsync() to stop recognition.
                await recognizer.StartContinuousRecognitionAsync();

                // Waits for completion. Use Task.WaitAny to keep the task rooted.
                Task.WaitAny(new[] { stopRecognition.Task });

                // Stops recognition.
                await recognizer.StopContinuousRecognitionAsync();

                return output.ToString();
            }

        }
    }
}
