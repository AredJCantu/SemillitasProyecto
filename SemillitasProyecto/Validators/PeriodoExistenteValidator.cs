using SemillasCore.DTOs;
using SemillasCore.Validators;
using SemillitasProyecto.Entities;
using SemillitasProyecto.Repositories;

namespace SemillitasProyecto.Validators
{
    public class PeriodoExistenteValidator:CreatePeriodoValidator
    {
        private readonly Repository<Periodos> repos;

        public PeriodoExistenteValidator(Repository<Periodos> repos)
        {
            this.repos = repos;
        }
        public override ValidatorClass IsValid(CreatePeriodoDTO dto)
        {
            ValidatorClass result= base.IsValid(dto);
            if (repos.GetAll().Any(x => x.MesInicio == dto.MesInicio && x.MesFin == dto.MesFin && x.Año == dto.Año))
                result.Errores.Add("Ya existe un periodo con los mismos datos");
            return result;

        }
    }
}
