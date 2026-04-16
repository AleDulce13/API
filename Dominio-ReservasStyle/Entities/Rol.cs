using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Dominio_ReservasStyle.Entities
{
    public class Rol
    {
        public int IdRol { get; set; }
        public string NombreRol { get; set; }

        [JsonIgnore]
        public ICollection<Usuario> Usuarios { get; set; }
    }
}
