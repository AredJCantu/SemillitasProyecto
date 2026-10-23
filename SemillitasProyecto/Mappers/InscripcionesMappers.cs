using SemillasCore.DTOs;
using SemillitasProyecto.Entities;

namespace SemillitasProyecto.Mappers
{
    public static class InscripcionesMappers
    {
        public static InscripcionesDTO? ToDTO(this Inscripciones entity)
        {
            InscripcionesDTO inscripcion = new() {
                Id = entity.Id,
                IdAlumno = entity.IdAlumno,
                IdTaller = entity.IdTaller,
                Debe=entity.Debe
            };
            return inscripcion;
        }
        public static Inscripciones? ToEntity(this CreateInscripcionesDTO dto)
        {
            Inscripciones inscripcion = new()
            {
                IdAlumno= dto.IdAlumno,
                IdTaller= dto.IdTaller,
                Debe= dto.Debe
            };
            return inscripcion;
        }
        public static void ApplyTo(this EditInscripcionesDTO dto, Inscripciones entity)
        {
            entity.IdAlumno=dto.IdAlumno;
            entity.IdTaller=dto.IdTaller;
            entity.Debe=dto.Debe;
        }
        public static DatosInscripcionesDTO? ToDatosInscripcionesDTO(this Inscripciones entity) {
            DatosInscripcionesDTO inscripcion = new() {
                Id=entity.Id,
                NombreAlumno=entity.IdAlumnoNavigation.Nombre,
                NombreTaller=entity.IdTallerNavigation.Nombre,
                Debe=entity.Debe
            };
            return inscripcion;
        }
    }
}
