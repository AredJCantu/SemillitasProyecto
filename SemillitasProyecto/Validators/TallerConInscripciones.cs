using SemillasCore.Validators;
using SemillitasProyecto.Entities;
using SemillitasProyecto.Repositories;

namespace SemillitasProyecto.Validators
{
    public class TallerConInscripciones
    {
        private Repository<Inscripciones> repos;

        public TallerConInscripciones(Repository<Inscripciones> repos)
        {
            this.repos = repos;
        }
        public ValidatorClass IsValid(int id)
        {
            //Validar que no pueda eliminar un taller con inscripciones
            ValidatorClass res = new();
            if (repos.GetAll().Any(x => x.IdTaller == id))
                res.Errores.Add("No se puede eliminar un taller con alumnos inscritos. \n Elimine o cambie las inscripciones de los alumnos de taller.");
            return res;
        }
    }
}
