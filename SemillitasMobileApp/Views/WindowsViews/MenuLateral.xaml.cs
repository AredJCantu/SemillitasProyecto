namespace SemillitasMobileApp.Views.WindowsViews;

/// <summary>
/// Barra lateral reutilizable. Única "lógica": la propiedad Activo, que
/// permite a cada página decir cuál opción debe verse resaltada.
/// </summary>
public partial class MenuLateral : ContentView
{
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
