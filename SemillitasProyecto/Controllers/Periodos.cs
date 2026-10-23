using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using SemillasCore.DTOs;
using SemillitasProyecto.Services;

namespace SemillitasProyecto.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PeriodosController : ControllerBase
    {
        public PeriodosController(PeriodoServices service)
        {
            Service = service;
        }

        public PeriodoServices Service { get; }

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
            var data = Service.Create(dto);
            if (data != null)
            {
                return Created("api/periodos",data);
            }
            return BadRequest();
        }
        [HttpPut("{id:int}")]
        public IActionResult Put(EditPeriodoDTO dto,int id) {
            var data = Service.Update(dto,id);
            if (data != null)
            {
                return Ok(data);
            }
            return BadRequest();
        }
        [HttpDelete("{id:int}")]
        public IActionResult Delete(int id) {
            if (Service.Delete(id))
            {
                return Ok();
            }
            return NotFound();
        }

    }
}
