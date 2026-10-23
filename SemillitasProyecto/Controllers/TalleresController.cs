using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SemillasCore.DTOs;
using SemillitasProyecto.Services;

namespace SemillitasProyecto.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TalleresController : ControllerBase
    {
        public TalleresController(TalleresServices servicio)
        {
            Servicio = servicio;
        }

        public TalleresServices Servicio { get; }

        [HttpGet]
        public IActionResult GetAll() {
            return Ok(Servicio.GetAll());
        }
        [HttpGet("{id:int}")]
        public IActionResult GetDatosById(int id) {
            var data = Servicio.GetDatosById(id);
            if (data != null)
            {
                return Ok(data);
            }
            return NotFound();
        }
        [HttpPost]
        public IActionResult Post(CreateTallerDTO dto) {
            var data = Servicio.Create(dto);
            if (data != null)
            {
                return Created("api/talleres",data);
            }
            return BadRequest();
        }
        [HttpPut("{id:int}")]
        public IActionResult Put(EditTallerDTO dto,int id) {
            var data = Servicio.Update(dto, id);
            if (data != null)
            {
                return Ok(data);
            }
            return NotFound();
        }

        [HttpDelete("{id:int}")]
        public IActionResult Delete(int id) {
            if(Servicio.Delete(id))
            {
                return Ok();
            }
            return NotFound();
        }

    }
}
