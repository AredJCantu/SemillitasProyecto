using System;
using System.Collections.Generic;

namespace SemillitasProyecto.Entities;

public partial class Inscripciones
{
    public int Id { get; set; }

    public int IdAlumno { get; set; }

    public int IdTaller { get; set; }

    public bool? Debe { get; set; }

    public virtual Alumnos IdAlumnoNavigation { get; set; } = null!;

    public virtual Talleres IdTallerNavigation { get; set; } = null!;
}
