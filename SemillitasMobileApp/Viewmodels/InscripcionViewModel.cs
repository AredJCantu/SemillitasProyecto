using SemillasCore.DTOs;
using SemillasCore.Validators;
using SemillitasMobileApp.Commands;
using SemillitasMobileApp.Helpers;
using SemillitasMobileApp.Services;

namespace SemillitasMobileApp.Viewmodels;

// Es una sección raíz del Shell. Desde Disponibilidad llega opcionalmente { "Taller": taller }.
// CAMBIOS: sin toolkit; sin ApellidoTutor; el cupo lleno sale de TallerDTO.Cupo (lugares que quedan);
// el choque de horario revisa entre semana y sábado; los datos del alumno se validan con
// CreateAlumnoValidator de SemillasCore.
public class InscripcionViewModel : ViewModelBase, IQueryAttributable
{
    private readonly TalleresService _talleres = new();
    private readonly AlumnosService _alumnos = new();
    private readonly InscripcionesService _inscripciones = new();
    private readonly PeriodosService _periodos = new();

    private List<TallerDTO> _todosTalleres = new();
    private List<AlumnoDTO> _todosAlumnos = new();
    private readonly List<TallerDTO> _agregados = new();   // talleres elegidos en esta inscripción
    private AlumnoDTO? _alumnoExistente;                   // si se eligió con el buscador
    private PeriodoDTO? _periodoActual;
    private TallerDTO? _tallerInicial;                     // el que se pulsó en Disponibilidad

    public InscripcionViewModel()
    {
        CargarCommand = new AsyncRelayCommand(() => Ejecutar(Cargar));
        AlternarTallerCommand = new RelayCommand<TallerInscripcionFila>(AlternarTaller);
        QuitarDelResumenCommand = new RelayCommand<ResumenFila>(QuitarDelResumen);
        CancelarCommand = new AsyncRelayCommand(Cancelar);
        ConfirmarInscripcionCommand = new AsyncRelayCommand(ConfirmarInscripcion);
    }

    public AsyncRelayCommand CargarCommand { get; }
    public RelayCommand<TallerInscripcionFila> AlternarTallerCommand { get; }
    public RelayCommand<ResumenFila> QuitarDelResumenCommand { get; }
    public AsyncRelayCommand CancelarCommand { get; }
    public AsyncRelayCommand ConfirmarInscripcionCommand { get; }

    private string _subtituloInscripcion = "";
    public string SubtituloInscripcion { get => _subtituloInscripcion; set => SetField(ref _subtituloInscripcion, value); }

    private string _periodo = "";
    public string Periodo { get => _periodo; set => SetField(ref _periodo, value); }

    private string _total = "$0";
    public string Total { get => _total; set => SetField(ref _total, value); }

    private bool _hayError;
    public bool HayError { get => _hayError; set => SetField(ref _hayError, value); }

    private string _mensajeError = "";
    public string MensajeError { get => _mensajeError; set => SetField(ref _mensajeError, value); }

    // Buscador: si hay una sola coincidencia se rellenan los datos del alumno
    private string _textoBusquedaAlumno = "";
    public string TextoBusquedaAlumno
    {
        get => _textoBusquedaAlumno;
        set { if (SetField(ref _textoBusquedaAlumno, value)) BuscarAlumno(value); }
    }

    private string _nombreAlumno = "";
    public string NombreAlumno { get => _nombreAlumno; set => SetField(ref _nombreAlumno, value); }

    private string _apellidoAlumno = "";
    public string ApellidoAlumno { get => _apellidoAlumno; set => SetField(ref _apellidoAlumno, value); }

    private string _nombreTutor = "";
    public string NombreTutor { get => _nombreTutor; set => SetField(ref _nombreTutor, value); }

    private string _telefonoAlumno = "";
    public string TelefonoAlumno { get => _telefonoAlumno; set => SetField(ref _telefonoAlumno, value); }

    private List<TallerInscripcionFila> _talleresLista = new();
    public List<TallerInscripcionFila> Talleres { get => _talleresLista; set => SetField(ref _talleresLista, value); }

