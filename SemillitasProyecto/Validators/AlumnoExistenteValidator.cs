using SemillasCore.DTOs;
using SemillasCore.Validators;
using SemillitasProyecto.Entities;
using SemillitasProyecto.Repositories;

namespace SemillitasProyecto.Validators
{
    public class AlumnoExistenteValidator:EditAlumnoValidator
    {
        public AlumnoExistenteValidator(Repository<Alumnos> repos)
        {
            Repos = repos;
        }

        public Repository<Alumnos> Repos { get; }

        public override ValidatorClass IsValid(EditAlumnoDTO dto)
        {
            ValidatorClass res = base.IsValid(dto);
            if (Repos.GetAll().Any(x => x.Nombre == dto.Nombre && x.Apellido == dto.Apellido && x.NombreTutor == dto.NombreTutor
            && x.Telefono == dto.Telefono && x.Id == dto.Id))
                res.Errores.Add("Ya existe un alumno con los mismos datos.");
            return res;
        }
    }
}
