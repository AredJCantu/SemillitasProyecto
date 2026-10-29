using SemillasCore.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SemillasCore.Validators
{
    public class CreatePeriodoValidator
    {
        public virtual ValidatorClass IsValid(CreatePeriodoDTO dto) {
            ValidatorClass res = new();
            if (dto.MesInicio <= 0 || dto.MesInicio > 12)
                res.Errores.Add("Mes de inicio inválido. \n Ingrese un mes del año en numero entero.");
            if (dto.MesFin <= 0 || dto.MesFin > 12)
                res.Errores.Add("Mes de cierre inválido. \n Ingrese un mes del año en numero entero.");
            if (dto.Año <= 2000 || dto.Año > 2100)
                res.Errores.Add("Año inválido. \n Ingrese un año mayor a 2000 y menor a 2100");
            return res;
        }
    }
}
