using System.Windows.Input;

namespace SemillitasMobileApp.Commands;

// CAMBIO: comandos propios para reemplazar [RelayCommand] del toolkit. Implementan ICommand,
// que es lo que usan Button.Command y TapGestureRecognizer.Command.

// Comando sin parámetro: new RelayCommand(Metodo)
public class RelayCommand : ICommand
{
    private readonly Action _ejecutar;
    private readonly Func<bool>? _puedeEjecutar;

    public RelayCommand(Action ejecutar, Func<bool>? puedeEjecutar = null)
    {
        _ejecutar = ejecutar;
        _puedeEjecutar = puedeEjecutar;
    }

    public event EventHandler? CanExecuteChanged;

    public bool CanExecute(object? parameter) => _puedeEjecutar?.Invoke() ?? true;

    public void Execute(object? parameter) => _ejecutar();

    public void NotificarCambio() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
}

// Comando con parámetro (CommandParameter): new RelayCommand<TallerFila>(Metodo)
public class RelayCommand<T> : ICommand
{
    private readonly Action<T> _ejecutar;
    private readonly Func<T, bool>? _puedeEjecutar;

    public RelayCommand(Action<T> ejecutar, Func<T, bool>? puedeEjecutar = null)
    {
        _ejecutar = ejecutar;
        _puedeEjecutar = puedeEjecutar;
    }

    public event EventHandler? CanExecuteChanged;

    public bool CanExecute(object? parameter) =>
        parameter is T valor && (_puedeEjecutar?.Invoke(valor) ?? true);

    public void Execute(object? parameter)
    {
        if (parameter is T valor) _ejecutar(valor);
    }

    public void NotificarCambio() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
}
