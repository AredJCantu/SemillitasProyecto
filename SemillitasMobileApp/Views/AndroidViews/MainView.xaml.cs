namespace SemillitasMobileApp.Views.AndroidViews;

/// <summary>
/// Code-behind de MainView (Android). Sin lógica.
/// El bloque "DATOS DE PRUEBA" es solo para ver la pantalla con datos; bórralo
/// cuando exista el ViewModel real.
/// </summary>
public partial class MainView : ContentPage
{
    public MainView()
    {
        InitializeComponent();
        CargarDatosDePrueba(); // TODO: borrar cuando exista el ViewModel real
    }

    #region DATOS DE PRUEBA (borrar)
    private void CargarDatosDePrueba()
    {
        BindingContext = new
        {
            Periodos = new[] { "Marzo 2026", "Febrero 2026" },
            PeriodoSeleccionado = "Marzo 2026",
            TextoBusqueda = "",
            ConteoConCupo = 4,
            ConteoPocos = 2,
            ConteoLlenos = 1,
            IsBusy = false,
            Categorias = new[]
            {
                new { Texto = "Todas" }, new { Texto = "Arte" }, new { Texto = "Deportes" },
                new { Texto = "Tecnología" }, new { Texto = "Idiomas" }
            },
            Talleres = new[]
            {
                new { Nombre = "Ballet inicial", Meta = "Arte · Laura Medina", Horario = "Lun y Mié · 4:00–5:00 pm", Ocupacion = "12 de 12 inscritos", Porcentaje = 1.0, Badge = "Lleno", Tono = "Rojo" },
                new { Nombre = "Futbol infantil", Meta = "Deportes · Carlos Ibarra", Horario = "Mar y Jue · 4:00–5:00 pm", Ocupacion = "9 de 15 inscritos", Porcentaje = 0.6, Badge = "Quedan 6", Tono = "Verde" },
                new { Nombre = "Guitarra", Meta = "Arte · Daniel Ortega", Horario = "Martes · 4:30–5:30 pm", Ocupacion = "7 de 8 inscritos", Porcentaje = 0.875, Badge = "Queda 1", Tono = "Ambar" },
                new { Nombre = "Robótica", Meta = "Tecnología · Elena Ruiz", Horario = "Miércoles · 5:00–6:30 pm", Ocupacion = "9 de 10 inscritos", Porcentaje = 0.9, Badge = "Queda 1", Tono = "Ambar" },
                new { Nombre = "Pintura", Meta = "Arte · Marta Solís", Horario = "Viernes · 4:00–5:30 pm", Ocupacion = "4 de 12 inscritos", Porcentaje = 0.33, Badge = "Quedan 8", Tono = "Verde" },
                new { Nombre = "Inglés para niños", Meta = "Idiomas · Paul Evans", Horario = "Sábado · 9:00–10:30 am", Ocupacion = "10 de 16 inscritos", Porcentaje = 0.625, Badge = "Quedan 6", Tono = "Verde" }
            }
        };
    }
    #endregion
}
