using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplicacion_ReservasStyle.DTOs
{
    public class ServicioDTO
    {
        public string Nombre { get; set; } = null!;
        public string? Descripcion { get; set; }
        public int DuracionMinutos { get; set; }
        public string? ImagenUrl { get; set; }
        public bool Estado { get; set; } = true;
    }
}
