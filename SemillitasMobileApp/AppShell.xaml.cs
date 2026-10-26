using SemillitasMobileApp.Views.WindowsViews;

namespace SemillitasMobileApp;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        Routing.RegisterRoute(nameof(TallerFormView), typeof(TallerFormView));
    }
}
