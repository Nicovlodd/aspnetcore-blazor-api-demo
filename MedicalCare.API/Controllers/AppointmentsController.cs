using Microsoft.AspNetCore.Mvc;
using MedicalCare.API.Models;
using MedicalCare.API.Services;
using MedicalCare.API.Attributes;

namespace MedicalCare.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
     [ApiKey]
    public class AppointmentsController : ControllerBase
    {
        private readonly IAppointmentService _appointmentService;

        public AppointmentsController(IAppointmentService appointmentService)
        {
            _appointmentService = appointmentService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _appointmentService.GetAllAsync());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var appointment = await _appointmentService.GetByIdAsync(id);
            if (appointment == null) return NotFound("Cita médica no encontrada.");
            return Ok(appointment);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Appointment appointment)
        {
            var newId = await _appointmentService.CreateAsync(appointment);
            appointment.Appointment_Id = newId;
            return CreatedAtAction(nameof(GetById), new { id = newId }, appointment);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Appointment appointment)
        {
            if (id != appointment.Appointment_Id) return BadRequest("El ID de la URL no coincide.");
            
            var updated = await _appointmentService.UpdateAsync(appointment);
            if (!updated) return NotFound("No se pudo actualizar.");
            
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _appointmentService.DeleteAsync(id);
            if (!deleted) return NotFound("No se pudo eliminar.");
            
            return NoContent();
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search(
            [FromQuery] DateTime? date, 
            [FromQuery] string? doctor, 
            [FromQuery] string? patient, 
            [FromQuery] string? speciality)
        {
            var results = await _appointmentService.SearchAsync(date, doctor, patient, speciality);
            return Ok(results);
        }

        [HttpGet("average-duration")]
        public async Task<IActionResult> GetAverageDuration()
        {
            var report = await _appointmentService.GetAverageDurationBySpecialityAsync();
            return Ok(report);
        }
    }
}