using NAudio;
using NAudio.Wave;
using FFMpegCore.Pipes;
using FFMpegCore;
using System.Diagnostics;

namespace OscilloGraph.Test
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            using var stream = new WaveFileReader(args[0]);
            using var writer = new WaveFileWriter("new.wav", stream.WaveFormat);

            Process.Start(new ProcessStartInfo()
            {
                FileName = "ffplay",
                Arguments = $"-i \"{Path.GetFullPath("new.wav")}\" -nodisp",
                CreateNoWindow = true
            });

            while (stream.Position < stream.Length)
            {
                var frame = stream.ReadNextSampleFrame();
                writer.WriteSamples(frame, 0, frame.Length);
            }
        }
    }
}