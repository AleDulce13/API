using Aplicacion_ReservasStyle.DTOs;
using Dominio_ReservasStyle.Entities;
using Infraestructura_ReservasStyle.Data;
using Infraestructura_ReservasStyle.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplicacion_ReservasStyle.Services
{
    public class ServicioService
    {
        private readonly IGenericRepository<Servicio> _repo;
        private readonly LogService _logService;
        private readonly AppDbContext _context;

        public ServicioService(IGenericRepository<Servicio> repo, LogService logService, AppDbContext context)
        {
            _repo = repo;
            _logService = logService;
            _context = context;
        }

        // GET ALL
        public async Task<List<Servicio>> GetAll()
        {
            return await _repo.GetAll();
        }

        // GET BY ID
        public async Task<Servicio> GetById(int id)
        {
            return await _repo.GetById(id);
        }


        public async Task<List<ServicioDetalleDTO>> GetServiciosDetalle()
        {
            var result = await (
                from ss in _context.ServicioSucursal
                join s in _context.Servicios on ss.IdServicio equals s.IdServicio
                join su in _context.Sucursales on ss.IdSucursal equals su.IdSucursal
                join hl in _context.HorarioLocal on su.IdSucursal equals hl.IdSucursal into horarios
                from hl in horarios.DefaultIfEmpty()
                where s.Estado && ss.Estado && su.EstadoActivo
                select new ServicioDetalleDTO
                {
                    NombreServicio = s.Nombre,
                    Sucursal = su.Nombre,
                    Direccion = su.Direccion,
                    DiaSemana = hl != null ? hl.DiaSemana : "",
                    HoraApertura = hl != null ? hl.HoraApertura.ToString() : "",
                    HoraCierre = hl != null ? hl.HoraCierre.ToString() : "",
                    Precio = ss.Precio,
                    Imagen = s.ImagenUrl
                }
            ).ToListAsync();

            return result;
        }

        // CREATE
        public async Task<Servicio> Add(ServicioDTO dto)
        {
            var data = new Servicio
            {
                Nombre = dto.Nombre,
                Descripcion = dto.Descripcion,
                DuracionMinutos = dto.DuracionMinutos,
                ImagenUrl = dto.ImagenUrl ?? "",
                Estado = dto.Estado
            };

            await _repo.Add(data);

            await _logService.Crear(new LogDTO
            {
                IdUsuario = null,
                Accion = "CREAR_SERVICIO",
                Descripcion = $"Se creó el servicio '{dto.Nombre}' con duración {dto.DuracionMinutos} minutos",
                TablaAfectada = "Servicios",
                RegistroId = data.IdServicio,
                Ip = null
            });

            return data;

        }

        // UPDATE
        public async Task Update(int id, ServicioDTO dto)
        {
            var data = await _repo.GetById(id);

            if (data == null)
                throw new Exception("Servicio no encontrado");

            data.Nombre = dto.Nombre;
            data.Descripcion = dto.Descripcion;
            data.DuracionMinutos = dto.DuracionMinutos;
            data.ImagenUrl = dto.ImagenUrl;
            data.Estado = dto.Estado;

            await _repo.Update(data);

            await _logService.Crear(new LogDTO
            {
                IdUsuario = null,
                Accion = "ACTUALIZAR_SERVICIO",
                Descripcion = $"Se actualizó el servicio ID {id}",
                TablaAfectada = "Servicios",
                RegistroId = id,
                Ip = null
            });
        }


        // DELETE
        public async Task Delete(int id)
        {
            var data = await _repo.GetById(id);

            if (data == null)
                throw new Exception("Servicio no encontrado");

            await _repo.Delete(id);

            await _logService.Crear(new LogDTO
            {
                IdUsuario = null,
                Accion = "ELIMINAR_SERVICIO",
                Descripcion = $"Se eliminó el servicio ID {id}",
                TablaAfectada = "Servicios",
                RegistroId = id,
                Ip = null
            });
        }
    }
}
