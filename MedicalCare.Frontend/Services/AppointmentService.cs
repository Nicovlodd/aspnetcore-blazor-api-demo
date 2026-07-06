using System.Net.Http.Json;
using MedicalCare.Frontend.Models;

namespace MedicalCare.Frontend.Services
{
    public class AppointmentService
    {
        private readonly HttpClient _http;

        public AppointmentService(HttpClient http)
        {
            _http = http;
        }
        // Obtener todas las citas con detalles de paciente, doctor y especialidad

        public async Task<List<Appointment>> GetAppointmentsAsync()
        {
            return await _http.GetFromJsonAsync<List<Appointment>>("api/Appointments") ?? new List<Appointment>();
        }
        // Obtener una sola cita por ID

        public async Task<bool> CreateAppointmentAsync(Appointment appointment)
        {
            var response = await _http.PostAsJsonAsync("api/Appointments", appointment);
            return response.IsSuccessStatusCode;
        }

        // Actualizar una cita existente
        public async Task<bool> DeleteAppointmentAsync(int id)
        {
            var response = await _http.DeleteAsync($"api/Appointments/{id}");
            return response.IsSuccessStatusCode;
        }

        // Buscar citas dinámicamente por fecha, doctor, paciente o especialidad
        public async Task<List<Appointment>> SearchAppointmentsAsync(DateTime? date, string? doctor, string? patient, string? speciality)
        {
            string dateParam = date.HasValue ? date.Value.ToString("yyyy-MM-dd") : "";
            
            var query = $"api/Appointments/search?date={dateParam}&doctor={doctor}&patient={patient}&speciality={speciality}";
            return await _http.GetFromJsonAsync<List<Appointment>>(query) ?? new List<Appointment>();
        }
        // Obtener el reporte de duración promedio por especialidad

        public async Task<List<SpecialityDurationReport>> GetAverageDurationReportAsync()
        {
            return await _http.GetFromJsonAsync<List<SpecialityDurationReport>>("api/Appointments/average-duration") 
                   ?? new List<SpecialityDurationReport>();
        }
    }
}