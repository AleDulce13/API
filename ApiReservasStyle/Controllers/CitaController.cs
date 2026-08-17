using Aplicacion_ReservasStyle.DTOs;
using Aplicacion_ReservasStyle.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ApiReservasStyle.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class CitaController : ControllerBase
    {
        private readonly CitaService _service;

        public CitaController(CitaService service)
        {
            _service = service;
        }

        private bool TryGetUserId(out int userId)
        {
            return int.TryParse(
                User.FindFirstValue(
                    ClaimTypes.NameIdentifier
                ),
                out userId
            );
        }

        // GET ALL

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            if (!TryGetUserId(out var userId))
                return Unauthorized();

            var esAdmin =
                User.IsInRole("Admin");

            var esCliente =
                User.IsInRole("Cliente");

            return Ok(
                await _service.GetAllForUser(
                    userId,
                    esAdmin,
                    esCliente
                )
            );
        }

        // GET BY ID

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            if (!TryGetUserId(out var userId))
                return Unauthorized();

            var cita =
                await _service.GetByIdForUser(
                    id,
                    userId,
                    User.IsInRole("Admin")
                );

            if (cita == null)
                return NotFound();

            return Ok(cita);
        }

        // CREATE

        [HttpPost]
        public async Task<IActionResult> Add(
            [FromBody] CitaDTO dto)
        {
            if (!TryGetUserId(out var userId))
                return Unauthorized();

            if (
                !User.IsInRole("Admin") &&
                dto.IdCliente != userId
            )
            {
                return Forbid();
            }

            await _service.Add(dto);

            return Ok("Cita creada");
        }

        [HttpGet("empleados-servicio/{idServicioSucursal}")]
        public async Task<IActionResult> GetEmpleadosPorServicioSucursal(
            int idServicioSucursal)
        {
            var empleados = await _service.GetEmpleadosPorServicioSucursal(
                idServicioSucursal);

            return Ok(empleados);
        }

        // HORARIOS DISPONIBLES

        [HttpGet("horarios-disponibles")]
        public async Task<IActionResult>
            GetHorariosDisponibles(
                [FromQuery] int empleadoId,
                [FromQuery] int servicioSucursalId,
                [FromQuery] DateTime fecha)
        {
            return Ok(
                await _service.GetHorariosDisponibles(
                    empleadoId,
                    servicioSucursalId,
                    fecha
                )
            );
        }

        // UPDATE

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(
            int id,
            [FromBody] CitaDTO dto)
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

        // ACEPTAR

        [HttpPatch("{id}/aceptar")]
        public async Task<IActionResult> AceptarCita(int id)
        {
            if (!TryGetUserId(out var userId))
                return Unauthorized();

            if (
                !await _service.ChangeStatusForAssignedUser(
                    id,
                    userId,
                    User.IsInRole("Admin"),
                    "Aceptada"
                )
            )
            {
                return Forbid();
            }

            return Ok(
                new
                {
                    message = "Cita aceptada"
                }
            );
        }

        // DECLINAR

        [HttpPatch("{id}/declinar")]
        public async Task<IActionResult> DeclinarCita(int id)
        {
            if (!TryGetUserId(out var userId))
                return Unauthorized();

            if (
                !await _service.ChangeStatusForAssignedUser(
                    id,
                    userId,
                    User.IsInRole("Admin"),
                    "Declinada"
                )
            )
            {
                return Forbid();
            }

            return Ok(
                new
                {
                    message = "Cita declinada"
                }
            );
        }
    }
}
