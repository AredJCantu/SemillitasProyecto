namespace SemillitasMobileApp.Views.WindowsViews;

/// <summary>
/// Code-behind de TalleresView (Windows). Sin lógica.
/// El bloque "DATOS DE PRUEBA" es solo para ver la pantalla con datos; bórralo
/// cuando exista el ViewModel real.
/// </summary>
public partial class TalleresView : ContentPage
{
    public TalleresView()
    {
        InitializeComponent();
        CargarDatosDePrueba(); // TODO: borrar cuando exista el ViewModel real
    }

    #region DATOS DE PRUEBA (borrar)
    private void CargarDatosDePrueba()
    {
        BindingContext = new
        {
            TextoBusqueda = "",
            IsBusy = false,
            Talleres = new[]
            {
                new { Nombre = "Ballet inicial", Categoria = "Arte", Instructor = "Laura Medina", Horario = "Lun y Mié · 4:00–5:00 pm", Cupo = "12", Costo = "$600" },
                new { Nombre = "Futbol infantil", Categoria = "Deportes", Instructor = "Carlos Ibarra", Horario = "Mar y Jue · 4:00–5:00 pm", Cupo = "15", Costo = "$550" },
                new { Nombre = "Guitarra", Categoria = "Arte", Instructor = "Daniel Ortega", Horario = "Martes · 4:30–5:30 pm", Cupo = "8", Costo = "$600" },
                new { Nombre = "Robótica", Categoria = "Tecnología", Instructor = "Elena Ruiz", Horario = "Miércoles · 5:00–6:30 pm", Cupo = "10", Costo = "$750" },
                new { Nombre = "Pintura", Categoria = "Arte", Instructor = "Marta Solís", Horario = "Viernes · 4:00–5:30 pm", Cupo = "12", Costo = "$500" },
                new { Nombre = "Inglés para niños", Categoria = "Idiomas", Instructor = "Paul Evans", Horario = "Sábado · 9:00–10:30 am", Cupo = "16", Costo = "$700" }
            }
        };
    }
    #endregion
}
