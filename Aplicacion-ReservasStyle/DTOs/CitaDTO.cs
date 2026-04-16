using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplicacion_ReservasStyle.DTOs
{
    public class CitaDTO
    {
        public int IdCliente { get; set; }
        public int IdEmpleado { get; set; }
        public int IdServicioSucursal { get; set; }

        public DateTime Fecha { get; set; }
        public TimeSpan HoraInicio { get; set; }
        public TimeSpan HoraFin { get; set; }

        public string Estado { get; set; }
    }
}
