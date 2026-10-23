using SemillasCore.DTOs;
using SemillitasProyecto.Entities;
using SemillitasProyecto.Mappers;
using SemillitasProyecto.Repositories;

namespace SemillitasProyecto.Services
{
    public class InscripcionesServices
    {
        public InscripcionesServices(Repository<Inscripciones> repository)
        {
            Repository = repository;
        }

        public Repository<Inscripciones> Repository { get; }

        public IEnumerable<InscripcionesDTO> GetAll() {
            var data = Repository.GetAll().Select(x=>x.ToDTO());
            return data;
        }
        public InscripcionesDTO? GetById(int id) {
            var data = Repository.GetById(id);
            if (data != null)
            {
                return data.ToDTO();
            }
            return null;
        }
        public IEnumerable<DatosInscripcionesDTO> GetDatosInscripciones() {
            var data = Repository.GetAll().Select(x => x.ToDatosInscripcionesDTO());
            return data;
        }
        public InscripcionesDTO? Create(CreateInscripcionesDTO dto) {
            var entity = dto.ToEntity();
            if (entity != null)
            {
                Repository.Insert(entity);
                return entity.ToDTO();
            }
            return null;
        }
        public InscripcionesDTO? Update(EditInscripcionesDTO dto, int id)
        {
            var entity = Repository.GetById(id);
            if (entity != null) {
                dto.ApplyTo(entity);
                Repository.Update(entity);
                return entity.ToDTO();
            }
            return null;
        }
        public bool Delete(int id) {
            var data = Repository.GetById(id);
            if (data != null) {
                Repository.Delete(data);
                return true;
            }
            return false;
        }


    }
}
