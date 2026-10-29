using System.Windows.Input;

namespace SemillitasMobileApp.Commands;

// CAMBIO: versión asíncrona. Mientras el método corre, el botón queda deshabilitado
// (así no se pulsa dos veces). Los errores del API se atrapan dentro del ViewModel con Ejecutar(...).

// Sin parámetro: new AsyncRelayCommand(MetodoAsync)
public class AsyncRelayCommand : ICommand
{
    private readonly Func<Task> _ejecutar;
    private readonly Func<bool>? _puedeEjecutar;
    private bool _corriendo;

    public AsyncRelayCommand(Func<Task> ejecutar, Func<bool>? puedeEjecutar = null)
    {
        _ejecutar = ejecutar;
        _puedeEjecutar = puedeEjecutar;
    }

    public event EventHandler? CanExecuteChanged;

    public bool CanExecute(object? parameter) => !_corriendo && (_puedeEjecutar?.Invoke() ?? true);

    public async void Execute(object? parameter)
    {
        if (!CanExecute(parameter)) return;
        try
        {
            _corriendo = true;
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
            await _ejecutar();
        }
        finally
        {
            _corriendo = false;
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}

// Con parámetro: new AsyncRelayCommand<CobroFila>(MetodoAsync)
public class AsyncRelayCommand<T> : ICommand
{
    private readonly Func<T, Task> _ejecutar;
    private bool _corriendo;

    public AsyncRelayCommand(Func<T, Task> ejecutar)
    {
        _ejecutar = ejecutar;
    }

    public event EventHandler? CanExecuteChanged;

    public bool CanExecute(object? parameter) => !_corriendo && parameter is T;

    public async void Execute(object? parameter)
    {
        if (!CanExecute(parameter) || parameter is not T valor) return;
        try
        {
            _corriendo = true;
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
            await _ejecutar(valor);
        }
        finally
        {
            _corriendo = false;
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
