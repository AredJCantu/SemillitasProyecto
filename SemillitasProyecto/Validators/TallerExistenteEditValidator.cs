using SemillasCore.DTOs;
using SemillasCore.Validators;
using SemillitasProyecto.Entities;
using SemillitasProyecto.Mappers;
using SemillitasProyecto.Repositories;

namespace SemillitasProyecto.Validators
{
    public class TallerExistenteEditValidator:EditTallerValidator
    {
        private readonly Repository<Talleres> repos;

        public TallerExistenteEditValidator(Repository<Talleres> repos)
        {
            this.repos = repos;
        }
        public override ValidatorClass IsValid(EditTallerDTO dto)
        {
            var res= base.IsValid(dto);
            if (repos.GetAll().Any(x => x.Nombre == dto.Nombre && x.HoraInicio==dto.HoraInicio && x.HoraFin==dto.HoraFin && x.IdPeriodo == dto.IdPeriodo && x.Id != dto.Id))
                res.Errores.Add("Ya existe un taller con los mismos datos");
            return res;
        }
    }
}
