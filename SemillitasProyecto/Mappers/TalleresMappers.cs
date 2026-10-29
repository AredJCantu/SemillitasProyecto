using SemillasCore.DTOs;
using SemillitasProyecto.Commons;
using SemillitasProyecto.Entities;

namespace SemillitasProyecto.Mappers
{
    public static class TalleresMappers
    {
       public static TallerDTO? ToDTO(this Talleres entity)
        {
            TallerDTO taller = new() {
                Id = entity.Id,
                Nombre = entity.Nombre,
                Horario = $"{entity.HoraInicio}-{entity.HoraFin}",
                Cupo = entity.Cupo,
                CantidadAlumnos = entity.Inscripciones.Count,
            };
            return taller;
        }
        public static Talleres ToEntity(this CreateTallerDTO dto) {
            Talleres taller = new() {
                Nombre= dto.Nombre,
                Costo= dto.Costo,
                HoraInicio= dto.HoraInicio,
                HoraFin= dto.HoraFin,
                HoraInicioSabado = dto.HoraInicioSabado,
                HoraFinSabado= dto.HoraFinSabado,
                NombreInstructor= dto.NombreInstructor,
                Categoria = dto.Categoria==Commons.Categorias.arte?"arte":dto.Categoria==Categorias.deportes?"deportes":"tecnología",
                IdPeriodo= dto.IdPeriodo,
                Cupo= dto.Cupo
            };
            return taller;
        }
 //La entidad debe tener el include de inscripciones y las inscripciones deben tener el include de IdAlumnoNavigation
        public static DatosTallerDTO? ToDatosTallerDTO(this Talleres entity) {
            DatosTallerDTO taller = new() {
                Id=entity.Id,
                Nombre=entity.Nombre,
                Cupo=entity.Cupo,
                NombreInstructor=entity.NombreInstructor,
                IdPeriodo=entity.IdPeriodo,
                alumnos=entity.Inscripciones.Select(a=>new AlumnoDTO() {
                    Id=a.Id,
                    Nombre=a.IdAlumnoNavigation.Nombre,
                    Apellido=a.IdAlumnoNavigation.Apellido,
                    NombreTutor=a.IdAlumnoNavigation.NombreTutor,
                    Telefono = a.IdAlumnoNavigation.Telefono
                })
            };
            return taller;
        }
        public static void ApplyTo(this EditTallerDTO dto,Talleres entity)
        {
            entity.Nombre = dto.Nombre;
            entity.Cupo= dto.Cupo;
            entity.Costo=dto.Costo;
            entity.HoraInicio=dto.HoraInicio;
            entity.HoraFin = dto.HoraFin;
            entity.HoraInicioSabado = dto.HoraInicioSabado;
            entity.HoraFinSabado = dto.HoraFinSabado;
            entity.NombreInstructor = dto.NombreInstructor;
            entity.Categoria= dto.Categoria == Commons.Categorias.arte ? "arte" : dto.Categoria == Categorias.deportes ? "deportes" : "tecnología";
            entity.IdPeriodo= dto.IdPeriodo;
        }

        public static NombreTalleres ToNombreTalleresDTO(this Talleres entity)
        {
            NombreTalleres dto = new() {
                Nombre=entity.Nombre
            };
            return dto;
        }

    }
}
