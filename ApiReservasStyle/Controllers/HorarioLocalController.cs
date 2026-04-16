using Aplicacion_ReservasStyle.DTOs;
using Aplicacion_ReservasStyle.Services;
using Microsoft.AspNetCore.Mvc;

namespace ApiReservasStyle.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HorarioLocalController : ControllerBase
    {
        private readonly HorarioLocalService _service;

        public HorarioLocalController(HorarioLocalService service)
        {
            _service = service;
        }

        // GET ALL
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _service.GetAll());
        }

        // GET BY ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var data = await _service.GetById(id);

            if (data == null)
                return NotFound();

            return Ok(data);
        }

        // CREATE
        [HttpPost]
        public async Task<IActionResult> Add([FromBody] HorarioLocalDTO dto)
        {
            await _service.Add(dto);
            return Ok("Horario creado");
        }

        // UPDATE
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] HorarioLocalDTO dto)
        {
            await _service.Update(id, dto);
            return Ok("Horario actualizado");
        }

        // DELETE
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _service.Delete(id);
            return Ok("Horario eliminado");
        }
    }
}
