using SemillitasProyecto.Data;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
var app = builder.Build();

using (var scoped=app.Services.CreateScope()) {
    var _context = scoped.ServiceProvider.GetRequiredService<SemillitasContext>();
    _context.Database.EnsureCreated();
}

    app.MapControllers();

app.Run();
