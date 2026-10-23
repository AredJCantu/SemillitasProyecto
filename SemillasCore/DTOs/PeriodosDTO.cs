using SemillasCore.Validators;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SemillasCore.DTOs
{
    public class CreatePeriodoDTO {
        [Required(ErrorMessage = "Favor de ingresar un valor.")]
        [RegularExpression(@"^([0-9])|(1[0-2])$",ErrorMessage = "Mes ingresado inválido.")]
        public int MesInicio { get; set; }
        [Required(ErrorMessage = "Favor de ingresar un valor.")]
        [RegularExpression(@"^([0-9])|(1[0-2])$", ErrorMessage = "Mes ingresado inválido.")]
        public int MesFin { get; set; }
        [Required(ErrorMessage = "Favor de ingresar un valor.")]
        [RegularExpression(@"^(200[0-9])|(201[0-9])|(202[0-9])$", ErrorMessage = "Año ingresado inválido.")]
        public int Año { get; set; }
    }
    public class PeriodoDTO : CreatePeriodoDTO {
        public int Id { get; set; }
        public override string ToString()
        {
            return $"{MesInicio}-{MesFin}-{Año}";
        }
    }
    //[NoMesIgualesValidator(ErrorMessage = "Ya existe un periodo con esos meses y años-")]
    public class EditPeriodoDTO:CreatePeriodoDTO
    {
    }
    
}
