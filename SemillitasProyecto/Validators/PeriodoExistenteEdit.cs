using SemillasCore.DTOs;
using SemillasCore.Validators;
using SemillitasProyecto.Entities;
using SemillitasProyecto.Repositories;

namespace SemillitasProyecto.Validators
{
    public class PeriodoExistenteEdit:EditPeriodoValidator
    {
        private readonly Repository<Periodos> repos;

        public PeriodoExistenteEdit(Repository<Periodos> repos)
        {
            this.repos = repos;
        }
        public override ValidatorClass IsValid(EditPeriodoDTO dto)
        {
            ValidatorClass result = base.IsValid(dto);
            if (repos.Query().ToList().Any(x => x.MesInicio == dto.MesInicio && x.MesFin == dto.MesFin && x.Año == dto.Año && x.Id != dto.Id))
                result.Errores.Add("Ya existe un periodo con esas caracteristicas");
            return result;
        }
    }
}
