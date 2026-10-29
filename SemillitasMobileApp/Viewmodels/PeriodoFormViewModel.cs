using SemillasCore.DTOs;
using SemillasCore.Validators;
using SemillitasMobileApp.Commands;
using SemillitasMobileApp.Services;

namespace SemillitasMobileApp.Viewmodels;

// Nuevo período (sin parámetro) o editar (GoToAsync con { "Periodo": periodo }).
// CAMBIO: este archivo estaba comentado; ahora los meses son números 1–12 (como los pide el DTO)
// y se agrega el año.
public class PeriodoFormViewModel : ViewModelBase, IQueryAttributable
{
    private readonly PeriodosService _periodos = new();
    private PeriodoDTO? _periodo;

    public PeriodoFormViewModel()
    {
        CancelarCommand = new AsyncRelayCommand(Regresar);
        GuardarCommand = new AsyncRelayCommand(() => Ejecutar(Guardar));
    }

    public AsyncRelayCommand CancelarCommand { get; }
    public AsyncRelayCommand GuardarCommand { get; }

    private string _titulo = "Nuevo período";
    public string Titulo { get => _titulo; set => SetField(ref _titulo, value); }

    public List<int> Meses { get; } = Enumerable.Range(1, 12).ToList();

    private int? _mesInicio;
    public int? MesInicio { get => _mesInicio; set => SetField(ref _mesInicio, value); }

    private int? _mesFin;
    public int? MesFin { get => _mesFin; set => SetField(ref _mesFin, value); }

    private string _anio = "";
    public string Anio { get => _anio; set => SetField(ref _anio, value); }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("Periodo", out var valor) && valor is PeriodoDTO p)
        {
            _periodo = p;
            Titulo = "Editar período";
            MesInicio = p.MesInicio;
            MesFin = p.MesFin;
            Anio = p.Año.ToString();
        }
    }

    private async Task Guardar()
    {
        LimpiarErrores();

        // Si no es número se manda 0 y el validator compartido lo marca como inválido
        _ = int.TryParse(Anio, out var anio);
        var inicio = MesInicio ?? 0;
        var fin = MesFin ?? 0;

        if (_periodo is null)
        {
            var nuevo = new CreatePeriodoDTO { MesInicio = inicio, MesFin = fin, Año = anio };
            if (!EsValido(new CreatePeriodoValidator().IsValid(nuevo))) return;
            await _periodos.CrearAsync(nuevo);
        }
        else
        {
            var edicion = new EditPeriodoDTO { Id = _periodo.Id, MesInicio = inicio, MesFin = fin, Año = anio };
            if (!EsValido(new EditPeriodoValidator().IsValid(edicion))) return;
            await _periodos.ActualizarAsync(edicion);
        }

        await Regresar();
    }
}
