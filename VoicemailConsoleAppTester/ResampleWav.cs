using NAudio.Wave;
using NAudio.Codecs;
using System.IO;
using System.Threading.Tasks;

namespace VoicemailConsoleAppTester
{
    public class ResampleWav
    {
        public static async Task<Stream> ResampleWavStream(Stream input)
        {
            var outStream = new MemoryStream();
            //Tried using a higher bit rate like 16000, but the WaveFormatConverstionStream crashed. 8000 works.
            var wavFormat = new WaveFormat(8000, 16, 1);
            await using (var reader = new WaveFileReader(input))
            await using (var wavStream = new WaveFormatConversionStream(wavFormat, reader))
            await using (var converter = WaveFormatConversionStream.CreatePcmStream(wavStream))
            {
                WaveFileWriter.WriteWavFileToStream(outStream, converter);
                return outStream;
            }
        }
    }
}
