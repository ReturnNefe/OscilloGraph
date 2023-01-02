using NAudio;
using NAudio.Wave;

namespace OscilloGraph.Test
{
    internal class Program
    {
        static void Main(string[] args)
        {
            using var stream = new WaveFileReader(args[0]);
            using var writer = new WaveFileWriter("../../../../new.wav", stream.WaveFormat);

            while (stream.Position < stream.Length)
            {
                var frame = stream.ReadNextSampleFrame();
                writer.WriteSamples(frame, 0, frame.Length);
            }
        }
    }
}