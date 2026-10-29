namespace SemillitasMobileApp.Viewmodels;

// Una "fila" es el modelo ya preparado para mostrarse: texto formateado, colores (Tono), etc.
// Así la vista solo enlaza propiedades y no calcula nada.

public class Chip
{
    public string Texto { get; set; } = "";
}

public class TallerFila   // Disponibilidad
{
    public Taller Taller { get; set; } = new();
    public string Nombre { get; set; } = "";
    public string Meta { get; set; } = "";
    public string Horario { get; set; } = "";
    public string Ocupacion { get; set; } = "";   // "9/15"
    public double Porcentaje { get; set; }        // 0 a 1
    public string Badge { get; set; } = "";
    public string Tono { get; set; } = "Verde";   // Verde | Ambar | Rojo
    public bool TieneCupo { get; set; }
}

public class TallerAdminFila   // Lista de talleres
{
    public Taller Taller { get; set; } = new();
    public string Nombre { get; set; } = "";
    public string Categoria { get; set; } = "";
    public string Instructor { get; set; } = "";
    public string Horario { get; set; } = "";
    public string Cupo { get; set; } = "";
    public string Costo { get; set; } = "";
}

public class TallerInscripcionFila   // Pantalla de inscripción
{
    public Taller Taller { get; set; } = new();
    public string Nombre { get; set; } = "";
    public string Horario { get; set; } = "";
    public string Costo { get; set; } = "";
    public string Badge { get; set; } = "";
    public string Tono { get; set; } = "Verde";
    public string Accion { get; set; } = "Disponible";   // Disponible | Agregado | Choque | Lleno
}

public class ResumenFila
{
    public Taller Taller { get; set; } = new();
    public string Nombre { get; set; } = "";
    public string Horario { get; set; } = "";
    public string Precio { get; set; } = "";
}

public class CobroFila
{
    public Inscripcion Inscripcion { get; set; } = new();
    public string Tutor { get; set; } = "";
    public string Alumno { get; set; } = "";
    public string Taller { get; set; } = "";
    public string Costo { get; set; } = "";
    public string Estado { get; set; } = "";   // Debe | Al corriente
    public string Tono { get; set; } = "Verde";
    public bool PuedeMarcarPago { get; set; }
}

public class AlumnoFila
{
    public Alumno Alumno { get; set; } = new();
    public string NombreCompleto { get; set; } = "";
    public string Tutor { get; set; } = "";
    public string Telefono { get; set; } = "";
    public int CantidadTalleres { get; set; }
    public string Estado { get; set; } = "";
    public string Tono { get; set; } = "Verde";
}

public class InscripcionAlumnoFila   // Detalle de alumno
{
    public Inscripcion Inscripcion { get; set; } = new();
    public string Taller { get; set; } = "";
    public string Periodo { get; set; } = "";
    public string Costo { get; set; } = "";
    public string Estado { get; set; } = "";
    public string Tono { get; set; } = "Verde";
    public bool PuedeMarcarPago { get; set; }
}

public class InscritoFila   // Detalle de taller
{
    public string NombreCompleto { get; set; } = "";
    public string Tutor { get; set; } = "";
    public string Telefono { get; set; } = "";
    public string Estado { get; set; } = "";
    public string Tono { get; set; } = "Verde";
}

public class EventoFila   // Agenda
{
    public string Nombre { get; set; } = "";
    public string Detalle { get; set; } = "";
    public string Categoria { get; set; } = "";
}

public class FranjaFila
{
    public string Hora { get; set; } = "";
    public List<EventoFila> Talleres { get; set; } = new();
}

public class PeriodoFila
{
    public Periodo Periodo { get; set; } = new();
    public string MesInicio { get; set; } = "";
    public string MesFin { get; set; } = "";
    public int CantidadTalleres { get; set; }
}
