using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplicacion_ReservasStyle.DTOs
{
    public class RegistrarSucursal
    {
        public string Nombre { get; set; }
        public string Direccion { get; set; }
        public string Ciudad { get; set; }
        public string Estado { get; set; }
        public string CodigoPostal { get; set; }
        public string Telefono { get; set; }
        public bool EstadoActivo { get; set; }

        public List<HorarioDTO> Horarios { get; set; }
    }

    public class HorarioDTO
    {
        public string DiaSemana { get; set; }
        public string HoraApertura { get; set; }
        public string HoraCierre { get; set; }
        public bool Estado { get; set; }
    }
}

