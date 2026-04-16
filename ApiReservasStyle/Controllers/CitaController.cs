using Aplicacion_ReservasStyle.DTOs;
using Aplicacion_ReservasStyle.Services;
using Microsoft.AspNetCore.Mvc;

namespace ApiReservasStyle.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CitaController : ControllerBase
    {
        private readonly CitaService _service;

        public CitaController(CitaService service)
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
            var cita = await _service.GetById(id);

            if (cita == null)
                return NotFound();

            return Ok(cita);
        }

        // CREATE
        [HttpPost]
        public async Task<IActionResult> Add([FromBody] CitaDTO dto)
        {
            await _service.Add(dto);
            return Ok("Cita creada");
        }

        // UPDATE
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] CitaDTO dto)
        {
            await _service.Update(id, dto);
            return Ok("Cita actualizada");
        }

        // DELETE
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _service.Delete(id);
            return Ok("Cita eliminada");
        }
    }
}
