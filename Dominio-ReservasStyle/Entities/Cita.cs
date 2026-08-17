using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Dominio_ReservasStyle.Entities
{
    public class Cita
    {
        public int IdCita { get; set; }

        public int IdCliente { get; set; }

        public int IdEmpleado { get; set; }

        public int IdServicioSucursal { get; set; }

        public DateTime Fecha { get; set; }

        public TimeSpan HoraInicio { get; set; }

        public TimeSpan HoraFin { get; set; }

        public string? Estado { get; set; }

        public DateTime FechaCreacion { get; set; }

        // Relaciones

        [JsonIgnore]
        public Usuario Cliente { get; set; }

        public Empleado Empleado { get; set; }

        [JsonIgnore]
        public ServicioSucursal ServicioSucursal { get; set; }
    }
}

