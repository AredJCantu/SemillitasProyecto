using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SemillitasMobileApp.Helpers;
using SemillitasMobileApp.Models;
using SemillitasMobileApp.Services;

namespace SemillitasMobileApp.Viewmodels;

// Se abre con GoToAsync(Rutas.TallerDetalle, { "Taller": taller })
public partial class TallerDetalleViewModel : BaseViewModel, IQueryAttributable
{
    private readonly TalleresService _talleres = new();
    private readonly AlumnosService _alumnos = new();
    private readonly PeriodosService _periodos = new();
    private readonly InscripcionesService _inscripciones = new();

    private Taller? _taller;

    [ObservableProperty] private string nombre = "";
    [ObservableProperty] private string categoria = "";
    [ObservableProperty] private string instructor = "";
    [ObservableProperty] private string horario = "";
    [ObservableProperty] private string periodo = "";
    [ObservableProperty] private string costo = "";
    [ObservableProperty] private string ocupacion = "";
    [ObservableProperty] private List<InscritoFila> inscritos = new();

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("Taller", out var valor) && valor is Taller t)
        {
            _taller = t;
            Mostrar(t);
        }
    }

    private void Mostrar(Taller t)
    {
        Nombre = t.Nombre;
        Categoria = Formato.Categoria(t.Categoria);
        Instructor = t.NombreInstructor;
        Horario = Formato.Rango(t);
        Costo = Formato.Dinero(t.Costo);
    }

    [RelayCommand]
    private Task Cargar() => Ejecutar(async () =>
    {
        if (_taller is null) return;

        // Se vuelve a pedir el taller por si se editó
        var actual = (await _talleres.ObtenerAsync()).FirstOrDefault(t => t.Id == _taller.Id);
        if (actual is not null) { _taller = actual; Mostrar(actual); }

        Periodo = (await _periodos.ObtenerAsync()).FirstOrDefault(p => p.Id == _taller.IdPeriodo)?.ToString() ?? "";

        var alumnos = await _alumnos.ObtenerAsync();
        var suyas = (await _inscripciones.ObtenerAsync()).Where(i => i.IdTaller == _taller.Id).ToList();

        Inscritos = (from i in suyas
                     join a in alumnos on i.IdAlumno equals a.Id
                     select new InscritoFila
                     {
                         NombreCompleto = Formato.NombreCompleto(a),
                         Tutor = Formato.Tutor(a),
                         Telefono = a.Telefono,
                         Estado = i.Debe ? "Debe" : "Al corriente",
                         Tono = i.Debe ? "Rojo" : "Verde"
                     }).ToList();

        Ocupacion = $"{suyas.Count}/{_taller.Cupo}";
    });

    [RelayCommand]
    private Task Volver() => Regresar();

    [RelayCommand]
    private Task EditarTaller() =>
        Shell.Current.GoToAsync(Rutas.TallerForm, new Dictionary<string, object> { ["Taller"] = _taller! });
}
