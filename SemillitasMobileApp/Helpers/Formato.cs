using System.Globalization;
using SemillasCore.DTOs;
using SemillitasProyecto.Commons;

namespace SemillitasMobileApp.Helpers;

// Convierte datos del API a texto para las vistas
public static class Formato
{
    // CAMBIO: horas sueltas y rangos como texto (13 -> "1 pm", 15 y 17 -> "3–5 pm")
    public static string Hora(int h)
    {
        var h12 = h % 12 == 0 ? 12 : h % 12;
        return $"{h12} {(h < 12 ? "am" : "pm")}";
    }

    public static string Rango(int inicio, int fin)
    {
        var a = inicio % 12 == 0 ? 12 : inicio % 12;
        var b = fin % 12 == 0 ? 12 : fin % 12;
        var sa = inicio < 12 ? "am" : "pm";
        var sb = fin < 12 ? "am" : "pm";
        return sa == sb ? $"{a}–{b} {sb}" : $"{a} {sa}–{b} {sb}";
    }

    // CAMBIO: el horario es la unión de hora inicial y final (lo que pidió el cliente)
    public static string Horario(TallerDTO t) => Rango(t.HoraInicio, t.HoraFin);

    public static string Dinero(decimal monto) => "$" + monto.ToString("#,##0.##", CultureInfo.InvariantCulture);

    // CAMBIO: la categoría ahora llega como enum (Categorias) en vez de texto
    public static string Categoria(Categorias c) => c switch
    {
        Categorias.arte => "Arte",
        Categorias.deportes => "Deportes",
        _ => "Tecnología"
    };

    // Clave sin acento que usan los triggers de color de la Agenda
    public static string CategoriaClave(Categorias c) => c switch
    {
        Categorias.arte => "Arte",
        Categorias.deportes => "Deportes",
        _ => "Tecnologia"
    };

    // CAMBIO: Cupo del API = lugares que quedan. El máximo es lo que quedan más los ya inscritos.
    public static int Maximo(TallerDTO t) => t.Cupo + t.CantidadAlumnos;

    // Texto y color según los lugares libres. Verde: hay de sobra. Ámbar: quedan pocos
    // (3 o menos, o el 20% del máximo). Rojo: lleno.
    public static (string Badge, string Tono) Lugares(int libres, int maximo)
    {
        if (libres <= 0) return ("Lleno", "Rojo");
        return ($"Quedan {libres}", libres <= Math.Max(3, maximo * 0.2) ? "Ambar" : "Verde");
    }

    public static string NombreCompleto(AlumnoDTO a) => $"{a.Nombre} {a.Apellido}".Trim();

    public static bool Contiene(string texto, string busqueda) =>
        texto.Contains(busqueda, StringComparison.CurrentCultureIgnoreCase);
}
