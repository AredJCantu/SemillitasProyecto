using SemillitasProyecto.Commons;
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
        public string Nombre { get; set; } = null!;
        public decimal Costo { get; set; }
        public int HoraInicio { get; set; }
        
        public int HoraFin { get; set; }
        public int Cupo { get; set; }
        public string NombreInstructor { get; set; } = null!;
        public Categorias Categoria { get; set; }
        public int IdPeriodo { get; set; }
        public int HoraInicioSabado { get; set; }
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
    public class NombreTalleres {
        public string Nombre { get; set; }
    }
    public class TallerDTO {
        public int Id { get; set; }
        public string Nombre { get; set; } = null!;
        public string Horario { get; set; } = "";
        public int Cupo { get; set; }
        public int CantidadAlumnos { get; set; }
        public decimal Costo { get; set; }
        public int IdPeriodo { get; set; }
        public int HoraInicio { get; set; }
        public int HoraFin { get; set; }
        public int HoraInicioSabado { get; set; }
        public int HoraFinSabado { get; set; }
        public Categorias Categoria { get; set; }          
        public string NombreInstructor { get; set; } = ""; 
    }
    public class EditTallerDTO:CreateTallerDTO
    {
        public int Id { get; set; }
    }
}
