using SemillitasMobileApp.Views.WindowsViews;

namespace SemillitasMobileApp;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        // Pantallas que se abren encima de otra: await Shell.Current.GoToAsync(nameof(TallerFormView));
        Routing.RegisterRoute(nameof(TallerFormView), typeof(TallerFormView));
        Routing.RegisterRoute(nameof(AlumnoDetalleView), typeof(AlumnoDetalleView));
        Routing.RegisterRoute(nameof(AlumnoFormView), typeof(AlumnoFormView));
        Routing.RegisterRoute(nameof(TallerDetalleView), typeof(TallerDetalleView));
        Routing.RegisterRoute(nameof(PeriodoFormView), typeof(PeriodoFormView));
    }
}
