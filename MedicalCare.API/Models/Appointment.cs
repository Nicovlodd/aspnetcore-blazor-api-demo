namespace MedicalCare.API.Models
{
    public class Appointment
    {
        public int Appointment_Id { get; set; }
        public int PatientId { get; set; }
        public int DoctorId { get; set; }
        public DateTime Appointment_StartUtc { get; set; }
        public DateTime Appointment_EndUtc { get; set; }
        public string? Appointment_Diagnosis { get; set; }
        public string? Appointment_Room { get; set; }
        public string? Appointment_Status { get; set; }

        public string Appointment_CreatedBy { get; set; } = string.Empty;
        public DateTime Appointment_CreatedAt { get; set; }
        public string? Appointment_ModifiedBy { get; set; }
        public DateTime? Appointment_ModifiedAt { get; set; }
        public string Patient_Name { get; set; } = string.Empty;
        public string Doctor_Name { get; set; } = string.Empty;
        public string Speciality_Name { get; set; } = string.Empty;
    }
}