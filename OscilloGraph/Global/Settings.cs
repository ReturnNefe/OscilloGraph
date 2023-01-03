using System.Text.Json.Serialization;

namespace OscilloGraph.Global
{
    public class Setting
    {
        public class CanvasSetting
        {
            [JsonPropertyName("line")]
            public string Line { get; set; } = "40, 35, 20";

            [JsonPropertyName("lineSize")]
            public int LineSize { get; set; } = 3;

            [JsonPropertyName("color")]
            public string Color { get; set; } = "0, 0, 0";
        }

        public class PenSetting
        {
            [JsonPropertyName("size")]
            public int Size { get; set; } = 3;

            [JsonPropertyName("color")]
            public string Color { get; set; } = "30, 255, 30";
        }

        public class AudioSetting
        {
            public class FFMpegSetting
            {
                [JsonPropertyName("path")]
                public string File { get; set; } = "ffplay";

                [JsonPropertyName("arguments")]
                public string Arguments { get; set; } = "-i \"${audioFile}\" -nodisp -loglevel quiet";
            }

            [JsonPropertyName("enabled")]
            public bool Enabled { get; set; } = true;

            [JsonPropertyName("player")]
            public string Player { get; set; } = "auto";

            [JsonPropertyName("ffmpeg")]
            public FFMpegSetting FFMpeg { get; set; } = new();
        }

        [JsonPropertyName("canvas")]
        public CanvasSetting Canvas { get; set; } = new();

        [JsonPropertyName("pen")]
        public PenSetting Pen { get; set; } = new();

        [JsonPropertyName("audio")]
        public AudioSetting Audio { get; set; } = new();
    }
}