using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio_ReservasStyle.Entities
{
    public class PromocionServicio
    {
        public int IdPromocion { get; set; }
        public int IdServicioSucursal { get; set; }

        public Promocion Promocion { get; set; }
        public ServicioSucursal ServicioSucursal { get; set; }
    }
}
