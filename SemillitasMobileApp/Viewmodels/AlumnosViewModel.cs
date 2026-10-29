using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SemillitasMobileApp.Helpers;
using SemillitasMobileApp.Models;
using SemillitasMobileApp.Services;

namespace SemillitasMobileApp.Viewmodels;

// Lo usan AlumnosView de Windows y de Android (Android no usa los Commands)
public partial class AlumnosViewModel : BaseViewModel
{
    private readonly AlumnosService _alumnos = new();
    private readonly InscripcionesService _inscripciones = new();

    private List<Alumno> _todos = new();
    private List<Inscripcion> _inscripcionesTodas = new();

    [ObservableProperty] private string textoBusqueda = "";
    [ObservableProperty] private List<AlumnoFila> alumnos = new();
    [ObservableProperty] private string resumenListado = "";

    partial void OnTextoBusquedaChanged(string value) => Filtrar();

    [RelayCommand]
    private Task Cargar() => Ejecutar(async () =>
    {
        _todos = await _alumnos.ObtenerAsync();
        _inscripcionesTodas = await _inscripciones.ObtenerAsync();
        Filtrar();
    });

    private void Filtrar()
    {
        Alumnos = _todos
            .Where(a => string.IsNullOrWhiteSpace(TextoBusqueda)
                        || Formato.Contiene(Formato.NombreCompleto(a), TextoBusqueda)
                        || Formato.Contiene(Formato.Tutor(a), TextoBusqueda)
                        || Formato.Contiene(a.Telefono, TextoBusqueda))
            .Select(a =>
            {
                var suyas = _inscripcionesTodas.Where(i => i.IdAlumno == a.Id).ToList();
                var debe = suyas.Any(i => i.Debe);
                return new AlumnoFila
                {
                    Alumno = a,
                    NombreCompleto = Formato.NombreCompleto(a),
                    Tutor = Formato.Tutor(a),
                    Telefono = a.Telefono,
                    CantidadTalleres = suyas.Count,
                    Estado = debe ? "Debe" : "Al corriente",
                    Tono = debe ? "Rojo" : "Verde"
                };
            }).ToList();

        ResumenListado = $"{Alumnos.Count} de {_todos.Count} alumnos";
    }

    [RelayCommand]
    private Task NuevoAlumno() => Shell.Current.GoToAsync(Rutas.AlumnoForm);

    [RelayCommand]
    private Task VerDetalleAlumno(AlumnoFila fila) =>
        Shell.Current.GoToAsync(Rutas.AlumnoDetalle, new Dictionary<string, object> { ["Alumno"] = fila.Alumno });
}
