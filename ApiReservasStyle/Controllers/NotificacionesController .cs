using Aplicacion_ReservasStyle.DTOs;
using Aplicacion_ReservasStyle.Services;
using Microsoft.AspNetCore.Mvc;

namespace ApiReservasStyle.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificacionesController : ControllerBase
    {
        private readonly NotificacionService _service;

        public NotificacionesController(NotificacionService service)
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
            var notificacion = await _service.ObtenerPorId(id);

            if (notificacion == null)
                return NotFound();

            return Ok(notificacion);
        }

        // POST
        [HttpPost]
        public async Task<IActionResult> Post(NotificacionDTO dto)
        {
            var result = await _service.Crear(dto);
            return Ok(result);
        }

        // PUT
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, NotificacionDTO dto)
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
