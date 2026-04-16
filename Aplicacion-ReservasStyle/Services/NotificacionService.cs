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
    public class NotificacionService
    {
        private readonly AppDbContext _context;
        private readonly LogService _logService;

        public NotificacionService(AppDbContext context, LogService logService)
        {
            _context = context;
            _logService = logService;
        }

        // GET ALL
        public async Task<List<Notificacion>> ObtenerTodas()
        {
            return await _context.Notificaciones.ToListAsync();
        }

        // GET BY ID
        public async Task<Notificacion?> ObtenerPorId(int id)
        {
            return await _context.Notificaciones.FindAsync(id);
        }

        // CREATE
        public async Task<Notificacion> Crear(NotificacionDTO dto)
        {
            var notificacion = new Notificacion
            {
                IdUsuario = dto.IdUsuario,
                Mensaje = dto.Mensaje,
                FechaEnvio = dto.FechaEnvio,
                Leida = dto.Leida
            };

            _context.Notificaciones.Add(notificacion);
            await _context.SaveChangesAsync();

            await _logService.Crear(new LogDTO
            {
                IdUsuario = dto.IdUsuario,
                Accion = "CREAR_NOTIFICACION",
                Descripcion = $"Se envió notificación: {dto.Mensaje}",
                TablaAfectada = "Notificaciones",
                RegistroId = notificacion.IdNotificacion,
                Ip = null
            });


            return notificacion;
        }

        // UPDATE
        public async Task<bool> Actualizar(int id, NotificacionDTO dto)
        {
            var notificacion = await _context.Notificaciones.FindAsync(id);

            if (notificacion == null)
                return false;

            notificacion.IdUsuario = dto.IdUsuario;
            notificacion.Mensaje = dto.Mensaje;
            notificacion.FechaEnvio = dto.FechaEnvio;
            notificacion.Leida = dto.Leida;

            await _context.SaveChangesAsync();

            await _logService.Crear(new LogDTO
            {
                IdUsuario = dto.IdUsuario,
                Accion = "ACTUALIZAR_NOTIFICACION",
                Descripcion = $"Se actualizó la notificación ID {id}",
                TablaAfectada = "Notificaciones",
                RegistroId = id,
                Ip = null
            });

            return true;
        }

        // DELETE
        public async Task<bool> Eliminar(int id)
        {
            var notificacion = await _context.Notificaciones.FindAsync(id);

            if (notificacion == null)
                return false;

            _context.Notificaciones.Remove(notificacion);
            await _context.SaveChangesAsync();

            await _logService.Crear(new LogDTO
            {
                IdUsuario = notificacion.IdUsuario,
                Accion = "ELIMINAR_NOTIFICACION",
                Descripcion = $"Se eliminó la notificación ID {id}",
                TablaAfectada = "Notificaciones",
                RegistroId = id,
                Ip = null
            });

            return true;
        }

    }
}
