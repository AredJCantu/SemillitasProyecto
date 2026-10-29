using Microsoft.EntityFrameworkCore;
using SemillasCore.DTOs;
using SemillitasProyecto.Entities;
using SemillitasProyecto.Repositories;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SemillasCore.Validators
{
    public class EmpalmeValidator:ValidationAttribute
    {
        private readonly Repository<Inscripciones> reposInscripciones;

        public EmpalmeValidator(Repository<Inscripciones> reposInscripciones)
        {
            this.reposInscripciones = reposInscripciones;
        }
        //public virtual ValidatorClass IsValid(CreateInscripcionesDTO dto)
        //{
        //    ValidatorClass res = new();
        //    //Todas las inscripciuones del alumno
        //    var Inscripciones = reposInscripciones.Query().Include(x=>x.IdTallerNavigation).Where(x=>x.IdAlumno==dto.IdAlumno).ToList();
        //    //periodo del taller que se quiere inscribir
        //    var periodo=reposInscripciones.Query().Include(x=>x.IdTallerNavigation).Where(x=>x.IdTaller==dto.IdTaller).ToList();
        //    foreach (var i in Inscripciones) {
        //        if()
        //    }

        //    return res;
        //}
    }
}
