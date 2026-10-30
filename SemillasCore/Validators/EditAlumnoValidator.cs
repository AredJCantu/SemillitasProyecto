using SemillasCore.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SemillasCore.Validators
{
    public class EditAlumnoValidator
    {
        public virtual ValidatorClass IsValid(EditAlumnoDTO dto)
        {
            ValidatorClass res = new();
            if (string.IsNullOrWhiteSpace(dto.Nombre))
                res.Errores.Add("Nombre inválido. \n El nombre del alumno no debe ir vacio.");
            if (string.IsNullOrWhiteSpace(dto.Apellido))
                res.Errores.Add("Apellido inválido. \n El apellido del alumno no debe ir vacio.");
            if (string.IsNullOrWhiteSpace(dto.NombreTutor))
                res.Errores.Add("Nombre del tutor inválido. \n Un alumno debe tener un tutor.");
            if (string.IsNullOrWhiteSpace(dto.Telefono) && dto.Telefono.Length < 12)
                res.Errores.Add("Telefono inválido. \n El Telefono del tutor del alumno no debe ir vacio y debe tener 12 digitos maximo.");
            return res;
        }
    }
}
