using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SemillitasDesktopApp.Infrastructure;

public abstract class ViewModelBase : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string? name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

    protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? name = null)
    {
        if (Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(name);
        return true;
    }

    // Lista única de errores del formulario (validators del Core o respuesta del API)
    public ObservableCollection<string> Errores { get; } = new();

    protected void MostrarErrores(IEnumerable<string> errores)
    {
        Errores.Clear();
        foreach (var e in errores) Errores.Add(e);
    }
}
