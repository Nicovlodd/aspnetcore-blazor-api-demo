# MedicalCare API - Gestión de Atenciones Médicas

Esta es una API RESTful desarrollada en **.NET 8** para la gestión integral de pacientes, doctores y atenciones médicas.

##  Tecnologías y Stack
* **Backend:** API RESTful en .NET 8 (C#)
* **Frontend:** Blazor WebAssembly (.NET 8)
* **Base de Datos:** SQL Server
* **ORM:** Dapper (Micro-ORM elegido por su alto rendimiento y control absoluto de consultas SQL).
* **Documentación:** Swagger UI (Integrado con soporte para cabeceras de seguridad).
* **Testing:** xUnit + Moq


###  Decisiones de Arquitectura (Trade-offs)
Se optó por un **enfoque arquitectónico híbrido**:
1. **Dapper para operaciones transaccionales estándar:** Se delegó la persistencia (CRUD) y las consultas dinámicas a la capa de aplicación usando Dapper. Esto maximiza el rendimiento, facilita el mantenimiento del código en C# y protege contra inyección SQL mediante parámetros dinámicos.
2. **Procedimientos Almacenados para reglas de negocio críticas:** La validación de integridad —específicamente la **prevención de solapamiento de agendas médicas**— se encapsuló en un Procedimiento Almacenado (`sp_CreateAppointment`). Esta decisión garantiza la atomicidad y consistencia estricta directamente en el motor relacional, evitando condiciones de carrera en escenarios de alta concurrencia.
3. **Inyección de Dependencias:** Todos los servicios y la fábrica de conexiones (`DbConnectionFactory`) se inyectan mediante el contenedor nativo de .NET (`IService`), facilitando el desacoplamiento y las pruebas unitarias.


### Desacoplamiento y Arquitectura Limpia: 
Se separó físicamente la interfaz de usuario (Blazor) de la lógica del servidor (API REST). 
El backend se estructuró basándose en el patrón MVC (Controladores, Servicios y Acceso a Datos), lo que garantiza una estricta separación de responsabilidades.
Esta decisión permite mantener un código limpio (Clean Code) y asegura que el ecosistema sea altamente escalable a medida que la clínica requiera integrar nuevos módulos en el futuro.

### Documentación Viva (Swagger): 
Al estar el sistema desacoplado, Swagger no solo actúa como interfaz de prueba, 
sino como el contrato técnico estricto entre el Backend y cualquier cliente (Frontend, Móvil o integraciones de terceros)
garantizando que la documentación nunca quede obsoleta respecto al código en producción.


### Seguridad
Todos los endpoints de la API están protegidos contra accesos no autorizados. Se implementó una validación por **API Key** mediante un Middleware personalizado que intercepta todas las peticiones y exige la cabecera HTTP `x-api-key`.

Para probar desde Swagger:
1. Al ingresar a Swagger, haz clic en el botón verde **Authorize** (arriba a la derecha).
2. Ingresa la siguiente clave de acceso: `ClinicaMedicalCareSecretKey2026`
*(Esta clave está configurada en el archivo `appsettings.json` y puede ser modificada si se requiere).*

##  Instrucciones de Setup y Ejecución

### 1. Base de Datos
1. Ejecutar el script `scripts.sql` adjunto en el gestor de SQL Server. para cargar tablas

### 2. Backend (API)
1. Abrir el archivo `appsettings.json` en el proyecto `MedicalCare.API` y actualizar el `DefaultConnection` con tu cadena de conexión local.
2. Abrir una terminal en la raíz del proyecto Backend y ejecutar:
   dotnet restore
   dotnet run

### 3. Abrir Frontend (Blazor WebAssembly)
1. Abrir una nueva terminal en la carpeta MedicalCare.Frontend.
2. ejecutar:
   ```bash
   dotnet restore
   dotnet run

### 4. Pruebas y Testing

Se incluye una capa de pruebas unitarias en el proyecto MedicalCare.Tests.
Se aplicó el patrón Arrange, Act, Assert.
Se utilizó Moq para simular la capa de servicios, permitiendo probar la lógica de los Controladores de forma aislada sin tocar la base de datos.

1.- Para ejectuar pruebas debe ir a la carpeta MedicalCare.Tests
1.1.- en un nuevo cmd correr "dotnet test"


### 5. Probar API desde archivo MedicalCare.http

Si se trabaja desde Visual Studio 2022, se puede utilizar el archivo MedicalCare.http 
incluido en la raíz para probar la API realizando peticiones con un solo clic. 
En caso de trabajar con VS Code, se recomienda instalar la extensión llamada REST Client 
para la misma funcionalidad.