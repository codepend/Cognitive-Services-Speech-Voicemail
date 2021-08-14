using Microsoft.CognitiveServices.Speech.Audio;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoicemailConsoleAppTester
{
    class AudioInputCallback : PullAudioInputStreamCallback
    {
        private readonly Stream memoryStream;

        public AudioInputCallback(Stream stream)
        {
            this.memoryStream = stream;
        }

        public override int Read(byte[] dataBuffer, uint size)
        {
            return this.Read(dataBuffer, 0, dataBuffer.Length);
        }

        private int Read(byte[] buffer, int offset, int count)
        {
            return memoryStream.Read(buffer, offset, count);
        }

        public override void Close()
        {
            memoryStream.Close();
            base.Close();
        }
    }
}
