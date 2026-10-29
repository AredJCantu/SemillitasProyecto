using Microsoft.EntityFrameworkCore;
using SemillitasProyecto.Data;
using SemillitasProyecto.Repositories;
using SemillitasProyecto.Services;
using SemillitasProyecto.Validators;

var builder = WebApplication.CreateBuilder(args);

var cs = builder.Configuration.GetConnectionString("Cadena");

builder.Services.AddControllers();
builder.Services.AddDbContext<SemillitasContext>(
    x=>x.UseMySql(cs,ServerVersion.AutoDetect(cs))
);
//Repository
builder.Services.AddScoped(typeof(Repository<>));
//Services
builder.Services.AddTransient(typeof(PeriodoServices));
builder.Services.AddTransient(typeof(AlumnoServices));
builder.Services.AddTransient<TalleresServices>();
builder.Services.AddTransient<InscripcionesServices>();
//Validators
builder.Services.AddTransient<PeriodoExistenteValidator>();
builder.Services.AddTransient<PeriodoExistenteEdit>();
builder.Services.AddTransient<PeriodoConTalleresValidator>();
builder.Services.AddTransient<AlumnoExistenteValidator>();

var app = builder.Build();

using (var scoped=app.Services.CreateScope()) {
    var _context = scoped.ServiceProvider.GetRequiredService<SemillitasContext>();
    _context.Database.EnsureCreated();
}

    app.MapControllers();

app.Run();
