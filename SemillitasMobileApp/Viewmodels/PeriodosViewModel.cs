using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SemillitasMobileApp.Helpers;
using SemillitasMobileApp.Services;

namespace SemillitasMobileApp.Viewmodels;

public partial class PeriodosViewModel : BaseViewModel
{
    private readonly PeriodosService _periodos = new();
    private readonly TalleresService _talleres = new();

    [ObservableProperty] private List<PeriodoFila> periodos = new();

    [RelayCommand]
    private Task Cargar() => Ejecutar(async () =>
    {
        var lista = await _periodos.ObtenerAsync();
        var talleres = await _talleres.ObtenerAsync();

        Periodos = lista.OrderBy(p => p.Id).Select(p => new PeriodoFila
        {
            Periodo = p,
            MesInicio = p.MesInicio,
            MesFin = p.MesFin,
            CantidadTalleres = talleres.Count(t => t.IdPeriodo == p.Id)
        }).ToList();
    });

    [RelayCommand]
    private Task NuevoPeriodo() => Shell.Current.GoToAsync(Rutas.PeriodoForm);

    [RelayCommand]
    private Task EditarPeriodo(PeriodoFila fila) =>
        Shell.Current.GoToAsync(Rutas.PeriodoForm, new Dictionary<string, object> { ["Periodo"] = fila.Periodo });
}
