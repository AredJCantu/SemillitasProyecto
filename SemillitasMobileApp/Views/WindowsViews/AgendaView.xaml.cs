namespace SemillitasMobileApp.Views.WindowsViews;

/// <summary>
/// Code-behind de AgendaView (Windows). Sin lógica.
/// El bloque "DATOS DE PRUEBA" es solo para ver la pantalla con datos; bórralo
/// cuando exista el ViewModel real.
/// </summary>
public partial class AgendaView : ContentPage
{
    public AgendaView()
    {
        InitializeComponent();
        CargarDatosDePrueba(); // TODO: borrar cuando exista el ViewModel real
    }

    #region DATOS DE PRUEBA (borrar)
    private void CargarDatosDePrueba()
    {
        // Fila 1 = 3:00 pm, fila 2 = 3:30 ... fila 8 = 6:30 pm.
        // Columna 1-2 = lunes, 3-4 = martes, 5-6 = miércoles, 7-8 = jueves, 9-10 = viernes.
        BindingContext = new
        {
            Periodos = new[] { "Marzo 2026", "Febrero 2026" },
            PeriodoSeleccionado = "Marzo 2026",
            Eventos = new[]
            {
                new { Nombre = "Ballet inicial", Detalle = "4:00–5:00 · Lun", Categoria = "Arte", Fila = 3, Filas = 2, Columna = 1, Columnas = 2 },
                new { Nombre = "Futbol infantil", Detalle = "4:00–5:00 · Mar", Categoria = "Deportes", Fila = 3, Filas = 2, Columna = 3, Columnas = 1 },
                new { Nombre = "Guitarra", Detalle = "4:30–5:30 · Mar", Categoria = "Arte", Fila = 4, Filas = 2, Columna = 4, Columnas = 1 },
                new { Nombre = "Robótica", Detalle = "5:00–6:30 · Mié", Categoria = "Tecnologia", Fila = 5, Filas = 3, Columna = 5, Columnas = 2 },
                new { Nombre = "Futbol infantil", Detalle = "4:00–5:00 · Jue", Categoria = "Deportes", Fila = 3, Filas = 2, Columna = 7, Columnas = 2 },
                new { Nombre = "Pintura", Detalle = "4:00–5:30 · Vie", Categoria = "Arte", Fila = 3, Filas = 3, Columna = 9, Columnas = 1 },
                new { Nombre = "Inglés niños", Detalle = "3:00–4:00 · Vie", Categoria = "Idiomas", Fila = 1, Filas = 2, Columna = 10, Columnas = 1 }
            }
        };
    }
    #endregion
}
