using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SemillasCore.DTOs
{
    public class CreateAlumnoDTO {
        public string Nombre { get; set; } = null!;

        public string Apellido { get; set; } = null!;

        public string NombreTutor { get; set; } = null!;

        public string ApellidoTutor { get; set; } = null!;

        public string Telefono { get; set; } = null!; 
    }
    public class AlumnoDTO:CreateAlumnoDTO {
        public int Id { get; set; }
    }
    public class EditAlumnoDTO:CreateAlumnoDTO {
        
    }
    
}
