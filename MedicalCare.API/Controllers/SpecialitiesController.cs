using Microsoft.AspNetCore.Mvc;
using MedicalCare.API.Models;
using MedicalCare.API.Services;
using MedicalCare.API.Attributes;

namespace MedicalCare.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiKey]
    public class SpecialitiesController : ControllerBase
    {
        private readonly ISpecialityService _specialityService;


        public SpecialitiesController(ISpecialityService specialityService)
        {
            _specialityService = specialityService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var specialities = await _specialityService.GetAllAsync();
            return Ok(specialities);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var speciality = await _specialityService.GetByIdAsync(id);
            if (speciality == null)
                return NotFound("Especialidad no encontrada.");
            
            return Ok(speciality);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Speciality speciality)
        {
            var newId = await _specialityService.CreateAsync(speciality);
            speciality.Speciality_Id = newId;
            
            // Devuelve un código 201 (Created) y la ruta para consultar el nuevo registro
            return CreatedAtAction(nameof(GetById), new { id = newId }, speciality);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Speciality speciality)
        {
            if (id != speciality.Speciality_Id) 
                return BadRequest("El ID de la URL no coincide.");

            var updated = await _specialityService.UpdateAsync(speciality);
            if (!updated)
                return NotFound("No se pudo actualizar. Especialidad no encontrada.");

            return NoContent(); // Código 204: Sin contenido
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _specialityService.DeleteAsync(id);
            if (!deleted)
                return NotFound("No se pudo eliminar. Especialidad no encontrada.");

            return NoContent();
        }
    }
}