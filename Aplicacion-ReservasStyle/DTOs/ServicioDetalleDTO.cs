using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplicacion_ReservasStyle.DTOs
{
    public class ServicioDetalleDTO
    {
        public string NombreServicio { get; set; }
        public string Sucursal { get; set; }
        public string Direccion { get; set; }
        public string DiaSemana { get; set; }
        public string HoraApertura { get; set; }
        public string HoraCierre { get; set; }
        public decimal Precio { get; set; }
        public string Imagen { get; set; }
    }
}
