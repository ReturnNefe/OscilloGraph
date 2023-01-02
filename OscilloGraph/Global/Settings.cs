using System.Text.Json.Serialization;

namespace OscilloGraph.Global
{
    public class Setting
    {
        public class CanvasSetting
        {
            [JsonPropertyName("line")]
            public string? Line { get; set; }
            
            [JsonPropertyName("lineSize")]
            public int? LineSize { get; set; }
            
            [JsonPropertyName("color")]
            public string? Color { get; set; }
        }
        
        public class PenSetting
        {
            [JsonPropertyName("size")]
            public int? Size { get; set; }
            
            [JsonPropertyName("color")]
            public string? Color { get; set; }
        }
        
        [JsonPropertyName("canvas")]
        public CanvasSetting? Canvas { get; set; }
        
        [JsonPropertyName("pen")]
        public PenSetting? Pen { get; set; }
    }
}