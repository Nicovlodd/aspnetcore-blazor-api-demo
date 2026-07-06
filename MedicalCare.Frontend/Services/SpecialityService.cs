using System.Net.Http.Json;
using MedicalCare.Frontend.Models;

namespace MedicalCare.Frontend.Services
{
    public class SpecialityService
    {
        private readonly HttpClient _http;

        public SpecialityService(HttpClient http)
        {
            _http = http;
        }
        // Obtener todas las especialidades

        public async Task<List<Speciality>> GetSpecialitiesAsync()
        {
            return await _http.GetFromJsonAsync<List<Speciality>>("api/Specialities") ?? new List<Speciality>();
        }
        // Obtener una sola especialidad por ID

        public async Task<Speciality?> GetSpecialityByIdAsync(int id)
        {
            return await _http.GetFromJsonAsync<Speciality>($"api/Specialities/{id}");
        }
        // Enviar una nueva especialidad a la API

        public async Task<bool> CreateSpecialityAsync(Speciality speciality)
        {
            var response = await _http.PostAsJsonAsync("api/Specialities", speciality);
            return response.IsSuccessStatusCode;
        }
        // Actualizar una especialidad existente
        public async Task<bool> UpdateSpecialityAsync(int id, Speciality speciality)
        {
            var response = await _http.PutAsJsonAsync($"api/Specialities/{id}", speciality);
            return response.IsSuccessStatusCode;
        }
        // Eliminar una especialidad
        public async Task<bool> DeleteSpecialityAsync(int id)
        {
            var response = await _http.DeleteAsync($"api/Specialities/{id}");
            return response.IsSuccessStatusCode;
        }
    }
}