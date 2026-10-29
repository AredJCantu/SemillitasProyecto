using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SemillitasMobileApp.Helpers;
using SemillitasMobileApp.Models;
using SemillitasMobileApp.Services;

namespace SemillitasMobileApp.Viewmodels;

// Es una sección raíz del Shell. Desde Disponibilidad llega opcionalmente { "Taller": taller }.
public partial class InscripcionViewModel : BaseViewModel, IQueryAttributable
{
    private readonly TalleresService _talleres = new();
    private readonly AlumnosService _alumnos = new();
    private readonly InscripcionesService _inscripciones = new();
    private readonly PeriodosService _periodos = new();

    private List<Taller> _todosTalleres = new();
    private List<Inscripcion> _inscritos = new();
    private List<Alumno> _todosAlumnos = new();
    private readonly List<Taller> _agregados = new();   // talleres elegidos en esta inscripción
    private Alumno? _alumnoExistente;                   // si se eligió con el buscador
    private Periodo? _periodo;
    private Taller? _tallerInicial;                     // el que se pulsó en Disponibilidad

    [ObservableProperty] private string subtituloInscripcion = "";
    [ObservableProperty] private string periodo = "";
    [ObservableProperty] private string total = "$0";
    [ObservableProperty] private bool hayError;
    [ObservableProperty] private string mensajeError = "";
    [ObservableProperty] private string textoBusquedaAlumno = "";
    [ObservableProperty] private string nombreAlumno = "";
    [ObservableProperty] private string apellidoAlumno = "";
    [ObservableProperty] private string nombreTutor = "";
    [ObservableProperty] private string apellidoTutor = "";
    [ObservableProperty] private string telefonoAlumno = "";
    [ObservableProperty] private List<TallerInscripcionFila> talleres = new();
    [ObservableProperty] private List<ResumenFila> resumen = new();

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("Taller", out var valor) && valor is Taller t)
        {
            _tallerInicial = t;
            // Si los datos ya estaban cargados no se espera a Appearing
            if (_todosTalleres.Count > 0) AplicarTallerInicial();
        }
    }

    [RelayCommand]
    private Task Cargar() => Ejecutar(async () =>
    {
        _todosTalleres = await _talleres.ObtenerAsync();
        _inscritos = await _inscripciones.ObtenerAsync();
        _todosAlumnos = await _alumnos.ObtenerAsync();
        var periodos = await _periodos.ObtenerAsync();

        // Período: el del taller elegido, o el que ya había, o el más reciente
        var idPeriodo = _tallerInicial?.IdPeriodo ?? _periodo?.Id;
        _periodo = periodos.FirstOrDefault(p => p.Id == idPeriodo) ?? periodos.OrderBy(p => p.Id).LastOrDefault();
        Periodo = _periodo?.ToString() ?? "";
        SubtituloInscripcion = $"Período {Periodo}";

        AplicarTallerInicial();
    });

    private void AplicarTallerInicial()
    {
        if (_tallerInicial is not null && !_agregados.Any(t => t.Id == _tallerInicial.Id) && !EstaLleno(_tallerInicial))
            _agregados.Add(_tallerInicial);
        _tallerInicial = null;
        Reconstruir();
    }

    private bool EstaLleno(Taller t) => _inscritos.Count(i => i.IdTaller == t.Id) >= t.Cupo;

    private static bool Chocan(Taller a, Taller b) => a.HoraInicio < b.HoraFin && b.HoraInicio < a.HoraFin;

    // Recalcula la acción de cada taller (Disponible/Agregado/Choque/Lleno), el resumen y el total
    private void Reconstruir()
    {
        Talleres = _todosTalleres
            .Where(t => t.IdPeriodo == _periodo?.Id)
            .Select(t =>
            {
                var inscritos = _inscritos.Count(i => i.IdTaller == t.Id);
                var (badge, tono) = Formato.Lugares(t.Cupo, inscritos);

                var accion = _agregados.Any(a => a.Id == t.Id) ? "Agregado"
                           : EstaLleno(t) ? "Lleno"
                           : _agregados.Any(a => Chocan(a, t)) ? "Choque"
                           : "Disponible";

                return new TallerInscripcionFila
                {
                    Taller = t,
                    Nombre = t.Nombre,
                    Horario = Formato.Rango(t),
                    Costo = Formato.Dinero(t.Costo),
                    Badge = badge,
                    Tono = tono,
                    Accion = accion
                };
            }).ToList();

        Resumen = _agregados.Select(t => new ResumenFila
        {
            Taller = t,
            Nombre = t.Nombre,
            Horario = Formato.Rango(t),
            Precio = Formato.Dinero(t.Costo)
        }).ToList();

        Total = Formato.Dinero(_agregados.Sum(t => t.Costo));
    }

    // Buscador: si hay una sola coincidencia se rellenan los datos del alumno
    partial void OnTextoBusquedaAlumnoChanged(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            _alumnoExistente = null;
            return;
        }

        var coincidencias = _todosAlumnos
            .Where(a => Formato.Contiene(Formato.NombreCompleto(a), value.Trim()))
            .ToList();

        if (coincidencias.Count != 1) return;

        var elegido = coincidencias[0];
        _alumnoExistente = elegido;
        NombreAlumno = elegido.Nombre;
        ApellidoAlumno = elegido.Apellido;
        NombreTutor = elegido.NombreTutor;
        ApellidoTutor = elegido.ApellidoTutor;
        TelefonoAlumno = elegido.Telefono;
    }

    [RelayCommand]
    private void AlternarTaller(TallerInscripcionFila fila)
    {
        if (fila.Accion == "Disponible") _agregados.Add(fila.Taller);
        else if (fila.Accion == "Agregado") _agregados.RemoveAll(t => t.Id == fila.Taller.Id);
        else return;   // Choque y Lleno no hacen nada

        HayError = false;
        Reconstruir();
    }

    [RelayCommand]
    private void QuitarDelResumen(ResumenFila fila)
    {
        _agregados.RemoveAll(t => t.Id == fila.Taller.Id);
        Reconstruir();
    }

    [RelayCommand]
    private async Task Cancelar()
    {
        if (_agregados.Count > 0 &&
            !await Confirmar("Cancelar inscripción", "Se perderán los talleres elegidos.", "Sí, cancelar"))
            return;

        Limpiar();
        await Shell.Current.GoToAsync($"//{Rutas.Disponibilidad}");
    }

    [RelayCommand]
    private async Task ConfirmarInscripcion()
    {
        HayError = false;

        if (string.IsNullOrWhiteSpace(NombreAlumno) || string.IsNullOrWhiteSpace(ApellidoAlumno)
            || string.IsNullOrWhiteSpace(NombreTutor) || string.IsNullOrWhiteSpace(ApellidoTutor)
            || string.IsNullOrWhiteSpace(TelefonoAlumno))
        {
            MostrarError("Completa los datos del alumno y del tutor.");
            return;
        }
        if (_agregados.Count == 0)
        {
            MostrarError("Agrega al menos un taller.");
            return;
        }

        await Ejecutar(async () =>
        {
            try
            {
                // Alumno nuevo: se crea primero para tener su Id. Alumno existente: se usa tal cual.
                var alumno = _alumnoExistente ?? await _alumnos.CrearAsync(new Alumno
                {
                    Nombre = NombreAlumno.Trim(),
                    Apellido = ApellidoAlumno.Trim(),
                    NombreTutor = NombreTutor.Trim(),
                    ApellidoTutor = ApellidoTutor.Trim(),
                    Telefono = TelefonoAlumno.Trim()
                }) ?? throw new ApiException("El servidor no devolvió el alumno creado.");

                // Una inscripción por taller; Debe = true hasta que se marque el pago en Cobranza
                foreach (var taller in _agregados.ToList())
                    await _inscripciones.CrearAsync(new Inscripcion { IdAlumno = alumno.Id, IdTaller = taller.Id, Debe = true });
            }
            catch (ApiException ex)
            {
                MostrarError(ex.Message);   // choque de horario, cupo lleno, datos inválidos...
                return;
            }

            await Avisar("Listo", "Inscripción registrada.");
            Limpiar();
            await Shell.Current.GoToAsync($"//{Rutas.Disponibilidad}");
        });
    }

    private void MostrarError(string mensaje)
    {
        MensajeError = mensaje;
        HayError = true;
    }

    private void Limpiar()
    {
        _agregados.Clear();
        _alumnoExistente = null;
        TextoBusquedaAlumno = "";
        NombreAlumno = ApellidoAlumno = NombreTutor = ApellidoTutor = TelefonoAlumno = "";
        HayError = false;
        Reconstruir();
    }
}
