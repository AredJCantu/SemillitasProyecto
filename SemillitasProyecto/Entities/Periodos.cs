using System;
using System.Collections.Generic;

namespace SemillitasProyecto.Entities;

public partial class Periodos
{
    public int Id { get; set; }

    public int MesInicio { get; set; } 

    public int MesFin { get; set; }
    public int Año { get; set; }

    public virtual ICollection<Talleres> Talleres { get; set; } = new List<Talleres>();
}
