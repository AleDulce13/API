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

            // IGNORAR SWAGGER
            if (path.StartsWith("/swagger"))
            {
                await _next(context);
                return;
            }

            // IGNORAR LOGIN Y REGISTER
            if (path.Contains("/api/Auth/register") ||
                path.Contains("/api/Auth/login"))
            {
                await _next(context);
                return;
            }

            // Obtener usuario desde JWT
            var userIdClaim =
                context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            int? userId = null;

            if (int.TryParse(userIdClaim, out int parsedId))
                userId = parsedId;

            await _next(context);

            stopwatch.Stop();

            await logService.Crear(new LogDTO
            {
                IdUsuario = userId,
                Accion = $"{method} {path}",
                Descripcion = $"HTTP {context.Response.StatusCode} - {stopwatch.ElapsedMilliseconds}ms",
                TablaAfectada = path,
                RegistroId = null,
                Ip = context.Connection.RemoteIpAddress?.ToString()
            });
        }
    } 
}
