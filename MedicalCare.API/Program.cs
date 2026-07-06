using Microsoft.OpenApi.Models; 
var builder = WebApplication.CreateBuilder(args);

// Configurar las reglas de CORS (Para dejar pasar a Blazor)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazor", policy =>
    {
        policy.AllowAnyOrigin()  
              .AllowAnyHeader()   
              .AllowAnyMethod();  
    });
});


builder.Services.AddControllers();

// 2. Configuración de swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "MedicalCare API", Version = "v1" });
    
    // Configurar Swagger para usar la cabecera X-API-KEY
    c.AddSecurityDefinition("ApiKey", new OpenApiSecurityScheme
    {
        Description = "Autenticación basada en API Key. Ingresa tu clave en el campo de texto.",
        In = ParameterLocation.Header,
        Name = "X-API-KEY",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "ApiKeyScheme"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "ApiKey"
                },
                In = ParameterLocation.Header
            },
            new List<string>()
        }
    });
});

// 3. Registro de dependencias (Inyección de componentes)
builder.Services.AddSingleton<MedicalCare.API.Data.DbConnectionFactory>();
builder.Services.AddScoped<MedicalCare.API.Services.ISpecialityService, MedicalCare.API.Services.SpecialityService>();
builder.Services.AddScoped<MedicalCare.API.Services.IDoctorService, MedicalCare.API.Services.DoctorService>();
builder.Services.AddScoped<MedicalCare.API.Services.IPatientService, MedicalCare.API.Services.PatientService>();
builder.Services.AddScoped<MedicalCare.API.Services.IAppointmentService, MedicalCare.API.Services.AppointmentService>();

var app = builder.Build();

app.UseCors("AllowBlazor");

// 4. Activar la interfaz gráfica de Swagger
app.UseSwagger();
app.UseSwaggerUI();

// 5. Mapear las rutas de nuestros controladores
app.MapControllers();

app.Run();