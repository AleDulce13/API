using Aplicacion_ReservasStyle.DTOs;
using Dominio_ReservasStyle.Entities;
using Infraestructura_ReservasStyle.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplicacion_ReservasStyle.Services
{
    public class PromocionServicioService
    {
        private readonly AppDbContext _context;
        private readonly LogService _logService;

        public PromocionServicioService(AppDbContext context, LogService logService)
        {
            _context = context;
            _logService = logService;
        }

        // GET ALL
        public async Task<List<PromocionServicio>> ObtenerTodos()
        {
            return await _context.PromocionServicio
                .Include(x => x.Promocion)
                .Include(x => x.ServicioSucursal)
                .ToListAsync();
        }

        // CREATE
        public async Task<PromocionServicio> Crear(PromocionServicioDTO dto)
        {
            var existe = await _context.PromocionServicio
                .AnyAsync(x => x.IdPromocion == dto.IdPromocion
                            && x.IdServicioSucursal == dto.IdServicioSucursal);

            if (existe)
                throw new Exception("Ya existe esta relación");

            var entidad = new PromocionServicio
            {
                IdPromocion = dto.IdPromocion,
                IdServicioSucursal = dto.IdServicioSucursal
            };

            _context.PromocionServicio.Add(entidad);
            await _context.SaveChangesAsync();

            await _logService.Crear(new LogDTO
            {
                IdUsuario = null,
                Accion = "ASIGNAR_PROMOCION_SERVICIO",
                Descripcion = $"Se asignó promoción {dto.IdPromocion} al servicio sucursal {dto.IdServicioSucursal}",
                TablaAfectada = "PromocionServicio",
                RegistroId = 0, 
                Ip = null
            });

            return entidad;
        }

        // DELETE
        public async Task<bool> Eliminar(int idPromocion, int idServicioSucursal)
        {
            var entidad = await _context.PromocionServicio
                .FirstOrDefaultAsync(x =>
                    x.IdPromocion == idPromocion &&
                    x.IdServicioSucursal == idServicioSucursal);

            if (entidad == null)
                return false;

            _context.PromocionServicio.Remove(entidad);
            await _context.SaveChangesAsync();

            await _logService.Crear(new LogDTO
            {
                IdUsuario = null,
                Accion = "ELIMINAR_PROMOCION_SERVICIO",
                Descripcion = $"Se eliminó relación promoción {idPromocion} - servicio {idServicioSucursal}",
                TablaAfectada = "PromocionServicio",
                RegistroId = 0,
                Ip = null
            });

            return true;
        }

    }
}
