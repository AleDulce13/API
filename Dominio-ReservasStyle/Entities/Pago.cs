using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio_ReservasStyle.Entities
{
    public class Pago
    {
        public int IdPago { get; set; }
        public int IdCita { get; set; }
        public decimal Monto { get; set; }
        public string MetodoPago { get; set; }
        public string EstadoPago { get; set; }
        public string ReferenciaTransaccion { get; set; }
        public DateTime FechaPago { get; set; }
    }
}
