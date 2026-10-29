using System.Windows.Input;

namespace SemillitasMobileApp.Behaviors;

// Ejecuta un Command cada vez que la página aparece (para recargar datos sin code-behind).
// Uso en XAML:
//   <ContentPage.Behaviors><b:AlAparecerBehavior Command="{Binding CargarCommand}" /></ContentPage.Behaviors>
public class AlAparecerBehavior : Behavior<ContentPage>
{
    public static readonly BindableProperty CommandProperty =
        BindableProperty.Create(nameof(Command), typeof(ICommand), typeof(AlAparecerBehavior));

    public ICommand? Command
    {
        get => (ICommand?)GetValue(CommandProperty);
        set => SetValue(CommandProperty, value);
    }

    private ContentPage? _pagina;

    protected override void OnAttachedTo(ContentPage pagina)
    {
        base.OnAttachedTo(pagina);
        _pagina = pagina;
        BindingContext = pagina.BindingContext;
        pagina.BindingContextChanged += AlCambiarContexto;
        pagina.Appearing += AlAparecer;
    }

    protected override void OnDetachingFrom(ContentPage pagina)
    {
        pagina.BindingContextChanged -= AlCambiarContexto;
        pagina.Appearing -= AlAparecer;
        _pagina = null;
        base.OnDetachingFrom(pagina);
    }

    private void AlCambiarContexto(object? sender, EventArgs e) => BindingContext = _pagina?.BindingContext;

    private void AlAparecer(object? sender, EventArgs e)
    {
        if (Command?.CanExecute(null) == true)
            Command.Execute(null);
    }
}
