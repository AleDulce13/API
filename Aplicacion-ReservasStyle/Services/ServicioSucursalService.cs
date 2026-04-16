using Aplicacion_ReservasStyle.DTOs;
using Dominio_ReservasStyle.Entities;
using Infraestructura_ReservasStyle.Repositories;
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

        public ServicioSucursalService(IGenericRepository<ServicioSucursal> repo, LogService logService)
        {
            _repo = repo;
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

        // CREATE
        public async Task Add(ServicioSucursalDTO dto)
        {
            var data = new ServicioSucursal
            {
                IdServicio = dto.IdServicio,
                IdSucursal = dto.IdSucursal,
                Precio = dto.Precio,
                Estado = dto.Estado
            };

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
