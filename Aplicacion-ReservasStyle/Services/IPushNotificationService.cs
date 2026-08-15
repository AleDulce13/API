namespace Aplicacion_ReservasStyle.Services
{
    public interface IPushNotificationService
    {
        Task NotifyAsync(int recipientUserId, string title, string body, int citaId, string eventName);
    }
}
