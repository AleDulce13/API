using Aplicacion_ReservasStyle.DTOs;
using Aplicacion_ReservasStyle.Services;
using Dominio_ReservasStyle.Entities;
using Infraestructura_ReservasStyle.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApiReservasStyle.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SucursalController : ControllerBase
    {
        private readonly SucursalService _service;
        private readonly AppDbContext _context;

        public SucursalController(SucursalService service, AppDbContext context)
        {
            _service = service;
            _context = context;
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
            var data = await _service.GetById(id);

            if (data == null)
                return NotFound();

            return Ok(data);
        }

        // CREATE
        [HttpPost("crear-completo")]
        public async Task<IActionResult> CrearCompleto(RegistrarSucursal dto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                //  Crear sucursal
                var sucursal = new Sucursal
                {
                    Nombre = dto.Nombre,
                    Direccion = dto.Direccion,
                    Ciudad = dto.Ciudad,
                    Estado = dto.Estado,
                    CodigoPostal = dto.CodigoPostal,
                    Telefono = dto.Telefono,
                    EstadoActivo = dto.EstadoActivo
                };

                _context.Sucursales.Add(sucursal);
                await _context.SaveChangesAsync();

                var horarios = dto.Horarios.Select(h => new HorarioLocal
                {
                    IdSucursal = sucursal.IdSucursal,
                    DiaSemana = h.DiaSemana,
                    HoraApertura = TimeSpan.Parse(h.HoraApertura),
                    HoraCierre = TimeSpan.Parse(h.HoraCierre),
                    Estado = h.Estado
                }).ToList();

                _context.HorarioLocal.AddRange(horarios);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                return Ok(new { message = "Todo guardado correctamente" });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return BadRequest(ex.Message);
            }
        }

        // UPDATE
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, SucursalDTO dto)
        {
            await _service.Update(id, dto);
            return Ok("Sucursal actualizada");
        }

        // DELETE
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _service.Delete(id);
            return Ok("Sucursal eliminada");
        }
    }
}

