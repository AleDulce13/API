using System.Security.Claims;
using Aplicacion_ReservasStyle.DTOs;
using Aplicacion_ReservasStyle.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ApiReservasStyle.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/devices")]
    public class DeviceController : ControllerBase
    {
        private readonly IDeviceRegistrationService _service;
        public DeviceController(IDeviceRegistrationService service) => _service = service;

        [HttpPost("register")]
        public async Task<IActionResult> Register(DeviceRegistrationDTO dto)
        {
            if (!TryGetUserId(out var userId)) return Unauthorized();
            try
            {
                await _service.RegisterAsync(userId, dto.Token, dto.Platform);
                return NoContent();
            }
            catch (ArgumentException exception)
            {
                return BadRequest(new { error = exception.Message });
            }
        }

        [HttpDelete("register")]
        public async Task<IActionResult> Remove([FromQuery] string token)
        {
            if (!TryGetUserId(out var userId)) return Unauthorized();
            await _service.RemoveAsync(userId, token);
            return NoContent();
        }

        private bool TryGetUserId(out int userId) => int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out userId);
    }
}
