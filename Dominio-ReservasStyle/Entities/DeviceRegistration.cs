namespace Dominio_ReservasStyle.Entities
{
    public class DeviceRegistration
    {
        public int IdDeviceRegistration { get; set; }
        public int IdUsuario { get; set; }
        public string FcmToken { get; set; } = string.Empty;
        public string Plataforma { get; set; } = string.Empty;
        public DateTime FechaActualizacion { get; set; }
        public Usuario Usuario { get; set; } = null!;
    }
}
