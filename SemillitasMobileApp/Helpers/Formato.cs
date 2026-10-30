using System.Globalization;
using SemillasCore.DTOs;
using SemillitasProyecto.Commons;

namespace SemillitasMobileApp.Helpers;

// Convierte datos del API a texto para las vistas
public static class Formato
{
    // 15 y 17 -> "3–5 pm"; 11 y 13 -> "11 am–1 pm"
    public static string Rango(int inicio, int fin)
    {
        var a = inicio % 12 == 0 ? 12 : inicio % 12;
        var b = fin % 12 == 0 ? 12 : fin % 12;
        var sa = inicio < 12 ? "am" : "pm";
        var sb = fin < 12 ? "am" : "pm";
        return sa == sb ? $"{a}–{b} {sb}" : $"{a} {sa}–{b} {sb}";
    }

    public static string Dinero(decimal monto) => "$" + monto.ToString("#,##0.##", CultureInfo.InvariantCulture);

    public static string Categoria(Categorias c) => c switch
    {
        Categorias.arte => "Arte",
        Categorias.deportes => "Deportes",
        _ => "Tecnología"
    };

    // Cupo = lugares que quedan; el máximo es lo que queda más los inscritos
    public static int Maximo(TallerDTO t) => t.Cupo + t.CantidadAlumnos;

    // Verde: hay de sobra. Ambar: quedan 3 o menos (o el 20% del máximo). Rojo: lleno.
    public static (string Badge, string Tono) Lugares(int libres, int maximo)
    {
        if (libres <= 0) return ("Lleno", "Rojo");
        return ($"Quedan {libres}", libres <= Math.Max(3, maximo * 0.2) ? "Ambar" : "Verde");
    }

    public static string NombreCompleto(AlumnoDTO a) => $"{a.Nombre} {a.Apellido}".Trim();

    public static bool Contiene(string? texto, string busqueda) =>
        (texto ?? "").Contains(busqueda.Trim(), StringComparison.CurrentCultureIgnoreCase);
}
