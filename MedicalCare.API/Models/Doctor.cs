namespace MedicalCare.API.Models
{
public class Doctor
    {
        public int Doctor_Id { get; set; }
        public string Doctor_RUT { get; set; } = string.Empty;
        public string Doctor_FirstName { get; set; } = string.Empty;
        public string Doctor_LastName { get; set; } = string.Empty;
        public int Speciality_Id { get; set; }
        public string Speciality_Name { get; set; } = string.Empty; 
    }
}