using Aplicacion_ReservasStyle.DTOs;
using Aplicacion_ReservasStyle.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ApiReservasStyle.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class EmpleadoController : ControllerBase
    {
        private readonly EmpleadoService _service;

        public EmpleadoController(EmpleadoService service)
        {
            _service = service;
        }

        private int ObtenerIdUsuario()
        {
            var claim = User.FindFirst(
                ClaimTypes.NameIdentifier
            );

            if (claim == null)
                throw new UnauthorizedAccessException(
                    "No se encontró el usuario en el token"
                );

            return int.Parse(claim.Value);
        }

        // GET EMPLEADOS 
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var idUsuario = ObtenerIdUsuario();

                var empleados =
                    await _service.GetAllByUsuario(idUsuario);

                return Ok(empleados);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new
                {
                    mensaje = ex.Message
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    mensaje = ex.Message
                });
            }
        }

        // GET BY ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var idUsuario = ObtenerIdUsuario();

                var empleado =
                    await _service.GetById(id, idUsuario);

                if (empleado == null)
                    return NotFound();

                return Ok(empleado);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new
                {
                    mensaje = ex.Message
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    mensaje = ex.Message
                });
            }
        }

        // CREAR
        [HttpPost]
        public async Task<IActionResult> Add(
            [FromBody] EmpleadoDTO dto)
        {
            try
            {
                var idUsuario = ObtenerIdUsuario();

                await _service.Add(dto, idUsuario);

                return Ok(new
                {
                    mensaje = "Empleado creado correctamente"
                });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new
                {
                    mensaje = ex.Message
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    mensaje = ex.Message
                });
            }
        }

        // ACTUALIZAR
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(
            int id,
            [FromBody] EmpleadoDTO dto)
        {
            try
            {
                var idUsuario = ObtenerIdUsuario();

                await _service.Update(
                    id,
                    dto,
                    idUsuario
                );

                return Ok(new
                {
                    mensaje = "Empleado actualizado correctamente"
                });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new
                {
                    mensaje = ex.Message
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    mensaje = ex.Message
                });
            }
        }

        // ELIMINAR
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var idUsuario = ObtenerIdUsuario();

                await _service.Delete(
                    id,
                    idUsuario
                );

                return Ok(new
                {
                    mensaje = "Empleado eliminado correctamente"
                });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new
                {
                    mensaje = ex.Message
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    mensaje = ex.Message
                });
            }
        }
    }
}