using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SemillasCore.DTOs
{
    public class CreateTallerDTO
    {
        [Required(ErrorMessage = "El nombre del taller no debe ir vació.")]
        public string Nombre { get; set; } = null!;
        [Required(ErrorMessage = "El taller debe tener un costo fijo.")]
        
        public decimal Costo { get; set; }
        [Required(ErrorMessage = "El taller debe tener una hora de inicio.")]
        [RegularExpression(@"^(0[0-9])|(1[0-9])|(2[0-4])$")]
        public int HoraInicio { get; set; }
        [Required(ErrorMessage = "El taller debe tener una hora final.")]
        [RegularExpression(@"^(0[0-9])|(1[0-9])|(2[0-4])$")]
        public int HoraFin { get; set; }
        [Required(ErrorMessage = "El taller debe tener un cupo de alumnos.")]
        public int Cupo { get; set; }
        [Required(ErrorMessage = "Se necesita un docente para el taller.")]
        public string NombreInstructor { get; set; } = null!;
        [Required(ErrorMessage = "Seleccione una categoria para el taller")]
        public string Categoria { get; set; } = null!;
        [Required(ErrorMessage = "Seleccione a que periodo pertenece ese taller.")]
        public int IdPeriodo { get; set; }
        [Required(ErrorMessage = "El taller debe tener una hora inicial sabatina.")]
        [RegularExpression(@"^(0[0-9])|(1[0-9])|(2[0-4])$")]
        public int HoraInicioSabado { get; set; }
        [Required(ErrorMessage = "El taller debe tener una hora final sabatina.")]
        [RegularExpression(@"^(0[0-9])|(1[0-9])|(2[0-4])$")]
        public int HoraFinSabado { get; set; }
    }
    public class DatosTallerDTO {
        public int Id { get; set; }
        public string Nombre { get; set; } = null!;
        public int Cupo { get; set; }
        public string NombreInstructor { get; set; } = null!;
        public int IdPeriodo { get; set; }
        public IEnumerable<AlumnoDTO>? alumnos { get; set; }
    }
    public class TallerDTO : CreateTallerDTO {
        public int Id { get; set; }
    }
    public class EditTallerDTO:CreateTallerDTO
    {
    }
}
