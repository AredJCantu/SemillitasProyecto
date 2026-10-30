using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SemillasCore.DTOs;
using SemillasCore.Validators;
using SemillitasProyecto.Services;
using SemillitasProyecto.Validators;

namespace SemillitasProyecto.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AlumnosController : ControllerBase
    {
        private readonly CreateAlumnoValidator createAlumnoValidator;

        public AlumnosController(AlumnoServices servicio,CreateAlumnoValidator CreateAlumnoValidator,AlumnoExistenteValidator EditAlumnoValidator, AlumnoConInscripcionValidator deleteAlumnoValidator)
        {
            Servicio = servicio;
            createAlumnoValidator = CreateAlumnoValidator;
            this.EditAlumnoValidator = EditAlumnoValidator;
            DeleteAlumnoValidator = deleteAlumnoValidator;
        }

        public AlumnoServices Servicio { get; }
        public AlumnoExistenteValidator EditAlumnoValidator { get; }
        public AlumnoConInscripcionValidator DeleteAlumnoValidator { get; }

        [HttpGet]
        public IActionResult GetAll() {
            var data = Servicio.GetAll();
            if (data != null)
            {
                return Ok(data);
            }
            return BadRequest();
        }
        [HttpGet("{id:int}")]
        public IActionResult GetDataById(int id) {
            var data = Servicio.GetDatosById(id);
            if (data != null)
            {
                return Ok(data);
            }
            return NotFound();
        }

        [HttpPost]
        public IActionResult Post(CreateAlumnoDTO dto) {
            var result = createAlumnoValidator.IsValid(dto);
            if (result.EsValido) {
                var alumno=Servicio.Create(dto);
                return Ok(alumno);
            }
            return BadRequest(result.Errores);
        }
        [HttpPut("{id:int}")]
        public IActionResult Put(EditAlumnoDTO dto,int id) {
            dto.Id = id;
            var result = EditAlumnoValidator.IsValid(dto);
            if (result.EsValido)
            {
                var data = Servicio.Update(dto, id);
                if (data != null)
                {
                    return Ok(data);
                }
                return BadRequest();
            }
            return BadRequest(result.Errores);
        }

        [HttpDelete("{id:int}")]
        public IActionResult Delete(int id) {
            var result = DeleteAlumnoValidator.IsValid(id);
            if (result.EsValido)
            {
                if (Servicio.Delete(id))
                {
                    return NoContent();
                }
                return NotFound();
            }
            return BadRequest(result.Errores);
        }

    }
}
