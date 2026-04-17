using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Aplicacion_ReservasStyle.DTOs
{
    public class ServicioCreateDto
    {
        // Servicio
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public int DuracionMinutos { get; set; }
        public bool Estado { get; set; }

        // Imagen
        public IFormFile Imagen { get; set; }

        // ServicioSucursal
        public int IdSucursal { get; set; }
        public decimal Precio { get; set; }
    }
}
