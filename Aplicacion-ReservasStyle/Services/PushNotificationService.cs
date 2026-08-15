using Dominio_ReservasStyle.Entities;
using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Infraestructura_ReservasStyle.Data;
using Microsoft.EntityFrameworkCore;

namespace Aplicacion_ReservasStyle.Services
{
    public class PushNotificationService : IPushNotificationService
    {
        private readonly AppDbContext _context;
        public PushNotificationService(AppDbContext context) => _context = context;

        public async Task NotifyAsync(int recipientUserId, string title, string body, int citaId, string eventName)
        {
            var registrations = await _context.DeviceRegistrations.Where(item => item.IdUsuario == recipientUserId).ToListAsync();
            _context.Notificaciones.Add(new Notificacion { IdUsuario = recipientUserId, Mensaje = body, FechaEnvio = DateTime.UtcNow, Leida = false });
            await _context.SaveChangesAsync();

            if (registrations.Count == 0) return;
            FirebaseMessaging messaging;
            try { messaging = FirebaseMessaging.DefaultInstance; }
            catch (Exception exception)
            {
                Console.WriteLine($"Firebase no está disponible: {exception.Message}");
                return;
            }
            foreach (var registration in registrations)
            {
                try
                {
                    await messaging.SendAsync(new Message
                    {
                        Token = registration.FcmToken,
                        Data = new Dictionary<string, string> { ["citaId"] = citaId.ToString(), ["event"] = eventName, ["title"] = title, ["body"] = body },
                        Android = new AndroidConfig { Priority = Priority.High }
                    });
                }
                catch (FirebaseMessagingException exception) when (exception.MessagingErrorCode == MessagingErrorCode.Unregistered)
                {
                    _context.DeviceRegistrations.Remove(registration);
                    await _context.SaveChangesAsync();
                }
                catch (Exception exception)
                {
                    Console.WriteLine($"No se pudo enviar push a la instalación {registration.IdDeviceRegistration}: {exception.Message}");
                }
            }
        }
    }
}
