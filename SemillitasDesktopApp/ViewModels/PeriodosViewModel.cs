using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using SemillasCore.DTOs;
using SemillasCore.Validators;
using SemillitasDesktopApp.Infrastructure;
using SemillitasDesktopApp.Models;
using SemillitasDesktopApp.Services;

namespace SemillitasDesktopApp.ViewModels;

// Todo lo de períodos: lista, formulario (nuevo/editar) y eliminar
public class PeriodosViewModel : ViewModelBase
{
    PeriodosService periodosService = new();
    TalleresService talleresService = new();

    private readonly MainViewModel _main;
    private PeriodoDTO? _editando;

    public ObservableCollection<PeriodoFila> Periodos { get; } = new();

    // ---------- Formulario ----------

    private bool _isFormularioAbierto;
    public bool IsFormularioAbierto { get => _isFormularioAbierto; set => SetField(ref _isFormularioAbierto, value); }

    private string _tituloFormulario = "";
    public string TituloFormulario { get => _tituloFormulario; set => SetField(ref _tituloFormulario, value); }

    public List<int> Meses { get; } = Enumerable.Range(1, 12).ToList();

    private int? _mesInicio;
    public int? MesInicio { get => _mesInicio; set => SetField(ref _mesInicio, value); }

    private int? _mesFin;
    public int? MesFin { get => _mesFin; set => SetField(ref _mesFin, value); }

    private string _anio = "";
    public string Anio { get => _anio; set => SetField(ref _anio, value); }

    // ---------- Commands ----------

    public ICommand NuevoCommand { get; }
    public ICommand EditarCommand { get; }
    public ICommand EliminarCommand { get; }
    public ICommand GuardarCommand { get; }
    public ICommand CerrarCommand { get; }

    public PeriodosViewModel(MainViewModel main)
    {
        _main = main;

        NuevoCommand = new RelayCommand(_ => Nuevo());
        EditarCommand = new RelayCommand(Editar);
        EliminarCommand = new RelayCommand(Eliminar);
        GuardarCommand = new RelayCommand(_ => Guardar());
        CerrarCommand = new RelayCommand(_ => IsFormularioAbierto = false);
    }

    public async void Cargar()
    {
        try
        {
            var lista = await periodosService.GetAll();
            var talleres = await talleresService.GetAll();

            Periodos.Clear();
            foreach (var p in lista.OrderBy(p => p.Id))
            {
                Periodos.Add(new PeriodoFila
                {
                    Periodo = p,
                    MesInicio = p.MesInicio,
                    MesFin = p.MesFin,
                    Anio = p.Año,
                    CantidadTalleres = talleres.Count(t => t.IdPeriodo == p.Id)
                });
            }
        }
        catch (Exception ex)
        {
            _main.ShowToast("Sin conexión", ex.Message);
        }
    }

    private void Nuevo()
    {
        _editando = null;
        TituloFormulario = "Nuevo período";
        MesInicio = MesFin = null;
        Anio = DateTime.Today.Year.ToString();
        Errores.Clear();
        IsFormularioAbierto = true;
    }

    private void Editar(object? parametro)
    {
        if (parametro is not PeriodoFila fila) return;

        _editando = fila.Periodo;
        TituloFormulario = "Editar período";
        MesInicio = fila.MesInicio;
        MesFin = fila.MesFin;
        Anio = fila.Anio.ToString();
        Errores.Clear();
        IsFormularioAbierto = true;
    }

    private async void Guardar()
    {
        // Si el año no es número se manda 0 y el validator del Core lo marca como inválido
        int.TryParse(Anio, out var anio);
        var dto = new EditPeriodoDTO
        {
            Id = _editando?.Id ?? 0,
            MesInicio = MesInicio ?? 0,
            MesFin = MesFin ?? 0,
            Año = anio
        };

        try
        {
            var errores = new ValidatorClass();
            bool listo;

            if (_editando is null)
            {
                var validacion = new CreatePeriodoValidator().IsValid(dto);
                if (!validacion.EsValido) { MostrarErrores(validacion.Errores); return; }
                listo = await periodosService.Create(dto, errores) != null;
            }
            else
            {
                var validacion = new EditPeriodoValidator().IsValid(dto);
                if (!validacion.EsValido) { MostrarErrores(validacion.Errores); return; }
                listo = await periodosService.Update(dto, errores);
            }

            if (!listo)
            {
                MostrarErrores(errores.Errores);
                return;
            }

            IsFormularioAbierto = false;
            _main.ShowToast("Período guardado", $"Se guardó {dto.MesInicio}-{dto.MesFin}-{dto.Año}.");
            Cargar();
        }
        catch (Exception ex)
        {
            _main.ShowToast("Sin conexión", ex.Message);
        }
    }

    private async void Eliminar(object? parametro)
    {
        if (parametro is not PeriodoFila fila) return;

        var respuesta = MessageBox.Show($"¿Eliminar el período {fila.Periodo}?", "Eliminar período",
            MessageBoxButton.YesNo, MessageBoxImage.Question);
        if (respuesta != MessageBoxResult.Yes) return;

        try
        {
            var errores = new ValidatorClass();
            if (await periodosService.Delete(fila.Periodo.Id, errores))
            {
                _main.ShowToast("Período eliminado", $"Se eliminó {fila.Periodo}.");
                Cargar();
            }
            else
            {
                _main.ShowToast("No se pudo eliminar", string.Join("\n", errores.Errores));
            }
        }
        catch (Exception ex)
        {
            _main.ShowToast("Sin conexión", ex.Message);
        }
    }
}
