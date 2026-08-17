using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Dominio_ReservasStyle.Entities
{
    public class Empleado
    {
        public int IdEmpleado { get; set; }

        public string Nombre { get; set; }

        public string Apellido { get; set; }

        public string Telefono { get; set; }

        public int IdSucursal { get; set; }

        public int IdUsuario { get; set; }

        public bool Estado { get; set; }

        public Usuario Usuario { get; set; }
    }
}
