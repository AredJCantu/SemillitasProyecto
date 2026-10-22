using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Scaffolding.Internal;
using SemillitasProyecto.Entities;

namespace SemillitasProyecto.Data;

public partial class SemillitasContext : DbContext
{
    public SemillitasContext()
    {
    }

    public SemillitasContext(DbContextOptions<SemillitasContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Alumnos> Alumnos { get; set; }

    public virtual DbSet<Inscripciones> Inscripciones { get; set; }

    public virtual DbSet<Periodos> Periodos { get; set; }

    public virtual DbSet<Talleres> Talleres { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseMySql("server=localhost;password=root;user=root;port=3307;database=semillitas", Microsoft.EntityFrameworkCore.ServerVersion.Parse("8.0.43-mysql"));

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_0900_ai_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<Alumnos>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("alumnos");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Apellido)
                .HasMaxLength(100)
                .HasColumnName("apellido");
            entity.Property(e => e.ApellidoTutor)
                .HasMaxLength(100)
                .HasColumnName("apellido_tutor");
            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .HasColumnName("nombre");
            entity.Property(e => e.NombreTutor)
                .HasMaxLength(100)
                .HasColumnName("nombre_tutor");
            entity.Property(e => e.Telefono)
                .HasMaxLength(18)
                .HasColumnName("telefono");
        });

        modelBuilder.Entity<Inscripciones>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("inscripciones");

            entity.HasIndex(e => e.IdAlumno, "FK_Inscripciones_Alumno");

            entity.HasIndex(e => e.IdTaller, "FK_Inscripciones_Taller");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Debe)
                .HasDefaultValueSql("'0'")
                .HasColumnName("debe");
            entity.Property(e => e.IdAlumno).HasColumnName("id_alumno");
            entity.Property(e => e.IdTaller).HasColumnName("id_taller");

            entity.HasOne(d => d.IdAlumnoNavigation).WithMany(p => p.Inscripciones)
                .HasForeignKey(d => d.IdAlumno)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Inscripciones_Alumno");

            entity.HasOne(d => d.IdTallerNavigation).WithMany(p => p.Inscripciones)
                .HasForeignKey(d => d.IdTaller)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Inscripciones_Taller");
        });

        modelBuilder.Entity<Periodos>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("periodos");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Año).HasColumnName("año");
            entity.Property(e => e.MesFin)
                .HasColumnName("mes_fin");
            entity.Property(e => e.MesInicio)
                .HasColumnName("mes_inicio");
        });

        modelBuilder.Entity<Talleres>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("talleres");

            entity.HasIndex(e => e.IdPeriodo, "Fk_Talleres_Periodos");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Categoria)
                .HasColumnType("enum('arte','deportes','tecnología')")
                .HasColumnName("categoria");
            entity.Property(e => e.Costo)
                .HasPrecision(5, 2)
                .HasColumnName("costo");
            entity.Property(e => e.HoraFin).HasColumnName("hora_fin");
            entity.Property(e => e.HoraFinSabado).HasColumnName("hora_fin_sabado");
            entity.Property(e => e.HoraInicio).HasColumnName("hora_inicio");
            entity.Property(e => e.HoraInicioSabado).HasColumnName("hora_inicio_sabado");
            entity.Property(e => e.IdPeriodo).HasColumnName("id_periodo");
            entity.Property(e => e.Nombre)
                .HasMaxLength(200)
                .HasColumnName("nombre");
            entity.Property(e => e.NombreInstructor)
                .HasMaxLength(150)
                .HasColumnName("nombre_instructor");

            entity.HasOne(d => d.IdPeriodoNavigation).WithMany(p => p.Talleres)
                .HasForeignKey(d => d.IdPeriodo)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Fk_Talleres_Periodos");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
