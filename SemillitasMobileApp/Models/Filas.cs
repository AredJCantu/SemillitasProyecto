using SemillasCore.DTOs;
using SemillitasMobileApp.Helpers;

namespace SemillitasMobileApp.Models;

// Datos ya preparados para mostrarse (textos y colores); la vista solo enlaza

public class TallerFila
{
    public TallerFila(TallerDTO t)
    {
        var maximo = Formato.Maximo(t);
        (Badge, Tono) = Formato.Lugares(t.Cupo, maximo);

        Nombre = t.Nombre;
        Categoria = Formato.Categoria(t.Categoria);
        Meta = $"{Categoria} · {Formato.Dinero(t.Costo)}";
        Horario = $"{Formato.Rango(t.HoraInicio, t.HoraFin)} · Sábado {Formato.Rango(t.HoraInicioSabado, t.HoraFinSabado)}";
        Ocupacion = $"{t.CantidadAlumnos}/{maximo} inscritos";
        Porcentaje = maximo == 0 ? 1 : Math.Min((double)t.CantidadAlumnos / maximo, 1);
    }

    public string Nombre { get; }
    public string Categoria { get; }    // también decide el color en la Agenda
    public string Meta { get; }         // "Arte · $420"
    public string Horario { get; }
    public string Ocupacion { get; }    // "9/15 inscritos"
    public double Porcentaje { get; }   // 0 a 1 (ProgressBar)
    public string Badge { get; }
    public string Tono { get; }         // Verde | Ambar | Rojo
}

public class AlumnoFila
{
    public string NombreCompleto { get; set; } = "";
    public string Tutor { get; set; } = "";
    public string Telefono { get; set; } = "";
    public int CantidadTalleres { get; set; }
    public string Estado { get; set; } = "";
    public string Tono { get; set; } = "Verde";
}

public class CobroFila
{
    public string Alumno { get; set; } = "";
    public string Tutor { get; set; } = "";
    public string Taller { get; set; } = "";
    public string Costo { get; set; } = "";
    public string Estado { get; set; } = "";
    public string Tono { get; set; } = "Verde";
}

// Agenda: talleres con el mismo horario entre semana
public class FranjaFila
{
    public string Hora { get; set; } = "";
    public List<TallerFila> Talleres { get; set; } = new();
}
