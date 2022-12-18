using System.IO;
using NAudio.Wave;

namespace HörbuchgreatorWPF.tools
{
    public class ByteToWav
    {
        public static void SaveData(string path, byte[] data)
        {
            var waveFileWriter = new WaveFileWriter(path, new WaveFormat(44100, 16, 2));
            // Schreiben Sie das byte-Array mit den Audiodaten in die mp3-Datei
            waveFileWriter.Write(data, 0, data.Length);
        }

    }
}
