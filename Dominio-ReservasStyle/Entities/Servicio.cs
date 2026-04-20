using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Dominio_ReservasStyle.Entities
{
    public class Servicio
    {
        public int IdServicio { get; set; }
        public string Nombre { get; set; }
        public string? Descripcion { get; set; }
        public int DuracionMinutos { get; set; }
        public string ImagenUrl { get; set; }
        public bool Estado { get; set; }

        public Sucursal Sucursal { get; set; }

        public ICollection<ServicioSucursal> ServicioSucursales { get; set; } = new List<ServicioSucursal>();

    }
}
