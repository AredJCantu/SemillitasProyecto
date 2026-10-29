using System.Globalization;
using System.Text;
using SemillitasMobileApp.Models;

namespace SemillitasMobileApp.Helpers;

// Convierte datos del API a texto para las vistas
public static class Formato
{
    public static string Hora(int h)
    {
        var h12 = h % 12 == 0 ? 12 : h % 12;
        return $"{h12} {(h < 12 ? "am" : "pm")}";
    }

    // 15, 17 -> "3–5 pm"; 11, 13 -> "11 am–1 pm"
    public static string Rango(int inicio, int fin)
    {
        var a = inicio % 12 == 0 ? 12 : inicio % 12;
        var b = fin % 12 == 0 ? 12 : fin % 12;
        var sa = inicio < 12 ? "am" : "pm";
        var sb = fin < 12 ? "am" : "pm";
        return sa == sb ? $"{a}–{b} {sb}" : $"{a} {sa}–{b} {sb}";
    }

    public static string Rango(Taller t) => Rango(t.HoraInicio, t.HoraFin);

    public static string Dinero(decimal monto) => "$" + monto.ToString("#,##0.##", CultureInfo.InvariantCulture);

    // "arte" -> "Arte"
    public static string Categoria(string c) =>
        string.IsNullOrEmpty(c) ? "" : char.ToUpper(c[0]) + c[1..].ToLower();

    // "Tecnología" -> "Tecnologia" (clave que usan los triggers de color de la Agenda)
    public static string CategoriaClave(string c)
    {
        var s = Categoria(c).Normalize(NormalizationForm.FormD);
        return new string(s.Where(ch => CharUnicodeInfo.GetUnicodeCategory(ch) != UnicodeCategory.NonSpacingMark).ToArray())
            .Normalize(NormalizationForm.FormC);
    }

    // Texto y color según los lugares que quedan. Verde: hay de sobra. Ámbar: quedan pocos
    // (3 o menos, o el 20% del cupo). Rojo: lleno.
    public static (string Badge, string Tono) Lugares(int cupo, int inscritos)
    {
        var libres = Math.Max(cupo - inscritos, 0);
        if (libres == 0) return ("Lleno", "Rojo");
        return ($"Quedan {libres}", libres <= Math.Max(3, cupo * 0.2) ? "Ambar" : "Verde");
    }

    public static string NombreCompleto(Alumno a) => $"{a.Nombre} {a.Apellido}".Trim();

    public static string Tutor(Alumno a) => $"{a.NombreTutor} {a.ApellidoTutor}".Trim();

    public static bool Contiene(string texto, string busqueda) =>
        texto.Contains(busqueda, StringComparison.CurrentCultureIgnoreCase);
}
