using MedicalCare.API.Controllers;
using MedicalCare.API.Models;
using MedicalCare.API.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace MedicalCare.Tests
{
    public class AppointmentsControllerTests
    {
        [Fact]
        public async Task GetAverageDuration_ReturnsOkResult_WithData()
        {
            // 1. ARRANGE (Preparar el escenario)
            var mockService = new Mock<IAppointmentService>();
            
            // Creamos un dato falso de prueba
            var fakeReport = new List<SpecialityDurationReport>
            {
                new SpecialityDurationReport { SpecialityName = "Cardiología", AverageDurationMinutes = 45 }
            };

            // Le decimos al mock qué responder cuando llamen a su método
            mockService.Setup(service => service.GetAverageDurationBySpecialityAsync())
                       .ReturnsAsync(fakeReport);

            // Inyectamos el servicio falso en el controlador
            var controller = new AppointmentsController(mockService.Object);

            // 2. ACT (Ejecutar la acción)
            var result = await controller.GetAverageDuration();

            // 3. ASSERT (Verificar el resultado)
            // Validamos que el código HTTP sea 200 OK
            var okResult = Assert.IsType<OkObjectResult>(result);
            
            // Validamos que los datos que trae adentro sean una lista del reporte
            var returnedData = Assert.IsAssignableFrom<IEnumerable<SpecialityDurationReport>>(okResult.Value);
            
            // Validamos que traiga exactamente 1 registro (el de Cardiología que inventamos)
            Assert.Single(returnedData);
        }
    }
}