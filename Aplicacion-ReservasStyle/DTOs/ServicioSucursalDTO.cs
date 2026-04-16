using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplicacion_ReservasStyle.DTOs
{
    public class ServicioSucursalDTO
    {
        public int IdServicio { get; set; }
        public int IdSucursal { get; set; }
        public decimal Precio { get; set; }
        public bool Estado { get; set; }
    }
}
