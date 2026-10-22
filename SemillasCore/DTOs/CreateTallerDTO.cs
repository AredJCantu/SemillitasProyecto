using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SemillasCore.DTOs
{
    public class CreateTallerDTO
    {
        public string Nombre { get; set; } = null!;

        public decimal Costo { get; set; }

        public int HoraInicio { get; set; }

        public int HoraFin { get; set; }

        public int Cupo { get; set; }

        public string NombreInstructor { get; set; } = null!;

        public string Categoria { get; set; } = null!;

        public int IdPeriodo { get; set; }
    }
    public class DatosTallerDTO {
        public int Id { get; set; }
        public string Nombre { get; set; } = null!;
    }
    public class TallerDTO : CreateTallerDTO {
        public int Id { get; set; }
    }
}
