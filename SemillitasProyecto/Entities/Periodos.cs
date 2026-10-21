using System;
using System.Collections.Generic;

namespace SemillitasProyecto.Entities;

public partial class Periodos
{
    public int Id { get; set; }

    public string MesInicio { get; set; } = null!;

    public string MesFin { get; set; } = null!;

    public virtual ICollection<Talleres> Talleres { get; set; } = new List<Talleres>();
}
