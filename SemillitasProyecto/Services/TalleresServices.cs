using Microsoft.EntityFrameworkCore;
using SemillasCore.DTOs;
using SemillitasProyecto.Data;
using SemillitasProyecto.Entities;
using SemillitasProyecto.Mappers;
using SemillitasProyecto.Repositories;

namespace SemillitasProyecto.Services
{
    public class TalleresServices
    {
        public TalleresServices(Repository<Talleres> repository)
        {
            Repository = repository;
        }

        public Repository<Talleres> Repository { get; }

        public IEnumerable<TallerDTO> GetAll()
        {
            var data = Repository.GetAll().Select(t => t.ToDTO());
            return data;
        }
        public TallerDTO? GetById(int id)
        {
            var data = Repository.GetById(id);
            if (data != null)
            {
                return data.ToDTO();
            }
            return null;
        }
        public DatosTallerDTO? GetDatosById(int id)
        {
            Talleres? data = Repository.Query().Where(X => X.Id == id).Include(x => x.Inscripciones).ThenInclude(p=>p.IdAlumnoNavigation).FirstOrDefault();
            if (data != null)
            {
                return data.ToDatosTallerDTO();
            }
            return null;
        }
        public TallerDTO? Create(CreateTallerDTO dto)
        {

            Talleres Entity = dto.ToEntity();
            Repository.Insert(Entity);
            return Entity.ToDTO();
        }
        public TallerDTO? Update(EditTallerDTO dto, int id)
        {
            var entity = Repository.GetById(id);
            if (entity != null)
            {
                dto.ApplyTo(entity);
                Repository.Update(entity);
                return entity.ToDTO();
            }
            return null;
        }

        public bool Delete(int id) {
            var entity = Repository.GetById(id);
            if (entity != null)
            {
                Repository.Delete(entity);
                return true;
            }
            return false;
        }
    }
}
