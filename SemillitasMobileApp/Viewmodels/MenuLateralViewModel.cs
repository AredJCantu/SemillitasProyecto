using SemillitasMobileApp.Commands;

namespace SemillitasMobileApp.Viewmodels
{
    // CAMBIO: sin toolkit; el botón del menú manda el nombre de la ruta como CommandParameter
    public class MenuLateralViewModel : ViewModelBase
    {
        public MenuLateralViewModel()
        {
            NavegarCommand = new AsyncRelayCommand<string>(Navegar);
        }

        public AsyncRelayCommand<string> NavegarCommand { get; }

        private Task Navegar(string ruta) => Shell.Current.GoToAsync($"//{ruta}");
    }
}
