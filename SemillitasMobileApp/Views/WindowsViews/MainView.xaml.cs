namespace SemillitasMobileApp.Views.WindowsViews;

/// <summary>
/// Code-behind de MainView (Windows). Sin lógica.
///
/// IMPORTANTE: el bloque "DATOS DE PRUEBA" solo existe para que puedas VER la
/// pantalla con datos mientras no hay ViewModel. En cuanto tu compañero te dé
/// el ViewModel real, borra CargarDatosDePrueba() y asigna el BindingContext
/// como corresponda (o déjalo vacío si lo asigna él).
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
            SoloConCupo = false,
            Categorias = new[]
            {
                new { Texto = "Todas" }, new { Texto = "Arte" }, new { Texto = "Deportes" },
                new { Texto = "Tecnología" }, new { Texto = "Idiomas" }
            },
            ConteoConCupo = 3,
            ConteoPocos = 3,
            ConteoLlenos = 1,
            IsBusy = false,
            ResumenListado = "Mostrando 7 de 14 talleres",
            Talleres = new[]
            {
                new { Nombre = "Ballet inicial", Meta = "Arte · Mtra. Daniela Ruiz", Horario = "Lun y Mié · 4:00–5:00 pm", Ocupacion = "12/12", Porcentaje = 1.0, Badge = "Lleno", Tono = "Rojo", TieneCupo = false },
                new { Nombre = "Futbol infantil", Meta = "Deportes · Prof. Luis Ortega", Horario = "Mar y Jue · 4:00–5:00 pm", Ocupacion = "9/15", Porcentaje = 0.6, Badge = "Quedan 6", Tono = "Verde", TieneCupo = true },
                new { Nombre = "Guitarra", Meta = "Arte · Mtro. Iván Salas", Horario = "Martes · 4:30–5:30 pm", Ocupacion = "7/8", Porcentaje = 0.875, Badge = "Queda 1", Tono = "Ambar", TieneCupo = true },
                new { Nombre = "Robótica", Meta = "Tecnología · Ing. Paola Mena", Horario = "Miércoles · 5:00–6:30 pm", Ocupacion = "9/10", Porcentaje = 0.9, Badge = "Queda 1", Tono = "Ambar", TieneCupo = true },
                new { Nombre = "Pintura", Meta = "Arte · Mtra. Sofía Nava", Horario = "Viernes · 4:00–5:30 pm", Ocupacion = "4/12", Porcentaje = 0.33, Badge = "Quedan 8", Tono = "Verde", TieneCupo = true },
                new { Nombre = "Inglés para niños", Meta = "Idiomas · Miss Karen Lugo", Horario = "Sábado · 9:00–10:30 am", Ocupacion = "10/16", Porcentaje = 0.625, Badge = "Quedan 6", Tono = "Verde", TieneCupo = true },
                new { Nombre = "Futbol 9–12 años", Meta = "Deportes · Prof. Luis Ortega", Horario = "Sábado · 10:00–11:30 am", Ocupacion = "13/15", Porcentaje = 0.87, Badge = "Quedan 2", Tono = "Ambar", TieneCupo = true }
            }
        };
    }
    #endregion
}
