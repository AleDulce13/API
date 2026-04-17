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
    public class ServicioSucursalService
    {
        private readonly IGenericRepository<ServicioSucursal> _repo;
        private readonly LogService _logService;
        private readonly AppDbContext _context;

        public ServicioSucursalService(IGenericRepository<ServicioSucursal> repo, AppDbContext context, LogService logService)
        {
            _repo = repo;
            _context = context;
            _logService = logService;
        }

        // GET ALL
        public async Task<List<ServicioSucursal>> GetAll()
        {
            return await _repo.GetAll();
        }

        // GET BY ID
        public async Task<ServicioSucursal> GetById(int id)
        {
            return await _repo.GetById(id);
        }

        //public async Task<List<ServicioAdminDTO>> ObtenerServiciosAdmin(string userId)
        //{
        //    // Obtener el usuario
        //    var usuario = await _context.Usuarios
        //        .FirstOrDefaultAsync(u => u.IdUsuario.ToString() == userId);

        //    if (usuario == null)
        //        return new List<ServicioAdminDTO>();

        //    var servicios = await (
        //        from ss in _context.ServicioSucursal
        //        join s in _context.Servicios on ss.IdServicio equals s.IdServicio
        //        join suc in _context.Sucursales on ss.IdSucursal equals suc.IdSucursal
        //        join h in _context.HorarioLocal on suc.IdSucursal equals h.IdSucursal
        //        where ss.IdSucursal == usuario.IdSucursal 
        //        select new ServicioAdminDTO
        //        {
        //            NombreServicio = s.Nombre,
        //            NombreSucursal = suc.Nombre,
        //            Direccion = suc.Direccion,
        //            Precio = ss.Precio,
        //            HoraApertura = h.HoraApertura,
        //            HoraCierre = h.HoraCierre,
        //            Imagen = s.Imagen
        //        }
        //    ).ToListAsync();

        //    return servicios;
        //}

        // CREATE
        public async Task Add(ServicioSucursalDTO dto)
        {

            if (dto.IdServicio == 0 || dto.IdSucursal == 0)
                throw new Exception("Servicio o Sucursal inválidos");

            var data = new ServicioSucursal
            {
                IdServicio = dto.IdServicio,
                IdSucursal = dto.IdSucursal,
                Precio = dto.Precio,
                Estado = dto.Estado
            };

            _context.ServicioSucursal.Add(data);
            await _context.SaveChangesAsync();


            await _repo.Add(data);

            await _logService.Crear(new LogDTO
            {
                IdUsuario = null,
                Accion = "CREAR_SERVICIO_SUCURSAL",
                Descripcion = $"Se asignó servicio {dto.IdServicio} a sucursal {dto.IdSucursal} con precio {dto.Precio}",
                TablaAfectada = "ServicioSucursal",
                RegistroId = data.IdServicioSucursal,
                Ip = null
            });
        }

        // UPDATE
        public async Task Update(int id, ServicioSucursalDTO dto)
        {
            var data = await _repo.GetById(id);

            if (data == null)
                throw new Exception("Registro no encontrado");

            data.IdServicio = dto.IdServicio;
            data.IdSucursal = dto.IdSucursal;
            data.Precio = dto.Precio;
            data.Estado = dto.Estado;

            await _repo.Update(data);

            await _logService.Crear(new LogDTO
            {
                IdUsuario = null,
                Accion = "ACTUALIZAR_SERVICIO_SUCURSAL",
                Descripcion = $"Se actualizó servicio sucursal ID {id}",
                TablaAfectada = "ServicioSucursal",
                RegistroId = id,
                Ip = null
            });
        }

        // DELETE
        public async Task Delete(int id)
        {
            var data = await _repo.GetById(id);

            if (data == null)
                throw new Exception("Registro no encontrado");

            await _repo.Delete(id);

            await _logService.Crear(new LogDTO
            {
                IdUsuario = null,
                Accion = "ELIMINAR_SERVICIO_SUCURSAL",
                Descripcion = $"Se eliminó servicio sucursal ID {id}",
                TablaAfectada = "ServicioSucursal",
                RegistroId = id,
                Ip = null
            });
        }

    }
}
