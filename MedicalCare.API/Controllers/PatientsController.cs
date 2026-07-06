using Microsoft.AspNetCore.Mvc;
using MedicalCare.API.Models;
using MedicalCare.API.Services;
using MedicalCare.API.Attributes;

namespace MedicalCare.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiKey]
    public class PatientsController : ControllerBase
    {
        private readonly IPatientService _patientService;

        public PatientsController(IPatientService patientService)
        {
            _patientService = patientService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var patients = await _patientService.GetAllAsync();
            return Ok(patients);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var patient = await _patientService.GetByIdAsync(id);
            if (patient == null)
                return NotFound("Paciente no encontrado.");
            
            return Ok(patient);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Patient patient)
        {
            try
            {
                var newId = await _patientService.CreateAsync(patient);
                patient.Patient_Id = newId;
                return CreatedAtAction(nameof(GetById), new { id = newId }, patient);
            }
            catch (Microsoft.Data.SqlClient.SqlException ex) when (ex.Number == 2627)
            {
                return BadRequest("El RUT ingresado ya se encuentra registrado en el sistema.");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Patient patient)
        {
            if (id != patient.Patient_Id)
                return BadRequest("El ID de la URL no coincide");

            var updated = await _patientService.UpdateAsync(patient);
            if (!updated)
                return NotFound("No se pudo actualizar. Paciente no encontrado.");

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _patientService.DeleteAsync(id);
            if (!deleted)
                return NotFound("No se pudo eliminar. Paciente no encontrado.");

            return NoContent();
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string? rut, [FromQuery] string? name, [FromQuery] int? minAge, [FromQuery] int? maxAge)
        {
            var patients = await _patientService.SearchAsync(rut, name, minAge, maxAge);
            return Ok(patients);
        }
    }
}