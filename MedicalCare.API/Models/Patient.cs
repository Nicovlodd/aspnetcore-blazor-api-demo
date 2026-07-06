namespace MedicalCare.API.Models
{
    public class Patient
    {
        public int Patient_Id { get; set; }
        public string Patient_RUT { get; set; } = string.Empty;
        public string Patient_FirstName { get; set; } = string.Empty;
        public string Patient_LastName { get; set; } = string.Empty;
        public DateTime Patient_DateOfBirth { get; set; }
    }
}