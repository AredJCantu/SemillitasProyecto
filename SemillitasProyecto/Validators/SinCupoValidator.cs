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

namespace SemillasCore.Validators;

    public class SinCupoValidator
    {

    private Repository<Periodos> repos;

    public SinCupoValidator(Repository<Periodos> repos,Repository<Talleres> reposTaller)
    {
        this.repos = repos;
        ReposTaller = reposTaller;
    }

    public Repository<Talleres> ReposTaller { get; }

    public ValidatorClass IsValid(CreateInscripcionesDTO dto)
    {
        //Validar un taller sin cupo
        ValidatorClass res = new();
        var taller = ReposTaller.GetById(dto.IdTaller);
        var periodo = repos.GetById(taller.IdPeriodo);
        if (taller.Cupo==0)
            res.Errores.Add("No se puede Inscribir a un taller sin cupo.");
        //Validar que no haya empezado el taller
        if (periodo.MesInicio < DateTime.Now.Month && periodo.Año <= DateTime.Now.Year)
            res.Errores.Add("El taller ya empezo");
        return res;
    }

}
