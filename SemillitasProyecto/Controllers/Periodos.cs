using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using SemillasCore.DTOs;
using SemillitasProyecto.Services;
using SemillitasProyecto.Validators;

namespace SemillitasProyecto.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PeriodosController : ControllerBase
    {
        public PeriodosController(PeriodoServices service,PeriodoExistenteValidator createValidator,PeriodoExistenteEdit editValidator
            ,PeriodoConTalleresValidator deleteValidator)
        {
            Service = service;
            CreateValidator = createValidator;
            EditValidator = editValidator;
            DeleteValidator = deleteValidator;
        }

        public PeriodoServices Service { get; }
        public PeriodoExistenteValidator CreateValidator { get; }
        public PeriodoExistenteEdit EditValidator { get; }
        public PeriodoConTalleresValidator DeleteValidator { get; }

        [HttpGet]
        public IActionResult GetAll() {
            var data = Service.GetAll();
            if (data != null)
            {
                return Ok(data);
            }
            return BadRequest();
        }
        [HttpGet("{id:int}")]
        public IActionResult GetById(int id) {
            var data = Service.GetById(id);
            if (data != null)
            {
                return Ok(data);
            }
            return NotFound();
        }

        [HttpPost]
        public IActionResult Post(CreatePeriodoDTO dto) {
            var resultado = CreateValidator.IsValid(dto);
            if (resultado.EsValido)
            {
                var data = Service.Create(dto);
                return Created("api/periodos", data);
            }
            return BadRequest(resultado.Errores);
        }

        [HttpPut("{id:int}")]
        public IActionResult Put(EditPeriodoDTO dto,int id) {
            dto.Id = id;
            var resultado = EditValidator.IsValid(dto);
            if (resultado.EsValido)
            {
                var data = Service.Update(dto);
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
            var resultado = DeleteValidator.IsValid(id);
            if (resultado.EsValido)
            {
                if (Service.Delete(id))
                {
                    return Ok();
                }
                return NotFound();
            }
            return BadRequest(resultado.Errores);
        }

    }
}
