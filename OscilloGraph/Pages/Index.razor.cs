using Blazor.Extensions.Canvas.Canvas2D;
using Blazor.Extensions;
using Microsoft.AspNetCore.Components;
using NAudio.Wave;

using OscilloGraph.Global;
using System;
using System.Diagnostics;

namespace OscilloGraph
{
    public class IndexBase : ComponentBase
    {
        private bool buttonHidden = false;
        protected bool ButtonHidden { get => AppInfo.AutoRender ? true : buttonHidden; set => buttonHidden = value; }

        private Canvas2DContext canvasContext = null!;
        protected BECanvasComponent? canvas;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                canvasContext = await canvas.CreateCanvas2DAsync();
                await canvasContext.SetFillStyleAsync($"rgb({AppInfo.Setting?.Canvas?.Color ?? "0, 0, 0"})");
                await canvasContext.FillRectAsync(0, 0, canvas?.Width ?? 320, canvas?.Height ?? 240);

                if (AppInfo.AutoRender)
                {
                    await RenderWaveFile();
                    // await canvasContext.SetFillStyleAsync($"rgb({AppInfo.Setting?.Pen?.Color ?? "0, 0, 0"})");
                    // await canvasContext.FillRectAsync(0, 0, 2, 2);
                }
            }
        }

        internal async Task OnButtonClick()
        {
            /*
            await canvasContext.SetStrokeStyleAsync("rgb(250,250,250)");
            await canvasContext.SetFontAsync("20px Microsoft YaHei");
            await canvasContext.StrokeTextAsync("Hello, OscilloGraph", 10, 50);
            */

            await RenderWaveFile();
        }
        
        private async Task RenderLine(Canvas2DContext canvas, string lineColor, int lineSize)
        {
            await canvas.SetFillStyleAsync($"rgb({lineColor})");
            
            for (int i = 1; i < 10; ++i)
            {
                // Bold Line
                var size = i == 5 ? 2 * lineSize : lineSize;
                
                // Fill Vertical Line
                await canvas.FillRectAsync((i * AppInfo.Width / 10) - (size / 2), 0, size, AppInfo.Height);
                // File Horizontal Line
                await canvas.FillRectAsync(0, (i * AppInfo.Height / 10) - (size / 2), AppInfo.Width, size);
            }
        }

        internal async Task RenderWaveFile()
        {
            try
            {
                // Disable the Start Button
                /* Known issue:
                 * When this method is called on OnAfterRenderAsync,
                 * the hidden property of a new button cannot be updated.
                 * See -> Index.razor
                 */
                ButtonHidden = true;

                // Cal
                var audioFile = Path.GetFullPath(AppInfo.File);

                // Play Audio File in a Thread
                /* In the earliest version
                 * I create a copy of wave file, then play it.
                 * But it may caused some problems, like sound and rendering are out of sync
                 * 
                 * 2023/1/2
                 * I solved this problem :)
                 
                ****************
                Old Code ->
                ****************
                
                var audioFileClone = Path.ChangeExtension(audioFile, ".clone.wav");

                if (File.Exists(audioFileClone))
                    File.Delete(audioFileClone);
                File.Copy(audioFile, audioFileClone);
                
                using var waveOutReader = new WaveFileReader(audioFile);
                var waveOut = new WaveOutEvent();
                waveOut.Init(waveOutReader);
                waveOut.Play();
                
                ****************
                New Code -> See [Sync Solve] Tag
                ****************
                */
                
                
                // Render Audio File in Another Thread
                using var renderReader = new WaveFileReader(audioFile);
                
                // [Sync Solve]
                // New Scenarios
                // To Solve Sync Problem
                var waveProvider = new BufferedWaveProvider(renderReader.WaveFormat);
                using var waveOut = new WaveOutEvent();
                waveOut.Init(waveProvider);
                waveOut.Play();
                
                // Render
                var penSize = AppInfo.Setting?.Pen?.Size ?? 3;
                var readLength = renderReader.WaveFormat.SampleRate / AppInfo.Fps;
                var frameBytes = new byte[4];
                (decimal, decimal) frame;
                
                var final = DateTime.Now;
                while (renderReader.Position < renderReader.Length)
                {
                    // Adjust FPS precisely
                    if (DateTime.Now > final)
                    {
                        // Settings
                        await canvasContext.BeginBatchAsync();
                        await canvasContext.SetFillStyleAsync($"rgb({AppInfo.Setting?.Canvas?.Color ?? "0, 0, 0"})");
                        await canvasContext.FillRectAsync(0, 0, canvas?.Width ?? 320, canvas?.Height ?? 240);
                        await RenderLine(canvasContext, AppInfo.Setting?.Canvas?.Line ?? "50, 40, 0", AppInfo.Setting?.Canvas?.LineSize ?? 2);
                        await canvasContext.SetFillStyleAsync($"rgb({AppInfo.Setting?.Pen?.Color ?? "0, 250, 0"})");
                        
                        for (var i = 0; i < readLength; ++i)
                        {
                            // renderReader.ReadNextSampleFrame use 2e17
                            // But I should use 2e16
                            // So bits per sample (AppInfo.File) must be 16, and channels must be 2
                            await renderReader.ReadAsync(frameBytes, 0, frameBytes.Length);
                            frame.Item1 = BitConverter.ToInt16(frameBytes[0..2]) / 65536m;
                            frame.Item2 = BitConverter.ToInt16(frameBytes[2..]) / 65536m;
                            
                            var x = (int)(frame.Item2 * AppInfo.Width + AppInfo.Height / 2m);
                            var y = (int)(-frame.Item1 * AppInfo.Height) + (AppInfo.Height / 2 + (AppInfo.Width - AppInfo.Height) / 2);
                            
                            await canvasContext.FillRectAsync(y + penSize / 2, x + penSize / 2, penSize, penSize);
                            
                            // [Sync Solve]
                            // Play Sound
                            waveProvider.AddSamples(frameBytes, 0, frameBytes.Length);
                        }

                        await canvasContext.EndBatchAsync();
                        final += TimeSpan.FromMilliseconds(Math.Round(1000d / AppInfo.Fps));
                    }
                    else await Task.Delay(final - DateTime.Now);
                }

                ButtonHidden = false;
            }
            catch
            {
                ButtonHidden = false;
                throw;
            }
        }
    }
}