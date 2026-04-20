using Aplicacion_ReservasStyle.DTOs;
using Aplicacion_ReservasStyle.Services;
using Dominio_ReservasStyle.Entities;
using Infraestructura_ReservasStyle.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ApiReservasStyle.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ServiciosController : ControllerBase
    {
        private readonly ServicioService _service;
        private readonly IWebHostEnvironment _env;
        private readonly AppDbContext _context;

        public ServiciosController(ServicioService service, AppDbContext context,IWebHostEnvironment env)
        {
            _service = service;
            _context = context;
            _env = env;
        }

        // GET ALL
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var data = await _service.GetAll();
            return Ok(data);
        }

        // GET BY ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var servicio = await _context.Servicios
                .Include(s => s.ServicioSucursales)
                    .ThenInclude(ss => ss.Sucursal)
                .FirstOrDefaultAsync(s => s.IdServicio == id);

            if (servicio == null)
                return NotFound("Servicio no encontrado");

            return Ok(servicio);
        }

        // DETALLE

        [HttpGet("detalle")]
        public async Task<IActionResult> GetServiciosDetalle()
        {
            var data = await _service.GetServiciosDetalle();
            return Ok(data);
        }

        // CREAR SERVICIO COMPLETO

        [Authorize]
        [HttpPost("crear-completo")]
        public async Task<IActionResult> Crear([FromForm] ServicioCreateDto dto)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userIdClaim))
                    return Unauthorized("Token inválido");

                int userId = int.Parse(userIdClaim);

                var usuario = await _context.Usuarios
                    .FirstOrDefaultAsync(u => u.IdUsuario == userId);

                if (usuario == null)
                    return Unauthorized("Usuario no encontrado");

                if (usuario.IdSucursal == null)
                    return BadRequest("El usuario no tiene sucursal asignada");

                var sucursalId = usuario.IdSucursal.Value;

                string imagenUrl = null;

                if (dto.Imagen != null)
                {
                    var webRoot = _env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
                    var folder = Path.Combine(webRoot, "imagenes");

                    if (!Directory.Exists(folder))
                        Directory.CreateDirectory(folder);

                    var fileName = Guid.NewGuid() + Path.GetExtension(dto.Imagen.FileName);
                    var path = Path.Combine(folder, fileName);

                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await dto.Imagen.CopyToAsync(stream);
                    }

                    imagenUrl = "/imagenes/" + fileName;
                }

                var servicio = new Servicio
                {
                    Nombre = dto.Nombre,
                    Descripcion = dto.Descripcion,
                    DuracionMinutos = dto.DuracionMinutos,
                    ImagenUrl = imagenUrl,
                    Estado = dto.Estado
                };

                _context.Servicios.Add(servicio);
                await _context.SaveChangesAsync();

                var servicioSucursal = new ServicioSucursal
                {
                    IdServicio = servicio.IdServicio,
                    IdSucursal = sucursalId,
                    Precio = dto.Precio
                };

                _context.ServicioSucursal.Add(servicioSucursal);
                await _context.SaveChangesAsync();

                return Ok(servicio);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    error = ex.Message,
                    inner = ex.InnerException?.Message
                });
            }
        }

        // UPDATE
        [HttpPut("{id}")]
        public async Task<IActionResult> Actualizar(int id, [FromForm] ServicioCreateDto dto, IFormFile imagen)
        {
            var servicio = await _context.Servicios.FindAsync(id);

            if (servicio == null)
                return NotFound();

            servicio.Nombre = dto.Nombre;
            servicio.Descripcion = dto.Descripcion;
            servicio.DuracionMinutos = dto.DuracionMinutos;

            if (imagen != null)
            {
                var folder = Path.Combine(_env.WebRootPath ?? "wwwroot", "imagenes");

                if (!Directory.Exists(folder))
                    Directory.CreateDirectory(folder);

                var fileName = Guid.NewGuid() + Path.GetExtension(imagen.FileName);
                var path = Path.Combine(folder, fileName);

                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await imagen.CopyToAsync(stream);
                }

                servicio.ImagenUrl = "/imagenes/" + fileName;
            }

            await _context.SaveChangesAsync();

            return Ok(servicio);
        }

        // DELETE
        [HttpDelete("{id}")]
        public async Task<IActionResult> Eliminar(int id)
        {
            var servicio = await _context.Servicios.FindAsync(id);

            if (servicio == null)
                return NotFound();

            _context.Servicios.Remove(servicio);
            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}
