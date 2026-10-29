using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SemillasCore.Validators
{
    public class ValidatorClass
    {
        public List<string> Errores { get; set; } = new();
        public bool EsValido { get => Errores.Count == 0; }
    }
}
