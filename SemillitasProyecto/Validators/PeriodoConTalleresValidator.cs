using SemillasCore.Validators;
using SemillitasProyecto.Entities;
using SemillitasProyecto.Repositories;

namespace SemillitasProyecto.Validators
{
    public class PeriodoConTalleresValidator
    {
        private readonly Repository<Talleres> repos;

        public PeriodoConTalleresValidator(Repository<Talleres> repos)
        {
            this.repos = repos;
        }
        public ValidatorClass IsValid(int id) {
            //Validar que no pueda eliminar un periodo con talleres ya inscritos
            ValidatorClass res = new();
            if (repos.GetAll().Any(x => x.IdPeriodo == id))
                res.Errores.Add("No se puede eliminar un periodo con talleres inscritos. \n Elimine o cambie los talleres de periodo.");
            return res;
        }
    }
}
