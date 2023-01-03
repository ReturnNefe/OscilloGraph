using System.Runtime.Serialization;
using System.Text;
using Cocona;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using OscilloGraph.Data;
using OscilloGraph.Global;
using OscilloGraph.Global.Helper;
using System.Diagnostics;
using System.Text.Json;

namespace OscilloGraph
{
    internal class Program
    {
        static async Task Init()
        {
            using (StreamReader reader = new(Path.Combine(AppInfo.Path, "config.txt"), Encoding.UTF8))
                AppInfo.Setting = JsonSerializer.Deserialize<Setting>(await reader.ReadToEndAsync(), AppInfo.JsonOptions) ?? new();
        }
        
        static async Task Main(string[] args)
        {
            void NoDevelopmentAction(IWebHostEnvironment environment, Action action)
            {
                if (!environment.IsDevelopment())
                    action();
            };


            // Parse Arguments
            var cocona = CoconaLiteApp.Create();
            cocona.AddCommand(async ([Argument("file", Description = "the Wave File to play")] string file,
                                     [Option("fps", Description = "The default is equal to 25")] int? fps,
                                     [Option("url", Description = "The URL that the HTTP server should use")] string? url,
                                     [Option("no-auto-open", Description = "Whether to open a webpage after the program starts (default is enalbed)"),] bool? noAutoOpen,
                                     [Option("auto-render", Description = "Whether to render after the webpage has loaded (default is disabled)")] bool? autoRender) =>
            {
                AppInfo.File = file;
                AppInfo.Fps = fps ?? 25;
                AppInfo.Url = url ?? $"http://localhost:{TcpHelper.GetAvailablePort()}";
                AppInfo.AutoOpen = !(noAutoOpen ?? false);
                AppInfo.AutoRender = autoRender ?? false;
                
                
                // Init
                await Init();
                
                // Blazor Default
                // Intercept Command Arguments
                var builder = WebApplication.CreateBuilder(Array.Empty<string>());
                
                // Add services to the container.
                builder.Services.AddRazorPages();
                builder.Services.AddServerSideBlazor();
                builder.Services.AddSingleton<WeatherForecastService>();
                
                /* Safe Options
                 * in the public environment
                builder.Services.AddServerSideBlazor(options =>
                {
                    options.DetailedErrors = false;
                    options.DisconnectedCircuitMaxRetained = 100;
                    options.DisconnectedCircuitRetentionPeriod = TimeSpan.FromMinutes(3);
                    options.JSInteropDefaultCallTimeout = TimeSpan.FromMinutes(1);
                    options.MaxBufferedUnacknowledgedRenderBatches = 10;
                });
                */

                var app = builder.Build();

                // Configure the HTTP request pipeline.
                NoDevelopmentAction(app.Environment, () =>
                {
                    app.UseExceptionHandler("/Error");

                    // Use Customized Url
                    app.Urls.Add(AppInfo.Url);
                });

                // Console.WriteLine(app.Urls.ToList()[0]);
                app.UseStaticFiles();

                app.UseRouting();

                app.MapBlazorHub();
                app.MapFallbackToPage("/_Host");

                var task = app.RunAsync();

                NoDevelopmentAction(app.Environment, () =>
                {
                    if (AppInfo.AutoOpen)
                        Process.Start(new ProcessStartInfo()
                        {
                            // Replace * -> localhost
                            FileName = AppInfo.Url.Replace("*", "localhost"),
                            UseShellExecute = true
                        });
                });
                
                await task;
            });

            await cocona.RunAsync();
        }
    }
}