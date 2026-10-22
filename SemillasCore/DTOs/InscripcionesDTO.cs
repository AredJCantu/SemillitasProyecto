using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SemillasCore.DTOs
{

    public class CreateInscripcionesDTO {
        public int IdAlumno { get; set; }

        public int IdTaller { get; set; }

        public bool Debe { get; set; }
    }
    public class InscripcionesDTO:CreateInscripcionesDTO {
        public int Id { get; set; }
    }
    public class MostrarInscripciones
    {
        public string Nombre_Taller { get; set; } = null!;
        public bool Debe { get; set; }
    }
}
