using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace MedicalCare.API.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class ApiKeyAttribute : Attribute, IAsyncActionFilter
    {
        private const string APIKEYNAME = "X-API-KEY";

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            // 1. Extraer el servicio de configuración
            var configuration = context.HttpContext.RequestServices.GetRequiredService<IConfiguration>();
            var extractedApiKey = configuration.GetValue<string>("ApiKeySettings:SecretKey");

            // 2. Verificar si la cabecera existe
            if (!context.HttpContext.Request.Headers.TryGetValue(APIKEYNAME, out var extractedHeaderValue))
            {
                context.Result = new ContentResult()
                {
                    StatusCode = 401,
                    Content = "Acceso denegado. No se proporcionó una API Key válida en la cabecera."
                };
                return;
            }

            // 3. Validar si la clave coincide
            if (!extractedApiKey!.Equals(extractedHeaderValue))
            {
                context.Result = new ContentResult()
                {
                    StatusCode = 403,
                    Content = "Acceso prohibido. La API Key proporcionada es incorrecta."
                };
                return;
            }
            await next();
        }
    }
}