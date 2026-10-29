using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SemillitasMobileApp.Helpers;
using SemillitasMobileApp.Models;
using SemillitasMobileApp.Services;

namespace SemillitasMobileApp.Viewmodels;

// Se abre con GoToAsync(Rutas.AlumnoDetalle, { "Alumno": alumno }). IQueryAttributable recibe ese parámetro.
public partial class AlumnoDetalleViewModel : BaseViewModel, IQueryAttributable
{
    private readonly AlumnosService _alumnos = new();
    private readonly TalleresService _talleres = new();
    private readonly PeriodosService _periodos = new();
    private readonly InscripcionesService _inscripciones = new();

    private Alumno? _alumno;

    [ObservableProperty] private string nombreCompleto = "";
    [ObservableProperty] private string tutor = "";
    [ObservableProperty] private string telefono = "";
    [ObservableProperty] private string totalAdeudo = "$0";
    [ObservableProperty] private List<InscripcionAlumnoFila> inscripciones = new();

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("Alumno", out var valor) && valor is Alumno a)
        {
            _alumno = a;
            Mostrar(a);
        }
    }

    private void Mostrar(Alumno a)
    {
        NombreCompleto = Formato.NombreCompleto(a);
        Tutor = Formato.Tutor(a);
        Telefono = a.Telefono;
    }

    [RelayCommand]
    private Task Cargar() => Ejecutar(async () =>
    {
        if (_alumno is null) return;

        // Se vuelve a pedir el alumno por si se editó en el formulario
        var actual = (await _alumnos.ObtenerAsync()).FirstOrDefault(a => a.Id == _alumno.Id);
        if (actual is not null) { _alumno = actual; Mostrar(actual); }

        var talleres = await _talleres.ObtenerAsync();
        var periodos = await _periodos.ObtenerAsync();
        var suyas = (await _inscripciones.ObtenerAsync()).Where(i => i.IdAlumno == _alumno.Id).ToList();

        var filas = (from i in suyas
                     join t in talleres on i.IdTaller equals t.Id
                     select new { i, t }).ToList();

        Inscripciones = filas.Select(x => new InscripcionAlumnoFila
        {
            Inscripcion = x.i,
            Taller = x.t.Nombre,
            Periodo = periodos.FirstOrDefault(p => p.Id == x.t.IdPeriodo)?.ToString() ?? "",
            Costo = Formato.Dinero(x.t.Costo),
            Estado = x.i.Debe ? "Debe" : "Al corriente",
            Tono = x.i.Debe ? "Rojo" : "Verde",
            PuedeMarcarPago = x.i.Debe
        }).ToList();

        TotalAdeudo = Formato.Dinero(filas.Where(x => x.i.Debe).Sum(x => x.t.Costo));
    });

    [RelayCommand]
    private Task Volver() => Regresar();

    [RelayCommand]
    private Task EditarAlumno() =>
        Shell.Current.GoToAsync(Rutas.AlumnoForm, new Dictionary<string, object> { ["Alumno"] = _alumno! });

    [RelayCommand]
    private async Task MarcarPagado(InscripcionAlumnoFila fila)
    {
        if (!await Confirmar("Marcar pagado", $"¿Marcar \"{fila.Taller}\" como pagado?", "Sí, pagó")) return;

        await Ejecutar(async () => await _inscripciones.MarcarPagadaAsync(fila.Inscripcion));
        await CargarCommand.ExecuteAsync(null);   // refresca la lista y el adeudo
    }
}
