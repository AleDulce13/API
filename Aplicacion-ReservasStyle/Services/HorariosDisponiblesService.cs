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
    public class HorariosDisponiblesService
    {
        private readonly IGenericRepository<HorariosDisponibles> _repo;
        private readonly LogService _logService;

        public HorariosDisponiblesService(IGenericRepository<HorariosDisponibles> repo, LogService logService)
        {
            _repo = repo;
            _logService = logService;
        }

        // GET ALL
        public async Task<List<HorariosDisponibles>> GetAll()
        {
            await _logService.Crear(new LogDTO
            {
                IdUsuario = null,
                Accion = "HORARIO_GET_ALL",
                Descripcion = "Consulta de todos los horarios disponibles",
                TablaAfectada = "HorariosDisponibles",
                RegistroId = null,
                Ip = null
            });

            return await _repo.GetAll();
        }


        // GET BY ID
        public async Task<HorariosDisponibles> GetById(int id)
        {
            var horario = await _repo.GetById(id);

            await _logService.Crear(new LogDTO
            {
                IdUsuario = null,
                Accion = "HORARIO_GET_BY_ID",
                Descripcion = $"Consulta de horario ID {id}",
                TablaAfectada = "HorariosDisponibles",
                RegistroId = id,
                Ip = null
            });

            return horario;
        }


        // CREATE
        public async Task Add(HorariosDisponiblesDTO dto)
        {
            var horario = new HorariosDisponibles
            {
                IdEmpleado = dto.IdEmpleado,
                DiaSemana = dto.DiaSemana,
                HoraInicio = dto.HoraInicio,
                HoraFin = dto.HoraFin
            };

            await _repo.Add(horario);

            await _logService.Crear(new LogDTO
            {
                IdUsuario = null,
                Accion = "HORARIO_CREATE",
                Descripcion = $"Se creó horario para empleado {dto.IdEmpleado} ({dto.DiaSemana})",
                TablaAfectada = "HorariosDisponibles",
                RegistroId = horario.IdHorario,
                Ip = null
            });
        }


        // UPDATE
        public async Task Update(int id, HorariosDisponiblesDTO dto)
        {
            var horario = await _repo.GetById(id);

            if (horario == null)
                throw new Exception("Horario no encontrado");

            horario.IdEmpleado = dto.IdEmpleado;
            horario.DiaSemana = dto.DiaSemana;
            horario.HoraInicio = dto.HoraInicio;
            horario.HoraFin = dto.HoraFin;

            await _repo.Update(horario);

            await _logService.Crear(new LogDTO
            {
                IdUsuario = null,
                Accion = "HORARIO_UPDATE",
                Descripcion = $"Se actualizó horario ID {id}",
                TablaAfectada = "HorariosDisponibles",
                RegistroId = id,
                Ip = null
            });
        }


        // DELETE
        public async Task Delete(int id)
        {
            await _repo.Delete(id);

            await _logService.Crear(new LogDTO
            {
                IdUsuario = null,
                Accion = "HORARIO_DELETE",
                Descripcion = $"Se eliminó horario ID {id}",
                TablaAfectada = "HorariosDisponibles",
                RegistroId = id,
                Ip = null
            });
        }

    }
}
