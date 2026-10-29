using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace SemillitasMobileApp.Viewmodels
{
    public partial class MenuLateralViewModel : ObservableObject
    {
        [RelayCommand]
        private async Task Navegar(string ruta)
        {
            await Shell.Current.GoToAsync($"//{ruta}");
        }
    }
}
