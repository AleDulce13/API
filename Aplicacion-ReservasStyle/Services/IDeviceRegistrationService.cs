namespace Aplicacion_ReservasStyle.Services
{
    public interface IDeviceRegistrationService
    {
        Task RegisterAsync(int userId, string token, string platform);
        Task RemoveAsync(int userId, string token);
    }
}
