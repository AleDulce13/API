using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplicacion_ReservasStyle.DTOs
{
    public class LogDTO
    {
        public int? IdUsuario { get; set; }
        public string Accion { get; set; }
        public string? Descripcion { get; set; }
        public string? TablaAfectada { get; set; }
        public int? RegistroId { get; set; }
        public string? Ip { get; set; }
    }
}
