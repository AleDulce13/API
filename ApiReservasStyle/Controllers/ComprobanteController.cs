using Aplicacion_ReservasStyle.DTOs;
using Aplicacion_ReservasStyle.Services;
using Microsoft.AspNetCore.Mvc;

namespace ApiReservasStyle.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ComprobanteController : ControllerBase
    {
        private readonly ComprobanteService _service;

        public ComprobanteController(ComprobanteService service)
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
            var comprobante = await _service.GetById(id);

            if (comprobante == null)
                return NotFound();

            return Ok(comprobante);
        }

        // CREATE
        [HttpPost]
        public async Task<IActionResult> Add([FromBody] ComprobanteDTO dto)
        {
            await _service.Add(dto);
            return Ok("Comprobante creado");
        }

        // UPDATE
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] ComprobanteDTO dto)
        {
            await _service.Update(id, dto);
            return Ok("Comprobante actualizado");
        }

        // DELETE
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _service.Delete(id);
            return Ok("Comprobante eliminado");
        }
    }
}
