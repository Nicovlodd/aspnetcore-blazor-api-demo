using System.Net.Http.Json;
using MedicalCare.Frontend.Models;

namespace MedicalCare.Frontend.Services
{
    public class PatientService
    {
        private readonly HttpClient _http;

        public PatientService(HttpClient http)
        {
            _http = http;
        }

        // Obtener todos los pacientes
        public async Task<List<Patient>> GetPatientsAsync()
        {
            return await _http.GetFromJsonAsync<List<Patient>>("api/Patients") ?? new List<Patient>();
        }

        // Obtener un solo paciente por ID
        public async Task<Patient?> GetPatientByIdAsync(int id)
        {
            return await _http.GetFromJsonAsync<Patient>($"api/Patients/{id}");
        }

        // Enviar un nuevo paciente a la API
        public async Task<bool> CreatePatientAsync(Patient patient)
        {
            var response = await _http.PostAsJsonAsync("api/Patients", patient);
            return response.IsSuccessStatusCode;
        }

        // Actualizar un paciente existente
        public async Task<bool> UpdatePatientAsync(int id, Patient patient)
        {
            var response = await _http.PutAsJsonAsync($"api/Patients/{id}", patient);
            return response.IsSuccessStatusCode;
        }

        // Eliminar un paciente
        public async Task<bool> DeletePatientAsync(int id)
        {
            var response = await _http.DeleteAsync($"api/Patients/{id}");
            return response.IsSuccessStatusCode;
        }

        // Buscar pacientes dinámicamente por nombre, RUT o edad
        public async Task<List<Patient>> SearchPatientsAsync(string? name, string? rut)
        {
            var query = $"api/Patients/search?name={name}&rut={rut}";
            return await _http.GetFromJsonAsync<List<Patient>>(query) ?? new List<Patient>();
        }
    }
}