using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MedicalCare.Frontend;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => 
{
    var client = new HttpClient { BaseAddress = new Uri("http://localhost:5175/Swagger") };
    client.DefaultRequestHeaders.Add("X-API-KEY", "ClinicaMedicalCareKey123123");
    return client;
});

builder.Services.AddScoped<MedicalCare.Frontend.Services.PatientService>();
builder.Services.AddScoped<MedicalCare.Frontend.Services.SpecialityService>();
builder.Services.AddScoped<MedicalCare.Frontend.Services.DoctorService>();
builder.Services.AddScoped<MedicalCare.Frontend.Services.AppointmentService>();

await builder.Build().RunAsync();
