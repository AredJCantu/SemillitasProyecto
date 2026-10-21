using System;
using System.Collections.Generic;

namespace SemillitasProyecto.Entities;

public partial class Talleres
{
    public int Id { get; set; }

    public string Nombre { get; set; } = null!;

    public decimal Costo { get; set; }

    public int HoraInicio { get; set; }

    public int HoraFin { get; set; }

    public int Cupo { get; set; }

    public string NombreInstructor { get; set; } = null!;

    public string Categoria { get; set; } = null!;

    public int IdPeriodo { get; set; }

    public virtual Periodos IdPeriodoNavigation { get; set; } = null!;

    public virtual ICollection<Inscripciones> Inscripciones { get; set; } = new List<Inscripciones>();
}
