using SemillasCore.DTOs;
using SemillasCore.Validators;
using SemillitasMobileApp.Commands;
using SemillitasMobileApp.Helpers;
using SemillitasMobileApp.Services;

namespace SemillitasMobileApp.Viewmodels;

// CAMBIO: sin toolkit; la fila muestra meses en número y el año; se agrega EliminarPeriodoCommand
public class PeriodosViewModel : ViewModelBase
{
    private readonly PeriodosService _periodos = new();
    private readonly TalleresService _talleres = new();

    public PeriodosViewModel()
    {
        CargarCommand = new AsyncRelayCommand(() => Ejecutar(Cargar));
        NuevoPeriodoCommand = new AsyncRelayCommand(() => Shell.Current.GoToAsync(Rutas.PeriodoForm));
        EditarPeriodoCommand = new AsyncRelayCommand<PeriodoFila>(fila =>
            Shell.Current.GoToAsync(Rutas.PeriodoForm, new Dictionary<string, object> { ["Periodo"] = fila.Periodo }));
        EliminarPeriodoCommand = new AsyncRelayCommand<PeriodoFila>(Eliminar);
    }

    public AsyncRelayCommand CargarCommand { get; }
    public AsyncRelayCommand NuevoPeriodoCommand { get; }
    public AsyncRelayCommand<PeriodoFila> EditarPeriodoCommand { get; }
    public AsyncRelayCommand<PeriodoFila> EliminarPeriodoCommand { get; }

    private List<PeriodoFila> _periodosLista = new();
    public List<PeriodoFila> Periodos { get => _periodosLista; set => SetField(ref _periodosLista, value); }

    private async Task Cargar()
    {
        var lista = await _periodos.GetAll();
        var talleres = await _talleres.GetAll();

        Periodos = lista.OrderBy(p => p.Id).Select(p => new PeriodoFila
        {
            Periodo = p,
            MesInicio = p.MesInicio.ToString(),
            MesFin = p.MesFin.ToString(),
            Anio = p.Año.ToString(),
            CantidadTalleres = talleres.Count(t => t.IdPeriodo == p.Id)
        }).ToList();
    }

    private async Task Eliminar(PeriodoFila fila)
    {
        if (!await Confirmar("Eliminar período", $"¿Eliminar el período \"{fila.Periodo}\"?", "Eliminar")) return;

        await Ejecutar(async () =>
        {
            var errores = new ValidatorClass();
            if (!await _periodos.Delete(fila.Periodo.Id, errores))
            {
                await Avisar("No se pudo eliminar", string.Join("\n", errores.Errores));
                return;
            }
            await Cargar();
        });
    }
}
