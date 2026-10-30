using System.Collections.ObjectModel;
using System.Windows.Threading;
using SemillitasDesktopApp.Infrastructure;

namespace SemillitasDesktopApp.ViewModels;

// Igual que en Sonrisas: tiene un ViewModel por entidad y la sección elegida en el menú.
// Varias secciones usan el mismo ViewModel (por ejemplo Disponibilidad, Agenda y Talleres usan Talleres);
// la vista que se muestra la elige MainWindow según SelectedNav.Key.
public class MainViewModel : ViewModelBase
{
    public record NavItem(string Key, string Title);

    public ObservableCollection<NavItem> NavItems { get; } = new()
    {
        new("Disponibilidad", "Disponibilidad"),
        new("Inscribir", "Inscribir"),
        new("Cobranza", "Cobranza"),
        new("Agenda", "Agenda"),
        new("Talleres", "Talleres"),
        new("Alumnos", "Alumnos"),
        new("Periodos", "Períodos")
    };

    public TalleresViewModel Talleres { get; }
    public AlumnosViewModel Alumnos { get; }
    public PeriodosViewModel Periodos { get; }
    public InscripcionesViewModel Inscripciones { get; }

    private NavItem? _selectedNav;
    public NavItem? SelectedNav
    {
        get => _selectedNav;
        set
        {
            if (!SetField(ref _selectedNav, value)) return;

            ViewModelBase pagina = value?.Key switch
            {
                "Inscribir" or "Cobranza" => Inscripciones,
                "Alumnos" => Alumnos,
                "Periodos" => Periodos,
                _ => Talleres
            };
            CurrentPage = pagina;

            // Cada vez que se entra a una sección se recargan sus datos (otra sección pudo cambiarlos)
            switch (pagina)
            {
                case TalleresViewModel t: t.Cargar(); break;
                case AlumnosViewModel a: a.Cargar(); break;
                case PeriodosViewModel p: p.Cargar(); break;
                case InscripcionesViewModel i: i.Cargar(); break;
            }
        }
    }

    private object? _currentPage;
    public object? CurrentPage
    {
        get => _currentPage;
        private set => SetField(ref _currentPage, value);
    }

    public ObservableCollection<ToastViewModel> Toasts { get; } = new();

    public MainViewModel()
    {
        Talleres = new TalleresViewModel(this);
        Alumnos = new AlumnosViewModel(this);
        Periodos = new PeriodosViewModel(this);
        Inscripciones = new InscripcionesViewModel(this);

        SelectedNav = NavItems[0];
    }

    // Para cambiar de sección desde otro ViewModel (por ejemplo "Inscribir" desde Disponibilidad)
    public void IrA(string key) => SelectedNav = NavItems.First(n => n.Key == key);

    public void ShowToast(string title, string message)
    {
        var toast = new ToastViewModel(title, message);
        Toasts.Add(toast);

        var timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(3.5) };
        timer.Tick += (_, _) =>
        {
            Toasts.Remove(toast);
            timer.Stop();
        };
        timer.Start();
    }
}

public class ToastViewModel
{
    public string Title { get; }
    public string Message { get; }

    public ToastViewModel(string title, string message)
    {
        Title = title;
        Message = message;
    }
}
