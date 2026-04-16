using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Dominio_ReservasStyle.Entities
{
    public class Sucursal
    {
        public int IdSucursal { get; set; }
        public string Nombre { get; set; }
        public string Direccion { get; set; }
        public string Ciudad { get; set; }
        public string Estado { get; set; }
        public string CodigoPostal { get; set; }
        public string Telefono { get; set; }
        public bool EstadoActivo { get; set; }

        [JsonIgnore]
        public ICollection<Usuario> Usuarios { get; set; }
        [JsonIgnore]
        public ICollection<Empleado> Empleados { get; set; }
        [JsonIgnore]
        public ICollection<ServicioSucursal> ServicioSucursales { get; set; }

    }
}
