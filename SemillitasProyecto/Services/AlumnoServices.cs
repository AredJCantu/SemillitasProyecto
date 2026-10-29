using Microsoft.EntityFrameworkCore;
using SemillasCore.DTOs;
using SemillitasProyecto.Entities;
using SemillitasProyecto.Mappers;
using SemillitasProyecto.Repositories;

namespace SemillitasProyecto.Services
{
    public class AlumnoServices
    {
        public AlumnoServices(Repository<Alumnos> repository)
        {
            Repository = repository;
        }

        public Repository<Alumnos> Repository { get; }

        public IEnumerable<AlumnoDTO> GetAll() {
            var data = Repository.GetAll().Select(a=>a.ToDTO());
            return data;
        }
        public AlumnoConInscripcionesDTO? GetDatosById(int id)
        {
            var data = Repository.Query().Include(a=>a.Inscripciones).ThenInclude(i=>i.IdTallerNavigation).Where(x=>x.Id==id).FirstOrDefault();
       
            if (data != null)
            {
                return data.ToAlumnosConInscripciones();
            }
            return null;
        }
        public AlumnoDTO Create(CreateAlumnoDTO dto) {
            var entity = dto.ToEntity();
            Repository.Insert(entity);
            return entity.ToDTO();
        }
        public AlumnoDTO? Update(EditAlumnoDTO dto,int id)
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
        public bool Delete (int id)
        {
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
