using Dominio_ReservasStyle.Entities;
using Infraestructura_ReservasStyle.Data;
using Microsoft.EntityFrameworkCore;

namespace Aplicacion_ReservasStyle.Services
{
    public class DeviceRegistrationService : IDeviceRegistrationService
    {
        private static readonly HashSet<string> SupportedPlatforms = new(StringComparer.OrdinalIgnoreCase) { "android", "wearos" };
        private readonly AppDbContext _context;
        public DeviceRegistrationService(AppDbContext context) => _context = context;

        public async Task RegisterAsync(int userId, string token, string platform)
        {
            token = token?.Trim() ?? string.Empty;
            platform = platform?.Trim().ToLowerInvariant() ?? string.Empty;
            if (token.Length is < 20 or > 4096) throw new ArgumentException("El token FCM no tiene una longitud válida.");
            if (!SupportedPlatforms.Contains(platform)) throw new ArgumentException("La plataforma debe ser android o wearos.");

            var registration = await _context.DeviceRegistrations.SingleOrDefaultAsync(item => item.FcmToken == token);
            if (registration is null)
            {
                registration = new DeviceRegistration { FcmToken = token };
                _context.DeviceRegistrations.Add(registration);
            }
            registration.IdUsuario = userId;
            registration.Plataforma = platform;
            registration.FechaActualizacion = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }

        public async Task RemoveAsync(int userId, string token)
        {
            var registration = await _context.DeviceRegistrations.SingleOrDefaultAsync(item => item.IdUsuario == userId && item.FcmToken == token);
            if (registration is null) return;
            _context.DeviceRegistrations.Remove(registration);
            await _context.SaveChangesAsync();
        }
    }
}
