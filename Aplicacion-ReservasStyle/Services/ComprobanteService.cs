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
    public class ComprobanteService
    {
        private readonly IGenericRepository<Comprobante> _repo;
        private readonly LogService _logService;

        public ComprobanteService(IGenericRepository<Comprobante> repo, LogService logService)
        {
            _repo = repo;
            _logService = logService;
        }

        // GET ALL
        public async Task<List<Comprobante>> GetAll()
        {
            return await _repo.GetAll();
        }

        // GET BY ID
        public async Task<Comprobante> GetById(int id)
        {
            return await _repo.GetById(id);
        }

        // CREATE
        public async Task Add(ComprobanteDTO dto)
        {
            var comprobante = new Comprobante
            {
                IdPago = dto.IdPago,
                Folio = dto.Folio,
                FechaEmision = DateTime.UtcNow
            };

            await _repo.Add(comprobante);

            await _logService.Crear(new LogDTO
            {
                IdUsuario = null,
                Accion = "CREAR_COMPROBANTE",
                Descripcion = $"Se creó el comprobante con folio {dto.Folio}",
                TablaAfectada = "Comprobantes",
                RegistroId = comprobante.IdComprobante,
                Ip = null
            });
        }

        // UPDATE
        public async Task Update(int id, ComprobanteDTO dto)
        {
            var comprobante = await _repo.GetById(id);

            if (comprobante == null)
                throw new Exception("Comprobante no encontrado");

            comprobante.IdPago = dto.IdPago;
            comprobante.Folio = dto.Folio;

            await _repo.Update(comprobante);

            await _logService.Crear(new LogDTO
            {
                IdUsuario = null,
                Accion = "ACTUALIZAR_COMPROBANTE",
                Descripcion = $"Se actualizó el comprobante ID {id}",
                TablaAfectada = "Comprobantes",
                RegistroId = id,
                Ip = null
            });
        }

        // DELETE
        public async Task Delete(int id)
        {
            var comprobante = await _repo.GetById(id);

            if (comprobante == null)
                throw new Exception("Comprobante no encontrado");

            await _repo.Delete(id);

            await _logService.Crear(new LogDTO
            {
                IdUsuario = null,
                Accion = "ELIMINAR_COMPROBANTE",
                Descripcion = $"Se eliminó el comprobante ID {id}",
                TablaAfectada = "Comprobantes",
                RegistroId = id,
                Ip = null
            });
        }
        
    }
}

