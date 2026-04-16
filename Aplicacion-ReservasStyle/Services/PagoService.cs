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
    public class PagoService
    {
        private readonly IGenericRepository<Pago> _repo;
        private readonly LogService _logService;

        public PagoService(IGenericRepository<Pago> repo, LogService logService)
        {
            _repo = repo;
            _logService = logService;
        }

        // GET ALL
        public async Task<List<Pago>> GetAll()
        {
            return await _repo.GetAll();
        }

        // GET BY ID
        public async Task<Pago> GetById(int id)
        {
            return await _repo.GetById(id);
        }

        // CREATE
        public async Task Add(PagoDTO dto)
        {
            var pago = new Pago
            {
                IdCita = dto.IdCita,
                Monto = dto.Monto,
                MetodoPago = dto.MetodoPago,
                EstadoPago = dto.EstadoPago,
                ReferenciaTransaccion = dto.ReferenciaTransaccion,
                FechaPago = DateTime.UtcNow
            };

            await _repo.Add(pago);

            await _logService.Crear(new LogDTO
            {
                IdUsuario = null,
                Accion = "CREAR_PAGO",
                Descripcion = $"Pago registrado por {dto.Monto} usando {dto.MetodoPago}",
                TablaAfectada = "Pagos",
                RegistroId = pago.IdPago,
                Ip = null
            });
        }

        // UPDATE
        public async Task Update(int id, PagoDTO dto)
        {
            var pago = await _repo.GetById(id);

            if (pago == null)
                throw new Exception("Pago no encontrado");

            pago.IdCita = dto.IdCita;
            pago.Monto = dto.Monto;
            pago.MetodoPago = dto.MetodoPago;
            pago.EstadoPago = dto.EstadoPago;
            pago.ReferenciaTransaccion = dto.ReferenciaTransaccion;

            await _repo.Update(pago);

            await _logService.Crear(new LogDTO
            {
                IdUsuario = null,
                Accion = "ACTUALIZAR_PAGO",
                Descripcion = $"Se actualizó el pago ID {id}",
                TablaAfectada = "Pagos",
                RegistroId = id,
                Ip = null
            });
        }

        // DELETE
        public async Task Delete(int id)
        {
            var pago = await _repo.GetById(id);

            if (pago == null)
                throw new Exception("Pago no encontrado");

            await _repo.Delete(id);

            await _logService.Crear(new LogDTO
            {
                IdUsuario = null,
                Accion = "ELIMINAR_PAGO",
                Descripcion = $"Se eliminó el pago ID {id}",
                TablaAfectada = "Pagos",
                RegistroId = id,
                Ip = null
            });
        }

    }
}
