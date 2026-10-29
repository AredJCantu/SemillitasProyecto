using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SemillitasMobileApp.Models;
using SemillitasMobileApp.Services;

namespace SemillitasMobileApp.Viewmodels;

// Nuevo período (sin parámetro) o editar (GoToAsync con { "Periodo": periodo })
public partial class PeriodoFormViewModel : BaseViewModel, IQueryAttributable
{
    private readonly PeriodosService _periodos = new();
    private Periodo? _periodo;

    [ObservableProperty] private string titulo = "Nuevo período";
    [ObservableProperty] private List<string> meses = new()
    {
        "Enero", "Febrero", "Marzo", "Abril", "Mayo", "Junio",
        "Julio", "Agosto", "Septiembre", "Octubre", "Noviembre", "Diciembre"
    };
    [ObservableProperty] private string? mesInicio;
    [ObservableProperty] private string? mesFin;
    [ObservableProperty] private bool tieneErrorPeriodo;
    [ObservableProperty] private string errorPeriodo = "";

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("Periodo", out var valor) && valor is Periodo p)
        {
            _periodo = p;
            Titulo = "Editar período";
            MesInicio = p.MesInicio;
            MesFin = p.MesFin;
        }
    }

    [RelayCommand]
    private Task Cancelar() => Regresar();

    [RelayCommand]
    private Task Guardar() => Ejecutar(async () =>
    {
        TieneErrorPeriodo = false;

        if (MesInicio is null || MesFin is null)
        {
            ErrorPeriodo = "Elige el mes de inicio y el de fin.";
            TieneErrorPeriodo = true;
            return;
        }

        var periodo = new Periodo { Id = _periodo?.Id ?? 0, MesInicio = MesInicio, MesFin = MesFin };

        try
        {
            if (_periodo is null) await _periodos.CrearAsync(periodo);
            else await _periodos.ActualizarAsync(periodo);
        }
        catch (ApiException ex) when (ex.Errores.Count > 0)
        {
            ErrorPeriodo = string.Join(" ", ex.Errores.Values.SelectMany(x => x));
            TieneErrorPeriodo = true;
            return;
        }

        await Regresar();
    });
}
