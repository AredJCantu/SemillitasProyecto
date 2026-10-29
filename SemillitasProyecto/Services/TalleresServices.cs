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
            var data = Repository.Query().Include(x=>x.Inscripciones).Select(t => t.ToDTO());
            return data;
        }

        public TallerDTO? GetById(int id)
        {
            var data = Repository.Query().Include(x=>x.Inscripciones).Where(x=>x.Id==id).FirstOrDefault();
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
        public TallerDTO Create(CreateTallerDTO dto)
        {
            Talleres Entity = dto.ToEntity();
            Repository.Insert(Entity);
            return Entity.ToDTO();
        }
        public TallerDTO? Update(EditTallerDTO dto)
        {
            var entity = Repository.GetById(dto.Id);
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

        //5 o mas talleres mas solicitados
        public IEnumerable<TallerDTO> GetPopulares() {
            var entities = Repository.Query().Include(x => x.Inscripciones).OrderByDescending(x => x.Inscripciones.Count).Select(x=>x.ToDTO()).Take(5).ToList();
            var ultimo = entities.TakeLast(1).FirstOrDefault();
            var repetidos = Repository.Query().Include(x => x.Inscripciones).OrderByDescending(x => x.Inscripciones.Count).Select(x => x.ToDTO()).Where(x=>x.CantidadAlumnos==ultimo.CantidadAlumnos);
           foreach(var i in repetidos)
            {
                if (!entities.Any(x => x.Id == i.Id)) {
                    entities.Add(i);
                }
            }
            return entities;
        }

    }
}
