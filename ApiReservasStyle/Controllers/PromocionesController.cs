using Aplicacion_ReservasStyle.DTOs;
using Aplicacion_ReservasStyle.Services;
using Microsoft.AspNetCore.Mvc;

namespace ApiReservasStyle.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PromocionesController : ControllerBase
    {
        private readonly PromocionService _service;

        public PromocionesController(PromocionService service)
        {
            _service = service;
        }

        // GET
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            return Ok(await _service.ObtenerTodas());
        }

        // GET
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var promo = await _service.ObtenerPorId(id);

            if (promo == null)
                return NotFound();

            return Ok(promo);
        }

        // POST
        [HttpPost]
        public async Task<IActionResult> Post(PromocionDTO dto)
        {
            var result = await _service.Crear(dto);
            return Ok(result);
        }

        // PUT
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, PromocionDTO dto)
        {
            var ok = await _service.Actualizar(id, dto);

            if (!ok)
                return NotFound();

            return NoContent();
        }

        // DELETE
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var ok = await _service.Eliminar(id);

            if (!ok)
                return NotFound();

            return NoContent();
        }
    }
}
