namespace SemillitasMobileApp.Views.WindowsViews;

/// <summary>
/// Code-behind de TallerFormView (Windows). Sin lógica.
/// El bloque "DATOS DE PRUEBA" es solo para ver la pantalla con datos; bórralo
/// cuando exista el ViewModel real.
/// </summary>
public partial class TallerFormView : ContentPage
{
    public TallerFormView()
    {
        InitializeComponent();
        CargarDatosDePrueba(); // TODO: borrar cuando exista el ViewModel real
    }

    #region DATOS DE PRUEBA (borrar)
    private void CargarDatosDePrueba()
    {
        BindingContext = new
        {
            Titulo = "Editar taller",
            EsEdicion = true,
            Nombre = "Futbol infantil",
            ErrorNombre = "",
            TieneErrorNombre = false,
            Categorias = new[] { "Arte", "Deportes", "Tecnología", "Idiomas" },
            CategoriaSeleccionada = "Deportes",
            Instructores = new[] { "Laura Medina", "Carlos Ibarra", "Daniel Ortega", "Elena Ruiz" },
            InstructorSeleccionado = "Carlos Ibarra",
            Dias = new[]
            {
                new { Texto = "Lun", Seleccionado = false },
                new { Texto = "Mar", Seleccionado = true },
                new { Texto = "Mié", Seleccionado = false },
                new { Texto = "Jue", Seleccionado = true },
                new { Texto = "Vie", Seleccionado = false }
            },
            HoraInicio = new TimeSpan(16, 0, 0),
            HoraFin = new TimeSpan(17, 0, 0),
            CupoMaximo = "15",
            CostoMensual = "0",
            TieneErrorCosto = true,
            ErrorCosto = "El costo debe ser mayor a $0",
            IsBusy = false
        };
    }
    #endregion
}
