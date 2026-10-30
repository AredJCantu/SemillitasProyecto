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
    public class EmpalmeValidator
    {
        private readonly Repository<Inscripciones> _reposInscripciones;
        private readonly Repository<Talleres> _reposTalleres;

        // Inyectamos los repositorios necesarios a través de tu arquitectura
        public EmpalmeValidator(Repository<Inscripciones> reposInscripciones, Repository<Talleres> reposTalleres)
        {
            _reposInscripciones = reposInscripciones;
            _reposTalleres = reposTalleres;
        }

        public ValidatorClass IsValid(CreateInscripcionesDTO dto)
        {
            ValidatorClass res = new();

            var tallerNuevo = _reposTalleres.GetById(dto.IdTaller);
            if (tallerNuevo == null)
            {
                res.Errores.Add("El taller seleccionado no existe.");
                return res;
            }

            var inscripcionesExistentes = _reposInscripciones.Query()
                .Include(x => x.IdTallerNavigation)
                .Where(x => x.IdAlumno == dto.IdAlumno)
                .ToList();

            foreach (var inscripcion in inscripcionesExistentes)
            {
                var tallerInscrito = inscripcion.IdTallerNavigation;

                bool solapamientoSemana = (tallerInscrito.HoraInicio < tallerNuevo.HoraFin) &&
                                          (tallerNuevo.HoraInicio < tallerInscrito.HoraFin);


                bool    solapamientoSabado = (tallerInscrito.HoraInicioSabado < tallerNuevo.HoraFinSabado) &&
                                         (tallerNuevo.HoraInicioSabado < tallerInscrito.HoraFinSabado);
                

                // Si hay cruce en semana o en sábado, guardamos el error
                if (solapamientoSemana || solapamientoSabado)
                {
                    res.Errores.Add($"El horario se solapa con el taller '{tallerInscrito.Nombre}' en el que ya está inscrito.");
                    return res;
                }
            }

            return res;
        }
    }
}
