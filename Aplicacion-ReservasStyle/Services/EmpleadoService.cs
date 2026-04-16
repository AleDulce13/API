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
    public class EmpleadoService
    {
        private readonly IGenericRepository<Empleado> _repo;
        private readonly LogService _logService;

        public EmpleadoService(IGenericRepository<Empleado> repo, LogService logService)
        {
            _repo = repo;
            _logService = logService;
        }

        // GET ALL
        public async Task<List<Empleado>> GetAll()
        {
            return await _repo.GetAll();
        }

        // GET BY ID
        public async Task<Empleado> GetById(int id)
        {
            return await _repo.GetById(id);
        }

        // CREATE
        public async Task Add(EmpleadoDTO dto)
        {
            var empleado = new Empleado
            {
                Nombre = dto.Nombre,
                Apellido = dto.Apellido,
                Telefono = dto.Telefono,
                Especialidad = dto.Especialidad,
                Estado = dto.Estado,
                IdSucursal = dto.IdSucursal,
                FechaRegistro = DateTime.UtcNow
            };

            await _repo.Add(empleado);

            await _logService.Crear(new LogDTO
            {
                IdUsuario = null,
                Accion = "CREAR_EMPLEADO",
                Descripcion = $"Se creó el empleado {dto.Nombre} {dto.Apellido}",
                TablaAfectada = "Empleados",
                RegistroId = empleado.IdEmpleado,
                Ip = null
            });
        }

        // UPDATE
        public async Task Update(int id, EmpleadoDTO dto)
        {
            var empleado = await _repo.GetById(id);

            if (empleado == null)
                throw new Exception("Empleado no encontrado");

            empleado.Nombre = dto.Nombre;
            empleado.Apellido = dto.Apellido;
            empleado.Telefono = dto.Telefono;
            empleado.Especialidad = dto.Especialidad;
            empleado.Estado = dto.Estado;
            empleado.IdSucursal = dto.IdSucursal;

            await _repo.Update(empleado);

            await _logService.Crear(new LogDTO
            {
                IdUsuario = null,
                Accion = "ACTUALIZAR_EMPLEADO",
                Descripcion = $"Se actualizó el empleado ID {id}",
                TablaAfectada = "Empleados",
                RegistroId = id,
                Ip = null
            });
        }

        // DELETE
        public async Task Delete(int id)
        {
            var empleado = await _repo.GetById(id);

            if (empleado == null)
                throw new Exception("Empleado no encontrado");

            await _repo.Delete(id);

            await _logService.Crear(new LogDTO
            {
                IdUsuario = null,
                Accion = "ELIMINAR_EMPLEADO",
                Descripcion = $"Se eliminó el empleado ID {id}",
                TablaAfectada = "Empleados",
                RegistroId = id,
                Ip = null
            });
        }
    }
}
