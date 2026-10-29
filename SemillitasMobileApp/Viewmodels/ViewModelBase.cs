using System.ComponentModel;
using System.Runtime.CompilerServices;
using SemillasCore.Validators;

namespace SemillitasMobileApp.Viewmodels;

// CAMBIO: reemplaza a BaseViewModel y al toolkit. Es tu ejemplo (INotifyPropertyChanged + SetField)
// más lo que usaban todos los ViewModels: IsBusy y el manejo de errores del API.
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

    private bool _isBusy;
    public bool IsBusy
    {
        get => _isBusy;
        set => SetField(ref _isBusy, value);
    }

    // Corre una acción mostrando el indicador de carga y avisando si no hay conexión
    protected async Task Ejecutar(Func<Task> accion)
    {
        if (IsBusy) return;
        try
        {
            IsBusy = true;
            await accion();
        }
        catch (HttpRequestException)
        {
            await Avisar("Sin conexión", "No se pudo comunicar con el servidor.");
        }
        catch (TaskCanceledException)
        {
            await Avisar("Sin respuesta", "El servidor tardó demasiado en responder.");
        }
        finally
        {
            IsBusy = false;
        }
    }

    protected Task Avisar(string titulo, string mensaje) =>
        Shell.Current.DisplayAlert(titulo, mensaje, "Aceptar");

    protected Task<bool> Confirmar(string titulo, string mensaje, string aceptar) =>
        Shell.Current.DisplayAlert(titulo, mensaje, aceptar, "Cancelar");

    // Vuelve a la pantalla anterior
    protected Task Regresar() => Shell.Current.GoToAsync("..");

    // Una sola lista de errores por formulario (HayErrores + Errores), que es lo que pidió el cliente
    private bool _hayErrores;
    public bool HayErrores
    {
        get => _hayErrores;
        private set => SetField(ref _hayErrores, value);
    }

    private List<string> _errores = new();
    public List<string> Errores
    {
        get => _errores;
        private set => SetField(ref _errores, value);
    }

    protected void MostrarErrores(IEnumerable<string> mensajes)
    {
        Errores = mensajes.ToList();
        HayErrores = Errores.Count > 0;
    }

    protected void LimpiarErrores() => MostrarErrores(Array.Empty<string>());

    // Muestra los errores de un validator compartido (SemillasCore). Devuelve true si todo está bien.
    protected bool EsValido(ValidatorClass resultado)
    {
        MostrarErrores(resultado.Errores);
        return resultado.EsValido;
    }
}
