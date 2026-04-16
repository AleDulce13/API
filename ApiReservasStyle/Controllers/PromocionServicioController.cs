using Aplicacion_ReservasStyle.DTOs;
using Aplicacion_ReservasStyle.Services;
using Microsoft.AspNetCore.Mvc;

namespace ApiReservasStyle.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PromocionServicioController : ControllerBase
    {
        private readonly PromocionServicioService _service;

        public PromocionServicioController(PromocionServicioService service)
        {
            _service = service;
        }

        // GET
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            return Ok(await _service.ObtenerTodos());
        }

        // POST
        [HttpPost]
        public async Task<IActionResult> Post(PromocionServicioDTO dto)
        {
            var result = await _service.Crear(dto);
            return Ok(result);
        }

        // DELETE
        [HttpDelete]
        public async Task<IActionResult> Delete(int idPromocion, int idServicioSucursal)
        {
            var ok = await _service.Eliminar(idPromocion, idServicioSucursal);

            if (!ok)
                return NotFound();

            return NoContent();
        }
    }
}
