using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplicacion_ReservasStyle.DTOs
{
    public class PromocionDTO
    {
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public decimal PorcentajeDescuento { get; set; }
        public DateOnly FechaInicio { get; set; }
        public DateOnly FechaFin { get; set; }
        public bool Estado { get; set; }
    }
}
