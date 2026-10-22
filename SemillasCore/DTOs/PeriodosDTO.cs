using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SemillasCore.DTOs
{
    public class CreatePeriodoDTO {
        public string MesInicio { get; set; } = null!;
        public string MesFin { get; set; } = null!;
    }
    public class PeriodoDTO : CreatePeriodoDTO {
        public int Id { get; set; }
        
    }
    public class MostrarPeriodoDTO:CreatePeriodoDTO {
        public override string ToString()
        {
            return $"{MesInicio}-{MesFin}";
        }
    }
    
}
