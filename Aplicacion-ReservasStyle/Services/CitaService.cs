using Aplicacion_ReservasStyle.DTOs;
using Dominio_ReservasStyle.Entities;
using Infraestructura_ReservasStyle.Repositories;
using Infraestructura_ReservasStyle.Data;
using Microsoft.EntityFrameworkCore;
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
        private readonly IPushNotificationService _pushNotificationService;
        private readonly AppDbContext _context;

        public CitaService(IGenericRepository<Cita> repo, LogService logService, IPushNotificationService pushNotificationService, AppDbContext context)
        {
            _repo = repo;
            _logService = logService;
            _pushNotificationService = pushNotificationService;
            _context = context;
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

        public async Task<List<Cita>> GetAllForUser(int userId, bool isAdmin)
        {
            var citas = await _repo.GetAll();
            return isAdmin ? citas : citas.Where(cita => cita.IdCliente == userId || cita.IdEmpleado == userId).ToList();
        }

        public async Task<Cita?> GetByIdForUser(int id, int userId, bool isAdmin)
        {
            var cita = await _repo.GetById(id);
            return cita is not null && (isAdmin || cita.IdCliente == userId || cita.IdEmpleado == userId) ? cita : null;
        }

        // CREATE
        public async Task Add(CitaDTO dto)
        {
            // La columna IdEmpleado se conserva por compatibilidad, pero contiene IdUsuario del empleado.
            var empleado = await _context.Usuarios.SingleOrDefaultAsync(usuario => usuario.IdUsuario == dto.IdEmpleado && usuario.Estado && usuario.IdRol == 3);
            if (empleado is null) throw new InvalidOperationException("El usuario asignado no es un empleado activo.");

            var servicio = await _context.ServicioSucursal.Include(item => item.Servicio)
                .SingleOrDefaultAsync(item => item.IdServicioSucursal == dto.IdServicioSucursal && item.Estado && item.Servicio.Estado);
            if (servicio is null) throw new InvalidOperationException("El servicio seleccionado no está disponible.");
            if (dto.HoraFin <= dto.HoraInicio || dto.HoraFin - dto.HoraInicio != TimeSpan.FromMinutes(servicio.Servicio.DuracionMinutos))
                throw new InvalidOperationException("La duración de la cita no coincide con el servicio.");

            var fecha = dto.Fecha.Date;
            var existeTraslape = await _context.Citas.AnyAsync(cita => cita.IdEmpleado == dto.IdEmpleado
                && cita.Fecha.Date == fecha && cita.Estado != "Declinada"
                && cita.HoraInicio < dto.HoraFin && dto.HoraInicio < cita.HoraFin);
            if (existeTraslape) throw new InvalidOperationException("El horario seleccionado ya no está disponible.");

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

            await TryNotifyAsync(dto.IdEmpleado, "Nueva cita", "Tienes una nueva cita asignada.", cita.IdCita, "cita_created");
        }

        public async Task<List<string>> GetHorariosDisponibles(int empleadoId, int servicioSucursalId, DateTime fecha)
        {
            var empleadoValido = await _context.Usuarios.AnyAsync(usuario => usuario.IdUsuario == empleadoId && usuario.Estado && usuario.IdRol == 3);
            var servicio = await _context.ServicioSucursal.Include(item => item.Servicio)
                .SingleOrDefaultAsync(item => item.IdServicioSucursal == servicioSucursalId && item.Estado && item.Servicio.Estado);
            if (!empleadoValido || servicio is null) return new List<string>();

            var horariosSucursal = await _context.HorarioLocal
                .Where(item => item.Estado && item.IdSucursal == servicio.IdSucursal)
                .OrderBy(item => item.HoraApertura)
                .ToListAsync();
            var horario = horariosSucursal.FirstOrDefault(item => AplicaAlDia(item.DiaSemana, fecha.DayOfWeek));
            if (horario is null) return new List<string>();

            // Npgsql solo permite UTC para columnas timestamp with time zone.
            var fechaUtc = DateTime.SpecifyKind(fecha.Date, DateTimeKind.Utc);
            var ocupadas = await _context.Citas.Where(cita => cita.IdEmpleado == empleadoId && cita.Fecha.Date == fechaUtc && cita.Estado != "Declinada").ToListAsync();
            var duracion = TimeSpan.FromMinutes(servicio.Servicio.DuracionMinutos);
            var resultado = new List<string>();
            for (var inicio = horario.HoraApertura; inicio + duracion <= horario.HoraCierre; inicio += TimeSpan.FromMinutes(30))
            {
                var fin = inicio + duracion;
                if (!ocupadas.Any(cita => cita.HoraInicio < fin && inicio < cita.HoraFin)) resultado.Add(inicio.ToString(@"hh\:mm"));
            }
            return resultado;
        }

        private static bool AplicaAlDia(string? diaSemana, DayOfWeek dia)
        {
            var texto = diaSemana?.Trim().ToLowerInvariant() ?? string.Empty;
            if (texto.Contains("lunes a domingo")) return true;
            if (texto.Contains("lunes a sábado") || texto.Contains("lunes a sabado")) return dia != DayOfWeek.Sunday;
            var nombres = new Dictionary<DayOfWeek, string>
            {
                [DayOfWeek.Monday] = "lunes", [DayOfWeek.Tuesday] = "martes", [DayOfWeek.Wednesday] = "miércoles",
                [DayOfWeek.Thursday] = "jueves", [DayOfWeek.Friday] = "viernes", [DayOfWeek.Saturday] = "sábado", [DayOfWeek.Sunday] = "domingo"
            };
            return texto.Contains(nombres[dia]) || texto.Contains(dia.ToString().ToLowerInvariant());
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

        public async Task<bool> ChangeStatusForAssignedUser(int id, int userId, bool isAdmin, string status)
        {
            var cita = await _repo.GetById(id);
            if (cita is null) return false;
            if (!isAdmin && cita.IdEmpleado != userId) return false;

            cita.Estado = status;
            await _repo.Update(cita);
            await _logService.Crear(new LogDTO
            {
                IdUsuario = userId,
                Accion = "CAMBIAR_ESTADO_CITA",
                Descripcion = $"La cita ID {id} cambió a {status}",
                TablaAfectada = "Citas",
                RegistroId = id,
                Ip = null
            });
            await TryNotifyAsync(cita.IdCliente, "Actualización de cita", $"Tu cita fue {status.ToLowerInvariant()}.", cita.IdCita, status == "Aceptada" ? "cita_accepted" : "cita_declined");
            return true;
        }

        private async Task TryNotifyAsync(int recipientUserId, string title, string body, int citaId, string eventName)
        {
            try { await _pushNotificationService.NotifyAsync(recipientUserId, title, body, citaId, eventName); }
            catch { /* El envío push no debe revertir la operación de negocio. */ }
        }

    }
}
