using System.Text.Json.Serialization;

namespace license
{
    public class Validation
    {
        public bool Valid { get; set; }
        public string Detail { get; set; }
        [JsonPropertyName("constant")]
        public string Code { get; set; }
    }
}
