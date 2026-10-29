using SemillasCore.DTOs;
using SemillitasProyecto.Entities;

namespace SemillitasProyecto.Mappers
{
    public static class AlumnosMappers
    {
        public static AlumnoDTO ToDTO(this Alumnos entity) {
            AlumnoDTO a = new() {
                Id = entity.Id,
                Nombre = entity.Nombre,
                Apellido = entity.Apellido,
                NombreTutor = entity.NombreTutor,
                Telefono=entity.Telefono
            };
            return a;
        }
        public static Alumnos ToEntity(this CreateAlumnoDTO dto) {
            Alumnos a = new() {
                Nombre= dto.Nombre,
                Apellido= dto.Apellido,
                NombreTutor= dto.NombreTutor,
                Telefono=dto.Telefono
            };
            return a;
        }
        public static AlumnoConInscripcionesDTO? ToAlumnosConInscripciones(this Alumnos entity) {
            AlumnoConInscripcionesDTO dto = new()
            {
                Id = entity.Id,
                Nombre = entity.Nombre,
                Apellido = entity.Apellido,
                NombreTutor = entity.NombreTutor,
                Telefono = entity.Telefono,
                inscripciones = entity.Inscripciones.Select(i => new AlumnosInscripcionesDTO()
                {
                    Id = i.Id,
                    Debe = i.Debe,
                    NombreTaller = i.IdTallerNavigation.Nombre
                })
            };
            
            return dto;
        }
        public static void ApplyTo(this EditAlumnoDTO dto,Alumnos entity) {
            entity.Nombre= dto.Nombre;
            entity.Apellido= dto.Apellido;
            entity.NombreTutor= dto.NombreTutor;
            entity.Telefono= dto.Telefono;
        }

    }
}
