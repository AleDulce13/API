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
        private readonly IGenericRepository<Empleado> _empleadoRepo;
        private readonly IGenericRepository<Usuario> _usuarioRepo;
        private readonly LogService _logService;

        public EmpleadoService(
            IGenericRepository<Empleado> empleadoRepo,
            IGenericRepository<Usuario> usuarioRepo,
            LogService logService)
        {
            _empleadoRepo = empleadoRepo;
            _usuarioRepo = usuarioRepo;
            _logService = logService;
        }

        // OBTENER EMPLEADOS
        public async Task<List<Empleado>> GetAllByUsuario(int idUsuario)
        {
            var usuario = await _usuarioRepo.GetById(idUsuario);

            if (usuario == null)
                throw new Exception("Usuario no encontrado");

            if (usuario.IdSucursal == null)
                throw new Exception("El usuario no tiene una sucursal asignada");

            var empleados = await _empleadoRepo.GetAll();

            return empleados
                .Where(e => e.IdSucursal == usuario.IdSucursal)
                .ToList();
        }

        // OBTENER EMPLEADO POR ID
        public async Task<Empleado> GetById(int id, int idUsuario)
        {
            var usuario = await _usuarioRepo.GetById(idUsuario);

            if (usuario == null)
                throw new Exception("Usuario no encontrado");

            var empleado = await _empleadoRepo.GetById(id);

            if (empleado == null)
                return null;

            if (empleado.IdSucursal != usuario.IdSucursal)
                return null;

            return empleado;
        }

        // CREAR EMPLEADO
        public async Task Add(
            EmpleadoDTO dto,
            int idUsuario)
        {
            var usuario = await _usuarioRepo.GetById(idUsuario);

            if (usuario == null)
                throw new Exception("Usuario no encontrado");

            if (usuario.IdSucursal == null)
                throw new Exception(
                    "El administrador no tiene una sucursal asignada"
                );

            var empleado = new Empleado
            {
                Nombre = dto.Nombre,
                Apellido = dto.Apellido,
                Telefono = dto.Telefono,
                Especialidad = dto.Especialidad,
                Estado = dto.Estado,
                IdSucursal = usuario.IdSucursal.Value,
                FechaRegistro = DateTime.UtcNow
            };

            await _empleadoRepo.Add(empleado);

            await _logService.Crear(new LogDTO
            {
                IdUsuario = idUsuario,
                Accion = "CREAR_EMPLEADO",
                Descripcion =
                    $"Se creó el empleado {empleado.Nombre} {empleado.Apellido}",
                TablaAfectada = "Empleados",
                RegistroId = empleado.IdEmpleado,
                Ip = null
            });
        }

        // ACTUALIZAR EMPLEADO
        public async Task Update(
            int id,
            EmpleadoDTO dto,
            int idUsuario)
        {
            var usuario = await _usuarioRepo.GetById(idUsuario);

            if (usuario == null)
                throw new Exception("Usuario no encontrado");

            if (usuario.IdSucursal == null)
                throw new Exception(
                    "El administrador no tiene una sucursal asignada"
                );

            var empleado = await _empleadoRepo.GetById(id);

            if (empleado == null)
                throw new Exception("Empleado no encontrado");

            if (empleado.IdSucursal != usuario.IdSucursal)
                throw new Exception(
                    "No tienes permiso para modificar este empleado"
                );

            empleado.Nombre = dto.Nombre;
            empleado.Apellido = dto.Apellido;
            empleado.Telefono = dto.Telefono;
            empleado.Especialidad = dto.Especialidad;
            empleado.Estado = dto.Estado;
            empleado.IdSucursal = usuario.IdSucursal.Value;

            await _empleadoRepo.Update(empleado);

            await _logService.Crear(new LogDTO
            {
                IdUsuario = idUsuario,
                Accion = "ACTUALIZAR_EMPLEADO",
                Descripcion =
                    $"Se actualizó el empleado ID {id}",
                TablaAfectada = "Empleados",
                RegistroId = id,
                Ip = null
            });
        }

        // ELIMINAR EMPLEADO
        public async Task Delete(
            int id,
            int idUsuario)
        {
            var usuario = await _usuarioRepo.GetById(idUsuario);

            if (usuario == null)
                throw new Exception("Usuario no encontrado");

            if (usuario.IdSucursal == null)
                throw new Exception(
                    "El administrador no tiene una sucursal asignada"
                );

            var empleado = await _empleadoRepo.GetById(id);

            if (empleado == null)
                throw new Exception("Empleado no encontrado");

            if (empleado.IdSucursal != usuario.IdSucursal)
                throw new Exception(
                    "No tienes permiso para eliminar este empleado"
                );

            await _empleadoRepo.Delete(id);

            await _logService.Crear(new LogDTO
            {
                IdUsuario = idUsuario,
                Accion = "ELIMINAR_EMPLEADO",
                Descripcion =
                    $"Se eliminó el empleado ID {id}",
                TablaAfectada = "Empleados",
                RegistroId = id,
                Ip = null
            });
        }
    }
}
