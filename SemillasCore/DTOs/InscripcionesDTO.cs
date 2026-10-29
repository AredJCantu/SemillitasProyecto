using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using SemillasCore.Validators;
using System.Threading.Tasks;

namespace SemillasCore.DTOs
{
    //[EmpalmeValidator]
    //[PeriodoEmpezadoValidator]
    //[SinCupoValidator]
    public class CreateInscripcionesDTO {
        //[Required(ErrorMessage = "Favor de seleccionar un alumno para la inscripción.")]
        public int IdAlumno { get; set; }
        //[Required(ErrorMessage = "Favor de seleccionar a que taller desea inscribirse.")]
        public int IdTaller { get; set; }
        //[Required]
        public bool? Debe { get; set; }
    }
    public class InscripcionesDTO:CreateInscripcionesDTO {
        public int Id { get; set; }
    }
    public class EditInscripcionesDTO : CreateInscripcionesDTO {
    
     }
    public class DatosInscripcionesDTO
    {
        public int Id { get; set; }
        public string NombreAlumno { get; set; } = null!;
        public string NombreTaller { get; set; } = null!;
        public bool? Debe { get; set; }
    }
    public class AlumnosInscripcionesDTO
    {
        public int Id { get; set; }
        public string? NombreTaller { get; set; }
        public bool? Debe { get; set; }
    }
}
