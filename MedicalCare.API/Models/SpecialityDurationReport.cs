namespace MedicalCare.API.Models
{
    public class SpecialityDurationReport
    {
        public string SpecialityName { get; set; } = string.Empty;
        public int AverageDurationMinutes { get; set; }
    }
}