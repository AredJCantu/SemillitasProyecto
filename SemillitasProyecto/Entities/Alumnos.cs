using System;
using System.Collections.Generic;

namespace SemillitasProyecto.Entities;

public partial class Alumnos
{
    public int Id { get; set; }

    public string Nombre { get; set; } = null!;

    public string Apellido { get; set; } = null!;

    public string NombreTutor { get; set; } = null!;

    public string Telefono { get; set; } = null!;

    public virtual ICollection<Inscripciones> Inscripciones { get; set; } = new List<Inscripciones>();
}
