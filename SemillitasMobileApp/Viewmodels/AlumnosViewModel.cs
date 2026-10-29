using SemillasCore.DTOs;
using SemillasCore.Validators;
using SemillitasMobileApp.Commands;
using SemillitasMobileApp.Helpers;
using SemillitasMobileApp.Services;

namespace SemillitasMobileApp.Viewmodels;

// Lo usan AlumnosView de Windows y de Android (Android no usa los Commands).
// CAMBIO: sin toolkit; se agrega EliminarAlumnoCommand; el tutor es solo NombreTutor.
public class AlumnosViewModel : ViewModelBase
{
    private readonly AlumnosService _alumnos = new();
    private readonly InscripcionesService _inscripciones = new();

    private List<AlumnoDTO> _todos = new();
    private List<InscripcionesDTO> _inscripcionesTodas = new();

    public AlumnosViewModel()
    {
        CargarCommand = new AsyncRelayCommand(() => Ejecutar(Cargar));
        NuevoAlumnoCommand = new AsyncRelayCommand(() => Shell.Current.GoToAsync(Rutas.AlumnoForm));
        VerDetalleAlumnoCommand = new AsyncRelayCommand<AlumnoFila>(fila =>
            Shell.Current.GoToAsync(Rutas.AlumnoDetalle, new Dictionary<string, object> { ["Alumno"] = fila.Alumno }));
        EliminarAlumnoCommand = new AsyncRelayCommand<AlumnoFila>(Eliminar);
    }

    public AsyncRelayCommand CargarCommand { get; }
    public AsyncRelayCommand NuevoAlumnoCommand { get; }
    public AsyncRelayCommand<AlumnoFila> VerDetalleAlumnoCommand { get; }
    public AsyncRelayCommand<AlumnoFila> EliminarAlumnoCommand { get; }

    private string _textoBusqueda = "";
    public string TextoBusqueda
    {
        get => _textoBusqueda;
        set { if (SetField(ref _textoBusqueda, value)) Filtrar(); }
    }

    private List<AlumnoFila> _alumnosLista = new();
    public List<AlumnoFila> Alumnos { get => _alumnosLista; set => SetField(ref _alumnosLista, value); }

    private string _resumenListado = "";
    public string ResumenListado { get => _resumenListado; set => SetField(ref _resumenListado, value); }

    private async Task Cargar()
    {
        _todos = await _alumnos.GetAll();
        _inscripcionesTodas = await _inscripciones.GetAll();
        Filtrar();
    }

    private void Filtrar()
    {
        Alumnos = _todos
            .Where(a => string.IsNullOrWhiteSpace(TextoBusqueda)
                        || Formato.Contiene(Formato.NombreCompleto(a), TextoBusqueda)
                        || Formato.Contiene(a.NombreTutor, TextoBusqueda)
                        || Formato.Contiene(a.Telefono, TextoBusqueda))
            .Select(a =>
            {
                var suyas = _inscripcionesTodas.Where(i => i.IdAlumno == a.Id).ToList();
                var debe = suyas.Any(i => i.Debe == true);
                return new AlumnoFila
                {
                    Alumno = a,
                    NombreCompleto = Formato.NombreCompleto(a),
                    Tutor = a.NombreTutor,
                    Telefono = a.Telefono,
                    CantidadTalleres = suyas.Count,
                    Estado = debe ? "Debe" : "Al corriente",
                    Tono = debe ? "Rojo" : "Verde"
                };
            }).ToList();

        ResumenListado = $"{Alumnos.Count} de {_todos.Count} alumnos";
    }

    // CAMBIO: botón Eliminar de la lista (pide confirmación antes de borrar)
    private async Task Eliminar(AlumnoFila fila)
    {
        if (!await Confirmar("Eliminar alumno", $"¿Eliminar a \"{fila.NombreCompleto}\"?", "Eliminar")) return;

        await Ejecutar(async () =>
        {
            var errores = new ValidatorClass();
            if (!await _alumnos.Delete(fila.Alumno.Id, errores))
            {
                await Avisar("No se pudo eliminar", string.Join("\n", errores.Errores));
                return;
            }
            _todos.Remove(fila.Alumno);
            Filtrar();
        });
    }
}
