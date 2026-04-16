using Aplicacion_ReservasStyle.DTOs;
using Aplicacion_ReservasStyle.Services;
using Microsoft.AspNetCore.Mvc;

namespace ApiReservasStyle.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LogsController : ControllerBase
    {
        private readonly LogService _service;

        public LogsController(LogService service)
        {
            _service = service;
        }

        // GET
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            return Ok(await _service.ObtenerTodos());
        }

        // GET
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var log = await _service.ObtenerPorId(id);

            if (log == null)
                return NotFound();

            return Ok(log);
        }

        // POST
        [HttpPost]
        public async Task<IActionResult> Post(LogDTO dto)
        {
            var result = await _service.Crear(dto);
            return Ok(result);
        }
    }
    
}
