using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SemillasCore.DTOs;
using SemillitasProyecto.Services;

namespace SemillitasProyecto.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InscripcionesController : ControllerBase
    {
        public InscripcionesController(InscripcionesServices servicio)
        {
            Servicio = servicio;
        }

        public InscripcionesServices Servicio { get; }

        [HttpGet]
        public IActionResult GetAll() {
            var data = Servicio.GetAll();
            return Ok(data);
        }
        [HttpGet("{id:int}")]
        public IActionResult GetById(int id) {
            var data = Servicio.GetById(id);
            if (data != null)
            {
                return Ok(data);
            }
            return NotFound();
        }
        [HttpPost] 
        public IActionResult Post(CreateInscripcionesDTO dto) {
            var data = Servicio.Create(dto);
            if (data != null)
            {
                return Created("api/inscripciones",data);
            }
            return BadRequest();
        }
        [HttpPut("{id:int}")]
        public IActionResult Put(EditInscripcionesDTO dto,int id) {
            var data = Servicio.Update(dto, id);
            if (data != null)
            {
                return Ok(data);
            }
            return BadRequest();
        }

        [HttpDelete("{id:int}")]
        public IActionResult Delete(int id) {
            if (Servicio.Delete(id)) {
                return Ok();
            }
            return NotFound();
        }

    }
}
