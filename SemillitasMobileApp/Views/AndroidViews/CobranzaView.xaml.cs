namespace SemillitasMobileApp.Views.AndroidViews;

/// <summary>
/// Code-behind de CobranzaView (Android). Sin lógica.
/// El bloque "DATOS DE PRUEBA" es solo para ver la pantalla con datos; bórralo
/// cuando exista el ViewModel real.
/// </summary>
public partial class CobranzaView : ContentPage
{
    public CobranzaView()
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
            Cobrado = "$38,450",
            PorCobrar = "$9,300",
            Vencidos = "14 tutores",
            IsBusy = false,
            Filtros = new[]
            {
                new { Texto = "Todos" }, new { Texto = "Vencidos" },
                new { Texto = "Pendientes" }, new { Texto = "Pagados" }
            },
            Pagos = new[]
            {
                new { Tutor = "Ana Hernández", Alumno = "Mateo", Talleres = "Futbol infantil, Pintura", Total = "$1,050", Estado = "Vencido", Tono = "Rojo", FechaPago = "" },
                new { Tutor = "Jorge Salazar", Alumno = "Emiliano", Talleres = "Guitarra", Total = "$600", Estado = "Vencido", Tono = "Rojo", FechaPago = "" },
                new { Tutor = "Mariana Torres", Alumno = "Sebastián y Camila", Talleres = "Robótica, Inglés", Total = "$1,450", Estado = "Pendiente", Tono = "Ambar", FechaPago = "" },
                new { Tutor = "Luis Ramírez", Alumno = "Diego", Talleres = "Futbol infantil", Total = "$550", Estado = "Pendiente", Tono = "Ambar", FechaPago = "" },
                new { Tutor = "Patricia Vega", Alumno = "Renata", Talleres = "Ballet inicial, Pintura", Total = "$1,150", Estado = "Pagado", Tono = "Verde", FechaPago = "Pagó el 3 mar" },
                new { Tutor = "Sofía Cantú", Alumno = "Isabela", Talleres = "Inglés para niños", Total = "$700", Estado = "Pagado", Tono = "Verde", FechaPago = "Pagó el 5 mar" }
            }
        };
    }
    #endregion
}
