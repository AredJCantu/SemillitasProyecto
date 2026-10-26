namespace SemillitasMobileApp.Views.WindowsViews;

/// <summary>
/// Code-behind de InscripcionView (Windows). Sin lógica.
/// El bloque "DATOS DE PRUEBA" es solo para ver la pantalla con datos; bórralo
/// cuando exista el ViewModel real.
/// </summary>
public partial class InscripcionView : ContentPage
{
    public InscripcionView()
    {
        InitializeComponent();
        CargarDatosDePrueba(); // TODO: borrar cuando exista el ViewModel real
    }

    #region DATOS DE PRUEBA (borrar)
    private void CargarDatosDePrueba()
    {
        BindingContext = new
        {
            SubtituloInscripcion = "Período Marzo 2026 · Mateo Hernández, tutor Ana Hernández",
            NombreAlumno = "Mateo",
            TextoAgregados = "2 agregados",
            Periodo = "Marzo 2026",
            Total = "$1,050",
            HayError = true,
            MensajeError = "No se puede agregar Guitarra. Su horario (martes 4:30–5:30 pm) se solapa con Futbol infantil (martes 4:00–5:00 pm).",
            Pasos = new[]
            {
                new { Numero = 1, Titulo = "Tutor", Estado = "Hecho" },
                new { Numero = 2, Titulo = "Niño", Estado = "Hecho" },
                new { Numero = 3, Titulo = "Talleres", Estado = "Actual" },
                new { Numero = 4, Titulo = "Resumen", Estado = "Pendiente" },
                new { Numero = 5, Titulo = "Pago", Estado = "Pendiente" }
            },
            Talleres = new[]
            {
                new { Nombre = "Futbol infantil", Horario = "Mar y Jue · 4:00–5:00 pm", Badge = "Quedan 6", Tono = "Verde", Accion = "Agregado" },
                new { Nombre = "Guitarra", Horario = "Martes · 4:30–5:30 pm", Badge = "Queda 1", Tono = "Ambar", Accion = "Choque" },
                new { Nombre = "Ballet inicial", Horario = "Lun y Mié · 4:00–5:00 pm", Badge = "Lleno", Tono = "Rojo", Accion = "Lleno" },
                new { Nombre = "Robótica", Horario = "Miércoles · 5:00–6:30 pm", Badge = "Queda 1", Tono = "Ambar", Accion = "Disponible" },
                new { Nombre = "Pintura", Horario = "Viernes · 4:00–5:30 pm", Badge = "Quedan 8", Tono = "Verde", Accion = "Agregado" },
                new { Nombre = "Inglés para niños", Horario = "Sábado · 9:00–10:30 am", Badge = "Quedan 6", Tono = "Verde", Accion = "Disponible" }
            },
            Resumen = new[]
            {
                new { Nombre = "Futbol infantil", Horario = "Mar y Jue · 4:00–5:00 pm", Precio = "$550" },
                new { Nombre = "Pintura", Horario = "Viernes · 4:00–5:30 pm", Precio = "$500" }
            }
        };
    }
    #endregion
}
