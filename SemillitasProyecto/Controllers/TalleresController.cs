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
    public class TalleresController : ControllerBase
    {
        private readonly CreateTallerValidator createValidator;
        private readonly EditTallerValidator editValidator;
        private readonly TallerConInscripciones deleteValidator;

        public TalleresController(TalleresServices servicio,CreateTallerValidator createValidator, TallerExistenteEditValidator editValidator,
            TallerConInscripciones deleteValidator)
        {
            Servicio = servicio;
            this.createValidator = createValidator;
            this.editValidator = editValidator;
            this.deleteValidator = deleteValidator;
        }

        public TalleresServices Servicio { get; }

        [HttpGet]
        public IActionResult GetAll() {
            return Ok(Servicio.GetAll());
        }
        [HttpGet("{id:int}")]
        public IActionResult GetById(int id) {
            var data = Servicio.GetById(id);
            if (data != null) {
                return Ok(data);
            }
            return NotFound();
        }
        [HttpGet("datos/{id:int}")]
        public IActionResult GetDatosById(int id) {
            var data = Servicio.GetDatosById(id);
            if (data != null)
            {
                return Ok(data);
            }
            return NotFound();
        }
        [HttpGet("populares/")]
        public IActionResult GetPopulares() {
            var data = Servicio.GetPopulares();
            return Ok(data);
        }
        [HttpPost]
        public IActionResult Post(CreateTallerDTO dto) {
            var resultado = createValidator.IsValid(dto);
            if (resultado.EsValido)
            {
                var data = Servicio.Create(dto);
                return Created("api/talleres", data);
            }
            return BadRequest(resultado.Errores);
        }
        [HttpPut("{id:int}")]
        public IActionResult Put(EditTallerDTO dto,int id) {
            dto.Id = id;
            var resultado = editValidator.IsValid(dto);
            if (resultado.EsValido)
            {
                var data = Servicio.Update(dto);
                if (data != null)
                {
                    return Ok(data);
                }
                return NotFound();
            }
            return BadRequest(resultado.Errores);
        }

        [HttpDelete("{id:int}")]
        public IActionResult Delete(int id) {
            var resultado = deleteValidator.IsValid(id);
            if (resultado.EsValido)
            {
                if (Servicio.Delete(id))
                {
                    return Ok();
                }
                return NotFound();
            }
            return BadRequest(resultado.Errores);
        }
        

    }
}
