using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SemillasCore.DTOs;
using SemillasCore.Validators;
using SemillitasProyecto.Services;

namespace SemillitasProyecto.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InscripcionesController : ControllerBase
    {
        private readonly SinCupoValidator sincupoValidator;

        public InscripcionesController(InscripcionesServices servicio,SinCupoValidator sincupoValidator,EmpalmeValidator empalmeValidator)
        {
            Servicio = servicio;
            this.sincupoValidator = sincupoValidator;
            EmpalmeValidator = empalmeValidator;
        }

        public InscripcionesServices Servicio { get; }
        public EmpalmeValidator EmpalmeValidator { get; }

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
            var res = sincupoValidator.IsValid(dto);
            var res2 = EmpalmeValidator.IsValid(dto);
            if (res.EsValido&&res2.EsValido)
            {
                    var data = Servicio.Create(dto);
                    return Created("api/inscripciones", data);
            }
            foreach (var e in res.Errores)
                res2.Errores.Add(e);
            return BadRequest(res2.Errores);
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
