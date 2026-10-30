using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SemillasCore.DTOs
{
    public class CreateAlumnoDTO {
        //[Required(ErrorMessage = "Especifique el nombre del alumno.")]
        public string Nombre { get; set; } = null!;
        //[Required(ErrorMessage = "Especifique el apellido del alumno")]
        public string Apellido { get; set; } = null!;
        //[Required(ErrorMessage = "Especifique el tutor del taller")]
        public string NombreTutor { get; set; } = null!;
        [Phone]
        public string Telefono { get; set; } = null!; 
    }
    public class AlumnoDTO:CreateAlumnoDTO {
        public int Id { get; set; }
        public IEnumerable<NombreTalleres> TalleresAlumno { get; set; }
    }
    public class NombreAlumnoDTO {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public override string ToString()
        {
            return Nombre;
        }
    }
    public class AlumnoConInscripcionesDTO : AlumnoDTO
    {
        public IEnumerable<AlumnosInscripcionesDTO>? inscripciones { get; set; }
    }
    public class EditAlumnoDTO:CreateAlumnoDTO {
        public int Id { get; set; }
    }
    
}
