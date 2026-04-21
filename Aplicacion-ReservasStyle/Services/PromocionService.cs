using Dominio_ReservasStyle.Entities;
using Microsoft.EntityFrameworkCore;
using Infraestructura_ReservasStyle.Data;
using Aplicacion_ReservasStyle.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplicacion_ReservasStyle.Services
{
    public class PromocionService
    {
        private readonly AppDbContext _context;
        private readonly LogService _logService;

        public PromocionService(AppDbContext context, LogService logService)
        {
            _context = context;
            _logService = logService;
        }

        //GET ALL
        public async Task<List<Promocion>> ObtenerTodas()
        {
            return await _context.Promociones.ToListAsync();
        }

        //GET BY ID
        public async Task<Promocion?> ObtenerPorId(int id)
        {
            return await _context.Promociones.FindAsync(id);
        }

        // CREATE
        public async Task<Promocion> Crear(PromocionDTO dto)
        {
            var promocion = new Promocion
            {
                Nombre = dto.Nombre,
                Descripcion = dto.Descripcion,
                PorcentajeDescuento = dto.PorcentajeDescuento,

                FechaInicio = dto.FechaInicio.ToDateTime(TimeOnly.MinValue),
                FechaFin = dto.FechaFin.ToDateTime(TimeOnly.MinValue),

                Estado = dto.Estado
            };

            _context.Promociones.Add(promocion);
            await _context.SaveChangesAsync();

            await _logService.Crear(new LogDTO
            {
                IdUsuario = null,
                Accion = "CREAR_PROMOCION",
                Descripcion = $"Se creó promoción {dto.Nombre} con {dto.PorcentajeDescuento}% de descuento",
                TablaAfectada = "Promociones",
                RegistroId = promocion.IdPromocion,
                Ip = null
            });

            return promocion;
        }

        // UPDATE
        public async Task<bool> Actualizar(int id, PromocionDTO dto)
        {
            var promocion = await _context.Promociones.FindAsync(id);

            if (promocion == null)
                return false;

            promocion.Nombre = dto.Nombre;
            promocion.Descripcion = dto.Descripcion;
            promocion.PorcentajeDescuento = dto.PorcentajeDescuento;

            promocion.FechaInicio = dto.FechaInicio.ToDateTime(TimeOnly.MinValue);
            promocion.FechaFin = dto.FechaFin.ToDateTime(TimeOnly.MinValue);

            promocion.Estado = dto.Estado;

            await _context.SaveChangesAsync();

            await _logService.Crear(new LogDTO
            {
                IdUsuario = null,
                Accion = "ACTUALIZAR_PROMOCION",
                Descripcion = $"Se actualizó la promoción ID {id}",
                TablaAfectada = "Promociones",
                RegistroId = id,
                Ip = null
            });

            return true;
        }

        // DELETE
        public async Task<bool> Eliminar(int id)
        {
            var promocion = await _context.Promociones.FindAsync(id);

            if (promocion == null)
                return false;

            _context.Promociones.Remove(promocion);
            await _context.SaveChangesAsync();

            await _logService.Crear(new LogDTO
            {
                IdUsuario = null,
                Accion = "ELIMINAR_PROMOCION",
                Descripcion = $"Se eliminó la promoción ID {id}",
                TablaAfectada = "Promociones",
                RegistroId = id,
                Ip = null
            });

            return true;
        }

    }
}
