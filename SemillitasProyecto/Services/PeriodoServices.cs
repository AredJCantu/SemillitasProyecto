using SemillasCore.DTOs;
using SemillitasProyecto.Entities;
using SemillitasProyecto.Mappers;
using SemillitasProyecto.Repositories;

namespace SemillitasProyecto.Services
{
    public class PeriodoServices
    {
        public PeriodoServices(Repository<Periodos> repository)
        {
            Repository = repository;
        }

        public Repository<Periodos> Repository { get; }

        public IEnumerable<PeriodoDTO> GetAll() {
            var data = Repository.GetAll().Select(p=>new PeriodoDTO {
                Id= p.Id,
                MesInicio= p.MesInicio,
                MesFin=p.MesFin,
                Año = p.Año
            });
            return data;
        }
        public PeriodoDTO? GetById(int id)
        {
            var data = Repository.GetById(id);
            if (data != null)
            {
                return data.ToDTO();
            }
            return null;
        }
        public PeriodoDTO? Create(CreatePeriodoDTO dto)
        {
            Periodos entity = dto.ToEntity();
            Repository.Insert(entity);
            return entity.ToDTO();
        }
        public PeriodoDTO? Update(EditPeriodoDTO dto,int id) {
            var entity=Repository.GetById(id);
            if (entity != null)
            {
                dto.ApplyTo(entity);
                Repository.Update(entity);
                return entity.ToDTO();
            }
            return null;
        }
        public bool Delete(int id) {
            var entity= Repository.GetById(id);
            if (entity != null)
            {
                Repository.Delete(entity);
                return true;
            }
            return false;
        }

    }
}
