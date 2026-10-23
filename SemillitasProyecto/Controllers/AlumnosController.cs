using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SemillasCore.DTOs;
using SemillitasProyecto.Services;

namespace SemillitasProyecto.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AlumnosController : ControllerBase
    {
        public AlumnosController(AlumnoServices servicio)
        {
            Servicio = servicio;
        }

        public AlumnoServices Servicio { get; }

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
            var data = Servicio.Create(dto);
            if (data != null) {
                return Created("api/alumnos",dto);
            }
            return BadRequest();
        }
        [HttpPut("{id:int}")]
        public IActionResult Put(EditAlumnoDTO dto,int id) {
            var data = Servicio.Update(dto, id);
            if (data != null)
            {
                return Ok(data);
            }
            return BadRequest();
        }

        [HttpDelete("{id:int}")]
        public IActionResult Delete(int id) {
            if (Servicio.Delete(id)){
                return NoContent();
            }
            return NotFound();
        }

    }
}
