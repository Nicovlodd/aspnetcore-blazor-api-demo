using MedicalCare.API.Models;

namespace MedicalCare.API.Services
{
    public interface IAppointmentService
    {
        Task<IEnumerable<Appointment>> GetAllAsync();
        Task<Appointment?> GetByIdAsync(int id);
        Task<int> CreateAsync(Appointment appointment);
        Task<bool> UpdateAsync(Appointment appointment);
        Task<bool> DeleteAsync(int id);
        Task<IEnumerable<Appointment>> SearchAsync(DateTime? date, string? doctorName, string? patientName, string? speciality);
        Task<IEnumerable<SpecialityDurationReport>> GetAverageDurationBySpecialityAsync();
    }
}