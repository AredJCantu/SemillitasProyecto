using Microsoft.EntityFrameworkCore;
using SemillitasProyecto.Data;
using SemillitasProyecto.Repositories;
using SemillitasProyecto.Services;

var builder = WebApplication.CreateBuilder(args);

var cs = builder.Configuration.GetConnectionString("Cadena");

builder.Services.AddControllers();
builder.Services.AddDbContext<SemillitasContext>(
    x=>x.UseMySql(cs,ServerVersion.AutoDetect(cs))
);

builder.Services.AddScoped(typeof(Repository<>));
builder.Services.AddScoped(typeof(PeriodoServices));
builder.Services.AddScoped(typeof(AlumnoServices));
builder.Services.AddScoped<TalleresServices>();
builder.Services.AddScoped<InscripcionesServices>();

var app = builder.Build();

using (var scoped=app.Services.CreateScope()) {
    var _context = scoped.ServiceProvider.GetRequiredService<SemillitasContext>();
    _context.Database.EnsureCreated();
}

    app.MapControllers();

app.Run();
