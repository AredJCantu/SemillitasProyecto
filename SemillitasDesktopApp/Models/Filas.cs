using SemillasCore.DTOs;
using SemillitasDesktopApp.Helpers;

namespace SemillitasDesktopApp.Models;

// Una "fila" es el dato ya preparado para mostrarse (textos y colores).
// Así la vista solo enlaza propiedades y no calcula nada.

// Un taller. La usan Disponibilidad, Agenda, el catálogo y la inscripción.
public class TallerFila
{
    public TallerFila(TallerDTO t)
    {
        var maximo = Formato.Maximo(t);
        (Badge, Tono) = Formato.Lugares(t.Cupo, maximo);

        Taller = t;
        Nombre = t.Nombre;
        Categoria = Formato.Categoria(t.Categoria);
        Horario = Formato.Rango(t.HoraInicio, t.HoraFin);
        HorarioSabado = Formato.Rango(t.HoraInicioSabado, t.HoraFinSabado);
        Costo = Formato.Dinero(t.Costo);
        Cupo = t.Cupo;
        Ocupacion = $"{t.CantidadAlumnos}/{maximo}";
        Porcentaje = maximo == 0 ? 100 : Math.Min(100.0 * t.CantidadAlumnos / maximo, 100);
        TieneCupo = t.Cupo > 0;
    }

    public TallerDTO Taller { get; }
    public string Nombre { get; }
    public string Categoria { get; }
    public string Horario { get; }
    public string HorarioSabado { get; }
    public string Costo { get; }
    public int Cupo { get; }
    public string Ocupacion { get; }     // "9/15" = inscritos/máximo
    public double Porcentaje { get; }    // 0 a 100 (ProgressBar)
    public string Badge { get; }
    public string Tono { get; }          // Verde | Ambar | Rojo
    public bool TieneCupo { get; }

    // Solo en la pantalla de inscripción: Disponible | Agregado | Choque | Lleno
    public string Accion { get; set; } = "Disponible";
}

// Una inscripción con sus datos de alumno y taller. La usan Cobranza y los detalles.
public class InscripcionFila
{
    public InscripcionesDTO Inscripcion { get; set; } = new();
    public string Alumno { get; set; } = "";
    public string Tutor { get; set; } = "";
    public string Telefono { get; set; } = "";
    public string Taller { get; set; } = "";
    public string Periodo { get; set; } = "";
    public string Costo { get; set; } = "";
    public string Estado { get; set; } = "";   // Debe | Al corriente
    public string Tono { get; set; } = "Verde";
    public bool PuedeMarcarPago { get; set; }

    // Junta la inscripción con su alumno y su taller (Debe es bool?: solo true cuenta como adeudo)
    public static InscripcionFila Crear(InscripcionesDTO i, AlumnoDTO a, TallerDTO t, string periodo = "")
    {
        var debe = i.Debe == true;
        return new InscripcionFila
        {
            Inscripcion = i,
            Alumno = Formato.NombreCompleto(a),
            Tutor = a.NombreTutor,
            Telefono = a.Telefono,
            Taller = t.Nombre,
            Periodo = periodo,
            Costo = Formato.Dinero(t.Costo),
            Estado = debe ? "Debe" : "Al corriente",
            Tono = debe ? "Rojo" : "Verde",
            PuedeMarcarPago = debe
        };
    }
}

public class AlumnoFila
{
    public AlumnoDTO Alumno { get; set; } = new();
    public string NombreCompleto { get; set; } = "";
    public string Tutor { get; set; } = "";
    public string Telefono { get; set; } = "";
    public int CantidadTalleres { get; set; }
    public string Estado { get; set; } = "";
    public string Tono { get; set; } = "Verde";
}

public class PeriodoFila
{
    public PeriodoDTO Periodo { get; set; } = new();
    public int MesInicio { get; set; }
    public int MesFin { get; set; }
    public int Anio { get; set; }
    public int CantidadTalleres { get; set; }
}

// Agenda: talleres con el mismo horario
public class FranjaFila
{
    public string Hora { get; set; } = "";
    public List<TallerFila> Talleres { get; set; } = new();
}
