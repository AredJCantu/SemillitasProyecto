using SemillasCore.DTOs;
using SemillitasProyecto.Entities;

namespace SemillitasProyecto.Mappers
{
    public static class PeriodosMappers
    {
        public static PeriodoDTO ToDTO(this Periodos entity) {
            PeriodoDTO dto = new() {
                Id= entity.Id,
                MesInicio = entity.MesInicio,
                MesFin=entity.MesFin,
                Año=entity.Año
            };
            return dto;
        }
        public static Periodos ToEntity(this CreatePeriodoDTO dto) {
            Periodos p = new() {
                MesInicio=dto.MesInicio,
                MesFin=dto.MesFin,
                Año=dto.Año
            };
            return p;
        }
        public static void ApplyTo(this EditPeriodoDTO dto, Periodos p) {
            p.MesInicio= dto.MesInicio;
            p.MesFin= dto.MesFin;
            p.Año = dto.Año;
        }
    }
}
