using SemillasCore.DTOs;
using SemillasCore.Validators;
using SemillitasProyecto.Entities;
using SemillitasProyecto.Repositories;

namespace SemillitasProyecto.Validators
{
    public class AlumnoConInscripcionValidator
    {
        public AlumnoConInscripcionValidator(Repository<Inscripciones> repos)
        {
            Repos = repos;
        }

        public Repository<Inscripciones> Repos { get; }

        public ValidatorClass IsValid(int id)
        {
            ValidatorClass res =new();
            if (Repos.GetAll().Any(x =>x.IdAlumno==id))
                res.Errores.Add("No se puede borrar un alumno con inscripciones.");
            return res;
        }
    }
}
