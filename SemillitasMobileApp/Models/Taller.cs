namespace SemillitasMobileApp.Models;

// Mismos campos que la entidad Talleres del API
public class Taller
{
    public int Id { get; set; }
    public string Nombre { get; set; } = "";
    public decimal Costo { get; set; }
    public int HoraInicio { get; set; }   // 15 = 3 pm
    public int HoraFin { get; set; }
    public int Cupo { get; set; }
    public string NombreInstructor { get; set; } = "";
    public string Categoria { get; set; } = "";
    public int IdPeriodo { get; set; }
}
