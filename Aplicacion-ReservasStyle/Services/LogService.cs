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
    public class LogService
    {
        private readonly AppDbContext _context;

        public LogService(AppDbContext context)
        {
            _context = context;
        }

        // GET ALL
        public async Task<List<Log>> ObtenerTodos()
        {
            return await _context.Logs
                .Include(x => x.Usuario)
                .ToListAsync();
        }

        // GET BY ID
        public async Task<Log?> ObtenerPorId(int id)
        {
            return await _context.Logs.FindAsync(id);
        }

        // CREATE
        public async Task<Log> Crear(LogDTO dto)
        {
            var log = new Log
            {
                IdUsuario = dto.IdUsuario,
                Accion = dto.Accion,
                Descripcion = dto.Descripcion,
                TablaAfectada = dto.TablaAfectada,
                RegistroId = dto.RegistroId,
                Ip = dto.Ip,
                Recha = DateTime.UtcNow
            };

            _context.Logs.Add(log);
            await _context.SaveChangesAsync();

            return log;
        }
    }
}
