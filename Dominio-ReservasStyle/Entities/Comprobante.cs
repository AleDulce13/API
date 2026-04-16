using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio_ReservasStyle.Entities
{
    public class Comprobante
    {
        public int IdComprobante { get; set; }
        public int IdPago { get; set; }
        public string Folio { get; set; }
        public DateTime FechaEmision { get; set; }
    }
}
