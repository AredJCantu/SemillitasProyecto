using SemillasCore.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SemillasCore.Validators
{
    public class CreateTallerValidator
    {
        public virtual ValidatorClass IsValid(CreateTallerDTO dto)
        {
            ValidatorClass res = new();
            if (string.IsNullOrWhiteSpace(dto.Nombre))
                res.Errores.Add("Nombre del taller inválido. \n El taller debe tener un nombre no vació.");
            if (dto.Costo<=0)
                res.Errores.Add("Costo del taller inválido. \n El taller debe tener un costo mayor a 0 pesos.");
            if (dto.HoraInicio<=11||dto.HoraInicio>=23)
                res.Errores.Add("Hora de inicio del taller inválido. \n El taller debe tener una hora de inicio mayor que las 11 am..");
            if (dto.HoraFin <= 11 || dto.HoraFin >= 23)
                res.Errores.Add("Hora de cierre del taller inválido. \n El taller debe tener una hora de cierre mayor a 11 am y menor que 23 pm.");
            if (dto.HoraInicio > dto.HoraFin)
                res.Errores.Add("Horas de entre semana del taller inválido. \n La hora de inicio debe ser antes que la hora de cierre del taller.");
            
            if (dto.HoraInicioSabado < 5 || dto.HoraInicioSabado > 12)
                res.Errores.Add("Hora de inicio de fin de semana del taller inválido. \n El taller debe tener una hora de inicio de fin de semana entre 5am y 11.");
            if (dto.HoraFinSabado < 6 || dto.HoraFinSabado > 13)
                res.Errores.Add("Hora de cierre de fin de semana del taller inválido. \n El taller debe tener una hora de cierre entre 6am y 13pm");
            if (dto.HoraInicioSabado > dto.HoraFinSabado)
                res.Errores.Add("Horas de fin de semana del taller inválido. \n La hora de inicio debe ser antes que la hora de cierre del taller.");
            if (dto.Cupo < 5)
                res.Errores.Add("El taller debe tener minimo 5 cupos iniciales disponibles.");
             return res;
        }
    }
}
