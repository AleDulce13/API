using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Dominio_ReservasStyle.Entities
{
    public class ServicioSucursal
    {
        public int IdServicioSucursal { get; set; }
        public int IdServicio { get; set; }
        public int IdSucursal { get; set; }
        public decimal Precio { get; set; }
        public bool Estado { get; set; }

        [JsonIgnore]
        public Servicio Servicio { get; set; }
        [JsonIgnore]
        public Sucursal Sucursal { get; set; }
    }
}
