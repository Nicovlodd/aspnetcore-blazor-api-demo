using System.Text.Json.Serialization;

namespace MedicalCare.Frontend.Models
{
    public class SpecialityDurationReport
    {
        [JsonPropertyName("specialityName")]
        public string SpecialityName { get; set; } = string.Empty;

        [JsonPropertyName("averageDurationMinutes")]
        public int AverageDurationMinutes { get; set; }
    }
}