using Aplicacion_ReservasStyle.DTOs;
using Aplicacion_ReservasStyle.Services;
using System.Diagnostics;
using System.Security.Claims;

namespace ApiReservasStyle.Middleware
{
    public class LogMiddleware
    {
        private readonly RequestDelegate _next;

        public LogMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, LogService logService)
        {
            var stopwatch = Stopwatch.StartNew();

            var request = context.Request;
            var path = request.Path.ToString();
            var method = request.Method;

            // 🔥 PERMITIR REGISTER Y LOGIN SIN VALIDACIONES
            if (path.StartsWith("/api/Auth/register") ||
                path.StartsWith("/api/Auth/login"))
            {
                await _next(context);
                return;
            }

            // Obtener usuario desde JWT
            var userIdClaim =
                context.User?.FindFirst("idUsuario")?.Value ??
                context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            int? userId = null;

            if (int.TryParse(userIdClaim, out int parsedId))
                userId = parsedId;

            // Ejecutar request
            await _next(context);

            stopwatch.Stop();

            var statusCode = context.Response.StatusCode;

            // Ignorar Swagger
            if (path.StartsWith("/swagger"))
                return;

            // Guardar log
            await logService.Crear(new LogDTO
            {
                IdUsuario = userId,
                Accion = $"{method} {path}",
                Descripcion = $"HTTP {statusCode} - {stopwatch.ElapsedMilliseconds}ms",
                TablaAfectada = path,
                RegistroId = null,
                Ip = context.Connection.RemoteIpAddress?.ToString()
            });
        }
    }
    
}
