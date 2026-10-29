using SemillasCore.DTOs;
using SemillitasMobileApp.Commands;
using SemillitasMobileApp.Helpers;
using SemillitasMobileApp.Services;

namespace SemillitasMobileApp.Viewmodels;

// Se abre con GoToAsync(Rutas.TallerDetalle, { "Taller": taller }).
// CAMBIO: sin toolkit y sin instructor; la ocupación es inscritos/máximo (máximo = Cupo + inscritos).
public class TallerDetalleViewModel : ViewModelBase, IQueryAttributable
{
    private readonly TalleresService _talleres = new();
    private readonly AlumnosService _alumnos = new();
    private readonly PeriodosService _periodos = new();
    private readonly InscripcionesService _inscripciones = new();

    private TallerDTO? _taller;

    public TallerDetalleViewModel()
    {
        CargarCommand = new AsyncRelayCommand(() => Ejecutar(Cargar));
        VolverCommand = new AsyncRelayCommand(Regresar);
        EditarTallerCommand = new AsyncRelayCommand(() =>
            Shell.Current.GoToAsync(Rutas.TallerForm, new Dictionary<string, object> { ["Taller"] = _taller! }));
    }

    public AsyncRelayCommand CargarCommand { get; }
    public AsyncRelayCommand VolverCommand { get; }
    public AsyncRelayCommand EditarTallerCommand { get; }

    private string _nombre = "";
    public string Nombre { get => _nombre; set => SetField(ref _nombre, value); }

    private string _categoria = "";
    public string Categoria { get => _categoria; set => SetField(ref _categoria, value); }

    private string _horario = "";
    public string Horario { get => _horario; set => SetField(ref _horario, value); }

    private string _periodo = "";
    public string Periodo { get => _periodo; set => SetField(ref _periodo, value); }

    private string _costo = "";
    public string Costo { get => _costo; set => SetField(ref _costo, value); }

    private string _ocupacion = "";
    public string Ocupacion { get => _ocupacion; set => SetField(ref _ocupacion, value); }

    private List<InscritoFila> _inscritosLista = new();
    public List<InscritoFila> Inscritos { get => _inscritosLista; set => SetField(ref _inscritosLista, value); }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("Taller", out var valor) && valor is TallerDTO t)
        {
            _taller = t;
            Mostrar(t);
        }
    }

    private void Mostrar(TallerDTO t)
    {
        Nombre = t.Nombre;
        Categoria = Formato.Categoria(t.Categoria);
        Horario = Formato.Horario(t);
        Costo = Formato.Dinero(t.Costo);
        Ocupacion = $"{t.CantidadAlumnos}/{Formato.Maximo(t)}";
    }

    private async Task Cargar()
    {
        if (_taller is null) return;

        // Se vuelve a pedir el taller por si se editó
        var actual = (await _talleres.GetAll()).FirstOrDefault(t => t.Id == _taller.Id);
        if (actual is not null) { _taller = actual; Mostrar(actual); }

        Periodo = (await _periodos.GetAll()).FirstOrDefault(p => p.Id == _taller.IdPeriodo)?.ToString() ?? "";

        var alumnos = await _alumnos.GetAll();
        var suyas = (await _inscripciones.GetAll()).Where(i => i.IdTaller == _taller.Id).ToList();

        Inscritos = (from i in suyas
                     join a in alumnos on i.IdAlumno equals a.Id
                     select new InscritoFila
                     {
                         NombreCompleto = Formato.NombreCompleto(a),
                         Tutor = a.NombreTutor,
                         Telefono = a.Telefono,
                         Estado = i.Debe == true ? "Debe" : "Al corriente",
                         Tono = i.Debe == true ? "Rojo" : "Verde"
                     }).ToList();
    }
}
