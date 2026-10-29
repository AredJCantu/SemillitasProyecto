using CommunityToolkit.Mvvm.ComponentModel;
using SemillitasMobileApp.Services;

namespace SemillitasMobileApp.Viewmodels;

// Lo que comparten todos los ViewModels: IsBusy y manejo de errores del API
public abstract partial class BaseViewModel : ObservableObject
{
    [ObservableProperty]
    private bool isBusy;

    // Corre una acción mostrando el indicador de carga y avisando si el API falla
    protected async Task Ejecutar(Func<Task> accion)
    {
        if (IsBusy) return;
        try
        {
            IsBusy = true;
            await accion();
        }
        catch (ApiException ex)
        {
            await Avisar("No se pudo completar", ex.Message);
        }
        catch (HttpRequestException)
        {
            await Avisar("Sin conexión", "No se pudo comunicar con el servidor.");
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
}
