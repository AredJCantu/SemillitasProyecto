namespace SemillitasMobileApp.Views.WindowsViews;

public partial class MenuLateral : ContentView
{
    // Opción resaltada: "Disponibilidad", "Inscribir", "Cobranza", "Agenda", "Alumnos" o "Talleres"
    public static readonly BindableProperty ActivoProperty =
        BindableProperty.Create(nameof(Activo), typeof(string), typeof(MenuLateral), string.Empty);

    public string Activo
    {
        get => (string)GetValue(ActivoProperty);
        set => SetValue(ActivoProperty, value);
    }

    public MenuLateral()
    {
        InitializeComponent();
    }
}
