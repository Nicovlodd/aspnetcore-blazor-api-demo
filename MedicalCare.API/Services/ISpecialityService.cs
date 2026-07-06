using MedicalCare.API.Models;

namespace MedicalCare.API.Services
{
    public interface ISpecialityService
    {
        Task<IEnumerable<Speciality>> GetAllAsync();
        Task<Speciality?> GetByIdAsync(int id);
        Task<int> CreateAsync(Speciality speciality);
        Task<bool> UpdateAsync(Speciality speciality);
        Task<bool> DeleteAsync(int id);
    }
}