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
    public class HorarioLocalService
    {
        private readonly IGenericRepository<HorarioLocal> _repo;
        private readonly LogService _logService;

        public HorarioLocalService(IGenericRepository<HorarioLocal> repo, LogService logService)
        {
            _repo = repo;
            _logService = logService;
        }

        // GET ALL
        public async Task<List<HorarioLocal>> GetAll()
        {
            return await _repo.GetAll();
        }

        // GET BY ID
        public async Task<HorarioLocal> GetById(int id)
        {
            return await _repo.GetById(id);
        }

        // CREATE
        public async Task Add(HorarioLocalDTO dto)
        {
            var horario = new HorarioLocal
            {
                IdSucursal = dto.IdSucursal,
                DiaSemana = dto.DiaSemana,
                HoraApertura = dto.HoraApertura,
                HoraCierre = dto.HoraCierre,
                Estado = dto.Estado
            };

            await _repo.Add(horario);

            await _logService.Crear(new LogDTO
            {
                IdUsuario = null,
                Accion = "CREAR_HORARIO_LOCAL",
                Descripcion = $"Se creó horario para sucursal {dto.IdSucursal} ({dto.DiaSemana})",
                TablaAfectada = "HorarioLocal",
                RegistroId = horario.IdHorarioLocal,
                Ip = null
            });
        }

        // UPDATE
        public async Task Update(int id, HorarioLocalDTO dto)
        {
            var horario = await _repo.GetById(id);

            if (horario == null)
                throw new Exception("Horario no encontrado");

            horario.IdSucursal = dto.IdSucursal;
            horario.DiaSemana = dto.DiaSemana;
            horario.HoraApertura = dto.HoraApertura;
            horario.HoraCierre = dto.HoraCierre;
            horario.Estado = dto.Estado;

            await _repo.Update(horario);

            await _logService.Crear(new LogDTO
            {
                IdUsuario = null,
                Accion = "ACTUALIZAR_HORARIO_LOCAL",
                Descripcion = $"Se actualizó horario ID {id} de sucursal {dto.IdSucursal}",
                TablaAfectada = "HorarioLocal",
                RegistroId = id,
                Ip = null
            });
        }

        // DELETE
        public async Task Delete(int id)
        {
            var horario = await _repo.GetById(id);

            if (horario == null)
                throw new Exception("Horario no encontrado");

            await _repo.Delete(id);

            await _logService.Crear(new LogDTO
            {
                IdUsuario = null,
                Accion = "ELIMINAR_HORARIO_LOCAL",
                Descripcion = $"Se eliminó horario ID {id}",
                TablaAfectada = "HorarioLocal",
                RegistroId = id,
                Ip = null
            });
        }

    }
}
