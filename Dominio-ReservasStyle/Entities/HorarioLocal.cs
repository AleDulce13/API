using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Dominio_ReservasStyle.Entities
{
    public class HorarioLocal
    {
        public int IdHorarioLocal { get; set; }
        public int IdSucursal { get; set; }
        public string DiaSemana { get; set; }
        public TimeSpan HoraApertura { get; set; }
        public TimeSpan HoraCierre { get; set; }
        public bool Estado { get; set; }

        [JsonIgnore]
        public Sucursal Sucursal { get; set; }
    }
}
