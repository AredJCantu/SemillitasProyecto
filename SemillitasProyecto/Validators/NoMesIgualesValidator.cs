using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SemillasCore.Validators
{
    [AttributeUsage(AttributeTargets.Class)]
    public class NoMesIgualesValidator:ValidationAttribute
    {
    }
}
