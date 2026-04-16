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
    public class SucursalService
    {
        private readonly IGenericRepository<Sucursal> _repo;
        private readonly LogService _logService;

        public SucursalService(IGenericRepository<Sucursal> repo, LogService logService)
        {
            _repo = repo;
            _logService = logService;
        }

        // GET ALL
        public async Task<List<Sucursal>> GetAll()
        {
            return await _repo.GetAll();
        }

        // GET BY ID
        public async Task<Sucursal> GetById(int id)
        {
            return await _repo.GetById(id);
        }

        // CREATE
        public async Task Add(SucursalDTO dto)
        {
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

            await _repo.Add(sucursal);

            await _logService.Crear(new LogDTO
            {
                IdUsuario = null,
                Accion = "CREAR_SUCURSAL",
                Descripcion = $"Se creó sucursal {dto.Nombre} en {dto.Ciudad}",
                TablaAfectada = "Sucursal",
                RegistroId = sucursal.IdSucursal,
                Ip = null
            });
        }

        // UPDATE
        public async Task Update(int id, SucursalDTO dto)
        {
            var sucursal = await _repo.GetById(id);

            if (sucursal == null)
                throw new Exception("Sucursal no encontrada");

            sucursal.Nombre = dto.Nombre;
            sucursal.Direccion = dto.Direccion;
            sucursal.Ciudad = dto.Ciudad;
            sucursal.Estado = dto.Estado;
            sucursal.CodigoPostal = dto.CodigoPostal;
            sucursal.Telefono = dto.Telefono;
            sucursal.EstadoActivo = dto.EstadoActivo;

            await _repo.Update(sucursal);

            await _logService.Crear(new LogDTO
            {
                IdUsuario = null,
                Accion = "ACTUALIZAR_SUCURSAL",
                Descripcion = $"Se actualizó sucursal ID {id}",
                TablaAfectada = "Sucursal",
                RegistroId = id,
                Ip = null
            });
        }

        // DELETE
        public async Task Delete(int id)
        {
            var sucursal = await _repo.GetById(id);

            if (sucursal == null)
                throw new Exception("Sucursal no encontrada");

            await _repo.Delete(id);

            await _logService.Crear(new LogDTO
            {
                IdUsuario = null,
                Accion = "ELIMINAR_SUCURSAL",
                Descripcion = $"Se eliminó sucursal ID {id}",
                TablaAfectada = "Sucursal",
                RegistroId = id,
                Ip = null
            });
        }

    }
}

