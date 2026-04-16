using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio_ReservasStyle.Entities
{
    public class Log
    {
        public int IdLog { get; set; }
        public int? IdUsuario { get; set; }
        public string Accion { get; set; }
        public string? Descripcion { get; set; }
        public string? TablaAfectada { get; set; }
        public int? RegistroId { get; set; }
        public DateTime Recha { get; set; }
        public string? Ip { get; set; }

        public Usuario Usuario { get; set; }
    }
}
