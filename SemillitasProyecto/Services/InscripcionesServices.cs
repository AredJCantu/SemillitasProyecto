using SemillasCore.DTOs;
using SemillitasProyecto.Entities;
using SemillitasProyecto.Mappers;
using SemillitasProyecto.Repositories;

namespace SemillitasProyecto.Services
{
    public class InscripcionesServices
    {
        public InscripcionesServices(Repository<Inscripciones> repository, Repository<Talleres> repoTalleres)
        {
            Repository = repository;
            RepoTalleres = repoTalleres;
        }

        public Repository<Inscripciones> Repository { get; }
        public Repository<Talleres> RepoTalleres { get; }

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
        private void MoverCupo(int idTaller, int cambio)
        {
            var taller = RepoTalleres.GetById(idTaller);
            if (taller == null) return;
            taller.Cupo += cambio;
            if (taller.Cupo < 0) taller.Cupo = 0;   // nunca negativo
            RepoTalleres.Update(taller);
        }
        public IEnumerable<DatosInscripcionesDTO> GetDatosInscripciones() {
            var data = Repository.GetAll().Select(x => x.ToDatosInscripcionesDTO());
            return data;
        }
        public InscripcionesDTO Create(CreateInscripcionesDTO dto) {
            var entity = dto.ToEntity();
            if (entity != null)
            {
                Repository.Insert(entity);
                MoverCupo(entity.IdTaller, -1);
                return entity.ToDTO();
            }
            return null;
        }
        public InscripcionesDTO? Update(EditInscripcionesDTO dto, int id)
        {
            var entity = Repository.GetById(id);
            if (entity != null) {
                var tallerAnterior = entity.IdTaller;
                dto.ApplyTo(entity);
                Repository.Update(entity);
                if (tallerAnterior != entity.IdTaller)
                {
                    MoverCupo(tallerAnterior, +1);
                    MoverCupo(entity.IdTaller, -1);
                }
                return entity.ToDTO();
            }
            return null;
        }
        public bool Delete(int id) {
            var data = Repository.GetById(id);
            if (data != null) {
                Repository.Delete(data);
                MoverCupo(idTaller, +1);
                return true;
            }
            return false;
        }


    }
}
