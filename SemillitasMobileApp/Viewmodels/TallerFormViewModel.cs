using System.Globalization;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SemillitasMobileApp.Helpers;
using SemillitasMobileApp.Models;
using SemillitasMobileApp.Services;

namespace SemillitasMobileApp.Viewmodels;

// Nuevo taller (sin parámetro) o editar (GoToAsync con { "Taller": taller })
public partial class TallerFormViewModel : BaseViewModel, IQueryAttributable
{
    private const int PrimeraHora = 6;   // 6 am
    private const int UltimaHora = 21;   // 9 pm

    private readonly TalleresService _talleres = new();
    private readonly PeriodosService _periodos = new();
    private Taller? _taller;

    [ObservableProperty] private string titulo = "Nuevo taller";
    [ObservableProperty] private bool esEdicion;
    [ObservableProperty] private string nombre = "";
    [ObservableProperty] private List<string> categorias = new() { "Arte", "Deportes", "Tecnología" };
    [ObservableProperty] private string? categoriaSeleccionada;
    [ObservableProperty] private string nombreInstructor = "";
    [ObservableProperty] private List<Periodo> periodos = new();
    [ObservableProperty] private Periodo? periodoSeleccionado;
    [ObservableProperty] private string cupoMaximo = "";
    [ObservableProperty] private string costo = "";
    [ObservableProperty] private List<string> horas =
        Enumerable.Range(PrimeraHora, UltimaHora - PrimeraHora + 1).Select(Formato.Hora).ToList();
    [ObservableProperty] private string? horaInicio;
    [ObservableProperty] private string? horaFin;
    [ObservableProperty] private bool tieneErrorNombre;
    [ObservableProperty] private string errorNombre = "";
    [ObservableProperty] private bool tieneErrorCosto;
    [ObservableProperty] private string errorCosto = "";

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("Taller", out var valor) && valor is Taller t)
        {
            _taller = t;
            EsEdicion = true;
            Titulo = "Editar taller";
            Nombre = t.Nombre;
            CategoriaSeleccionada = Formato.Categoria(t.Categoria);
            NombreInstructor = t.NombreInstructor;
            CupoMaximo = t.Cupo.ToString();
            Costo = t.Costo.ToString(CultureInfo.InvariantCulture);
            HoraInicio = Formato.Hora(t.HoraInicio);
            HoraFin = Formato.Hora(t.HoraFin);
        }
    }

    [RelayCommand]
    private Task Cargar() => Ejecutar(async () =>
    {
        Periodos = await _periodos.ObtenerAsync();
        PeriodoSeleccionado = Periodos.FirstOrDefault(p => p.Id == _taller?.IdPeriodo)
                              ?? PeriodoSeleccionado
                              ?? Periodos.OrderBy(p => p.Id).LastOrDefault();
    });

    [RelayCommand]
    private Task Cancelar() => Regresar();

    [RelayCommand]
    private Task Guardar() => Ejecutar(async () =>
    {
        TieneErrorNombre = TieneErrorCosto = false;

        // Solo se comprueba que lo escrito sea un número; las reglas de negocio las valida el API
        if (!decimal.TryParse(Costo, NumberStyles.Number, CultureInfo.InvariantCulture, out var monto))
        {
            ErrorCosto = "Escribe el costo como número (ej. 420 o 420.50)";
            TieneErrorCosto = true;
            return;
        }
        if (!int.TryParse(CupoMaximo, out var cupo) || PeriodoSeleccionado is null
            || CategoriaSeleccionada is null || HoraInicio is null || HoraFin is null)
        {
            await Avisar("Faltan datos", "Revisa categoría, período, cupo y horas.");
            return;
        }

        var taller = new Taller
        {
            Id = _taller?.Id ?? 0,
            Nombre = Nombre.Trim(),
            Categoria = CategoriaSeleccionada.ToLower(),   // el API guarda "arte", "deportes", "tecnología"
            NombreInstructor = NombreInstructor.Trim(),
            IdPeriodo = PeriodoSeleccionado.Id,
            Cupo = cupo,
            Costo = monto,
            HoraInicio = PrimeraHora + Horas.IndexOf(HoraInicio),
            HoraFin = PrimeraHora + Horas.IndexOf(HoraFin)
        };

        try
        {
            if (_taller is null) await _talleres.CrearAsync(taller);
            else await _talleres.ActualizarAsync(taller);
        }
        catch (ApiException ex) when (ex.Errores.Count > 0)
        {
            if (ex.Errores.TryGetValue("Nombre", out var n)) { ErrorNombre = string.Join(" ", n); TieneErrorNombre = true; }
            if (ex.Errores.TryGetValue("Costo", out var c)) { ErrorCosto = string.Join(" ", c); TieneErrorCosto = true; }
            if (!TieneErrorNombre && !TieneErrorCosto) throw;
            return;
        }

        await Regresar();
    });

    [RelayCommand]
    private async Task Eliminar()
    {
        if (_taller is null) return;
        if (!await Confirmar("Eliminar taller", $"¿Eliminar \"{_taller.Nombre}\"?", "Eliminar")) return;

        await Ejecutar(async () =>
        {
            await _talleres.EliminarAsync(_taller.Id);
            await Regresar();
        });
    }
}
