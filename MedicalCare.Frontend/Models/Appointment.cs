using System.Text.Json.Serialization;

namespace MedicalCare.Frontend.Models
{
    public class Appointment
    {
        [JsonPropertyName("appointment_Id")]
        public int Appointment_Id { get; set; }

        [JsonPropertyName("patientId")]
        public int PatientId { get; set; }

        [JsonPropertyName("doctorId")]
        public int DoctorId { get; set; }
        
        [JsonPropertyName("appointment_StartUtc")]
        public DateTime Appointment_StartUtc { get; set; } = DateTime.Now; 
        
        [JsonPropertyName("appointment_EndUtc")]
        public DateTime Appointment_EndUtc { get; set; } = DateTime.Now.AddHours(1); 
        
        [JsonPropertyName("appointment_Diagnosis")]
        public string Appointment_Diagnosis { get; set; } = string.Empty;

        [JsonPropertyName("appointment_Room")]
        public string Appointment_Room { get; set; } = "Box 1"; 

        [JsonPropertyName("appointment_Status")]
        public string Appointment_Status { get; set; } = "Agendada";

        [JsonPropertyName("appointment_CreatedBy")]
        public string Appointment_CreatedBy { get; set; } = "Sistema";

        public string Patient_Name { get; set; } = string.Empty;
        public string Doctor_Name { get; set; } = string.Empty;
        public string Speciality_Name { get; set; } = string.Empty;
    }
}