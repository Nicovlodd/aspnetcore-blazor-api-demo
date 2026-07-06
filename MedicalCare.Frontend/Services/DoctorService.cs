using System.Net.Http.Json;
using MedicalCare.Frontend.Models;

namespace MedicalCare.Frontend.Services
{
    public class DoctorService
    {
        private readonly HttpClient _http;

        public DoctorService(HttpClient http)
        {
            _http = http;
        }

        // Obtener todos los doctores

        public async Task<List<Doctor>> GetDoctorsAsync()
        {
            return await _http.GetFromJsonAsync<List<Doctor>>("api/Doctors") ?? new List<Doctor>();
        }
        // Obtener un solo doctor por ID

        public async Task<Doctor?> GetDoctorByIdAsync(int id)
        {
            return await _http.GetFromJsonAsync<Doctor>($"api/Doctors/{id}");
        }
        // Enviar un nuevo doctor a la API

        public async Task<bool> CreateDoctorAsync(Doctor doctor)
        {
            var response = await _http.PostAsJsonAsync("api/Doctors", doctor);
            return response.IsSuccessStatusCode;
        }
        // Actualizar un doctor existente

        public async Task<bool> UpdateDoctorAsync(int id, Doctor doctor)
        {
            var response = await _http.PutAsJsonAsync($"api/Doctors/{id}", doctor);
            return response.IsSuccessStatusCode;
        }
        // Eliminar un doctor

        public async Task<bool> DeleteDoctorAsync(int id)
        {
            var response = await _http.DeleteAsync($"api/Doctors/{id}");
            return response.IsSuccessStatusCode;
        }
    }
}