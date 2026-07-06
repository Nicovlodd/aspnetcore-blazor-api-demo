using Microsoft.AspNetCore.Mvc;
using MedicalCare.API.Models;
using MedicalCare.API.Services;
using MedicalCare.API.Attributes;

namespace MedicalCare.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
     [ApiKey]
    public class DoctorsController : ControllerBase
    {
        private readonly IDoctorService _doctorService;

        public DoctorsController(IDoctorService doctorService)
        {
            _doctorService = doctorService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var doctors = await _doctorService.GetAllAsync();
            return Ok(doctors);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var doctor = await _doctorService.GetByIdAsync(id);
            if (doctor == null)
                return NotFound("Doctor no encontrado.");
            
            return Ok(doctor);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Doctor doctor)
        {
            var newId = await _doctorService.CreateAsync(doctor);
            doctor.Doctor_Id = newId;
            
            return CreatedAtAction(nameof(GetById), new { id = newId }, doctor);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Doctor doctor)
        {
            if (id != doctor.Doctor_Id)
                return BadRequest("El ID de la URL no coincide.");

            var updated = await _doctorService.UpdateAsync(doctor);
            if (!updated)
                return NotFound("No se pudo actualizar. Doctor no encontrado.");

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _doctorService.DeleteAsync(id);
            if (!deleted)
                return NotFound("No se pudo eliminar. Doctor no encontrado.");

            return NoContent();
        }
    }
}