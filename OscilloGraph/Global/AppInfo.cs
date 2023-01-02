using System.Text.Json;

namespace OscilloGraph.Global
{
    internal static class AppInfo
    {
        internal static string Path = AppDomain.CurrentDomain.BaseDirectory;
        internal static Setting? Setting { get; set; }
        internal static JsonSerializerOptions JsonOptions = new JsonSerializerOptions
        {
            ReadCommentHandling = JsonCommentHandling.Skip,
            AllowTrailingCommas = true,
        };

        internal static string File { get; set; } = "";
        internal static string Url { get; set; } = "";
        internal static int Fps { get; set; }
        internal static int Width { get; set; } = 640;
        internal static int Height { get; set; } = 480;

        internal static bool AutoOpen { get; set; }
        internal static bool AutoRender { get; set; }
    }
}
