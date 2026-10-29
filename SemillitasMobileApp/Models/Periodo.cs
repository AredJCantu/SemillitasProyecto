namespace SemillitasMobileApp.Models;

public class Periodo
{
    public int Id { get; set; }
    public string MesInicio { get; set; } = "";
    public string MesFin { get; set; } = "";

    // Los Picker muestran el ToString() del objeto
    public override string ToString() => $"{MesInicio}–{MesFin}";
}