    private List<ResumenFila> _resumen = new();
    public List<ResumenFila> Resumen { get => _resumen; set => SetField(ref _resumen, value); }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("Taller", out var valor) && valor is TallerDTO t)
        {
            _tallerInicial = t;
            // Si los datos ya estaban cargados no se espera a Appearing
            if (_todosTalleres.Count > 0) AplicarTallerInicial();
        }
    }

    private async Task Cargar()
    {
        _todosTalleres = await _talleres.GetAll();
        _todosAlumnos = await _alumnos.GetAll();
        var periodos = await _periodos.GetAll();

        // Período: el del taller elegido, o el que ya había, o el más reciente
        var idPeriodo = _tallerInicial?.IdPeriodo ?? _periodoActual?.Id;
        _periodoActual = periodos.FirstOrDefault(p => p.Id == idPeriodo) ?? periodos.OrderBy(p => p.Id).LastOrDefault();
        Periodo = _periodoActual?.ToString() ?? "";
        SubtituloInscripcion = $"Período {Periodo}";

        AplicarTallerInicial();
    }

    private void AplicarTallerInicial()
    {
        if (_tallerInicial is not null && !_agregados.Any(t => t.Id == _tallerInicial.Id) && !EstaLleno(_tallerInicial))
            _agregados.Add(_tallerInicial);
        _tallerInicial = null;
        Reconstruir();
    }

    // Cupo = lugares que quedan, así que 0 significa lleno
    private static bool EstaLleno(TallerDTO t) => t.Cupo <= 0;

    private static bool Traslapa(int inicioA, int finA, int inicioB, int finB) => inicioA < finB && inicioB < finA;

    // CAMBIO: choque si se traslapan entre semana o el sábado
    private static bool Chocan(TallerDTO a, TallerDTO b) =>
        Traslapa(a.HoraInicio, a.HoraFin, b.HoraInicio, b.HoraFin)
        || Traslapa(a.HoraInicioSabado, a.HoraFinSabado, b.HoraInicioSabado, b.HoraFinSabado);

    // Recalcula la acción de cada taller (Disponible/Agregado/Choque/Lleno), el resumen y el total
    private void Reconstruir()
    {
        Talleres = _todosTalleres
            .Where(t => t.IdPeriodo == _periodoActual?.Id)
            .Select(t =>
            {
                var (badge, tono) = Formato.Lugares(t.Cupo, Formato.Maximo(t));

                var accion = _agregados.Any(a => a.Id == t.Id) ? "Agregado"
                           : EstaLleno(t) ? "Lleno"
                           : _agregados.Any(a => Chocan(a, t)) ? "Choque"
                           : "Disponible";

                return new TallerInscripcionFila
                {
                    Taller = t,
                    Nombre = t.Nombre,
                    Horario = Formato.Horario(t),
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
            Horario = Formato.Horario(t),
            Precio = Formato.Dinero(t.Costo)
        }).ToList();

        Total = Formato.Dinero(_agregados.Sum(t => t.Costo));
    }

    private void BuscarAlumno(string texto)
    {
        if (string.IsNullOrWhiteSpace(texto))
        {
            _alumnoExistente = null;
            return;
        }

        var coincidencias = _todosAlumnos
            .Where(a => Formato.Contiene(Formato.NombreCompleto(a), texto.Trim()))
            .ToList();

        if (coincidencias.Count != 1) return;

        var elegido = coincidencias[0];
        _alumnoExistente = elegido;
        NombreAlumno = elegido.Nombre;
        ApellidoAlumno = elegido.Apellido;
        NombreTutor = elegido.NombreTutor;
        TelefonoAlumno = elegido.Telefono;
    }

    private void AlternarTaller(TallerInscripcionFila fila)
    {
        if (fila.Accion == "Disponible") _agregados.Add(fila.Taller);
        else if (fila.Accion == "Agregado") _agregados.RemoveAll(t => t.Id == fila.Taller.Id);
        else return;   // Choque y Lleno no hacen nada

        HayError = false;
        Reconstruir();
    }

    private void QuitarDelResumen(ResumenFila fila)
    {
        _agregados.RemoveAll(t => t.Id == fila.Taller.Id);
        Reconstruir();
    }

    private async Task Cancelar()
    {
        if (_agregados.Count > 0 &&
            !await Confirmar("Cancelar inscripción", "Se perderán los talleres elegidos.", "Sí, cancelar"))
            return;

        Limpiar();
        await Shell.Current.GoToAsync($"//{Rutas.Disponibilidad}");
    }

    private async Task ConfirmarInscripcion()
    {
        HayError = false;

        // Los datos del alumno se validan con el mismo validator que usa el API
        var datos = new CreateAlumnoDTO
        {
            Nombre = NombreAlumno.Trim(),
            Apellido = ApellidoAlumno.Trim(),
            NombreTutor = NombreTutor.Trim(),
            Telefono = TelefonoAlumno.Trim()
        };
        var errores = new CreateAlumnoValidator().IsValid(datos).Errores;
        if (_agregados.Count == 0) errores.Add("Agrega al menos un taller.");
        if (errores.Count > 0)
        {
            MostrarError(string.Join("\n", errores));
            return;
        }

        await Ejecutar(async () =>
        {
            // Alumno nuevo: se crea primero para tener su Id. Alumno existente: se usa tal cual.
            var fallos = new ValidatorClass();
            var alumno = _alumnoExistente ?? await _alumnos.Create(datos, fallos);
            if (alumno is null)
            {
                MostrarError(string.Join("\n", fallos.Errores));
                return;
            }

            // Una inscripción por taller; Debe = true hasta que se marque el pago en Cobranza
            foreach (var taller in _agregados.ToList())
            {
                var nueva = new CreateInscripcionesDTO { IdAlumno = alumno.Id, IdTaller = taller.Id, Debe = true };
                if (await _inscripciones.Create(nueva, fallos) is null)
                {
                    MostrarError(string.Join("\n", fallos.Errores));   // choque de horario, cupo lleno...
                    return;
                }
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
        NombreAlumno = ApellidoAlumno = NombreTutor = TelefonoAlumno = "";
        HayError = false;
        Reconstruir();
    }
}
