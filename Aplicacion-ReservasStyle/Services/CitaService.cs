using Aplicacion_ReservasStyle.DTOs;
using Dominio_ReservasStyle.Entities;
using Infraestructura_ReservasStyle.Repositories;
using Aplicacion_ReservasStyle.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplicacion_ReservasStyle.Services
{
    public class CitaService
    {
        private readonly IGenericRepository<Cita> _repo;
        private readonly LogService _logService;

        public CitaService(IGenericRepository<Cita> repo,LogService logService)
        {
            _repo = repo;
            _logService = logService;
        }


        // GET ALL
        public async Task<List<Cita>> GetAll()
        {
            return await _repo.GetAll();
        }

        // GET BY ID
        public async Task<Cita> GetById(int id)
        {
            return await _repo.GetById(id);
        }

        // CREATE
        public async Task Add(CitaDTO dto)
        {
            var cita = new Cita
            {
                IdCliente = dto.IdCliente,
                IdEmpleado = dto.IdEmpleado,
                IdServicioSucursal = dto.IdServicioSucursal,
                Fecha = dto.Fecha.ToUniversalTime(),
                HoraInicio = dto.HoraInicio,
                HoraFin = dto.HoraFin,
                Estado = dto.Estado,
                FechaCreacion = DateTime.UtcNow
            };

            await _repo.Add(cita);

            
            // LOG 
            await _logService.Crear(new LogDTO
            {
                IdUsuario = dto.IdCliente, 
                Accion = "CREAR_CITA",
                Descripcion = $"Se creó una cita para el cliente {dto.IdCliente}",
                TablaAfectada = "Citas",
                RegistroId = cita.IdCita,
                Ip = null
            });
        }

        // UPDATE
        public async Task Update(int id, CitaDTO dto)
        {
            var cita = await _repo.GetById(id);

            if (cita == null)
                throw new Exception("Cita no encontrada");

            cita.IdCliente = dto.IdCliente;
            cita.IdEmpleado = dto.IdEmpleado;
            cita.IdServicioSucursal = dto.IdServicioSucursal;
            cita.Fecha = dto.Fecha.ToUniversalTime();
            cita.HoraInicio = dto.HoraInicio;
            cita.HoraFin = dto.HoraFin;
            cita.Estado = dto.Estado;

            await _repo.Update(cita);


            // LOG 
            await _logService.Crear(new LogDTO
            {
                IdUsuario = dto.IdCliente,
                Accion = "ACTUALIZAR_CITA",
                Descripcion = $"Se actualizó la cita ID {id}",
                TablaAfectada = "Citas",
                RegistroId = id,
                Ip = null
            });
        }

        // DELETE
        public async Task Delete(int id)
        {
            var cita = await _repo.GetById(id);

            if (cita == null)
                throw new Exception("Cita no encontrada");

            await _repo.Delete(id);

            // LOG 
            await _logService.Crear(new LogDTO
            {
                IdUsuario = cita.IdCliente,
                Accion = "ELIMINAR_CITA",
                Descripcion = $"Se eliminó la cita ID {id}",
                TablaAfectada = "Citas",
                RegistroId = id,
                Ip = null
            });
        }

    }
}
