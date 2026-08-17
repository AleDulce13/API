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

        public CitaService(
            IGenericRepository<Cita> repo,
            LogService logService,
            IPushNotificationService pushNotificationService,
            AppDbContext context)
        {
            _repo = repo;
            _logService = logService;
            _pushNotificationService = pushNotificationService;
            _context = context;
        }

        // GET ALL

        public async Task<List<Cita>> GetAll()
        {
            return await _context.Citas
                .Include(c => c.Empleado)
                .ToListAsync();
        }

        // GET BY ID
        
        public async Task<Cita> GetById(int id)
        {
            return await _context.Citas
                .Include(c => c.Empleado)
                .FirstOrDefaultAsync(c => c.IdCita == id);
        }

        // GET CITAS 

        public async Task<List<Cita>> GetAllForUser(int userId, bool isAdmin)
        {
            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.IdUsuario == userId);

            if (usuario == null || usuario.IdSucursal == null)
                return new List<Cita>();

            var empleadosSucursal = await _context.Empleados
                .Where(e =>
                    e.IdSucursal == usuario.IdSucursal &&
                    e.Estado)
                .Select(e => e.IdEmpleado)
                .ToListAsync();

            var citas = await _context.Citas
                .Include(c => c.Empleado)
                .Where(c => empleadosSucursal.Contains(c.IdEmpleado))
                .ToListAsync();

            if (isAdmin)
                return citas;

            var empleado = await _context.Empleados
                .FirstOrDefaultAsync(e =>
                    e.IdUsuario == userId &&
                    e.Estado);

            if (empleado == null)
                return new List<Cita>();

            return citas
                .Where(c => c.IdEmpleado == empleado.IdEmpleado)
                .ToList();
        }

        // GET BY ID PARA USUARIO

        public async Task<Cita?> GetByIdForUser(
            int id,
            int userId,
            bool isAdmin)
        {
            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.IdUsuario == userId);

            if (usuario == null || usuario.IdSucursal == null)
                return null;

            var empleadosSucursal = await _context.Empleados
                .Where(e =>
                    e.IdSucursal == usuario.IdSucursal &&
                    e.Estado)
                .Select(e => e.IdEmpleado)
                .ToListAsync();

            var cita = await _context.Citas
                .Include(c => c.Empleado)
                .FirstOrDefaultAsync(c => c.IdCita == id);

            if (cita == null)
                return null;

            if (!empleadosSucursal.Contains(cita.IdEmpleado))
                return null;

            if (isAdmin)
                return cita;

            var empleado = await _context.Empleados
                .FirstOrDefaultAsync(e =>
                    e.IdUsuario == userId &&
                    e.Estado);

            if (empleado == null)
                return null;

            if (cita.IdEmpleado == empleado.IdEmpleado)
                return cita;

            return null;
        }

        // CREATE

        public async Task Add(CitaDTO dto)
        {
            var empleado = await _context.Empleados
                .FirstOrDefaultAsync(e =>
                    e.IdEmpleado == dto.IdEmpleado &&
                    e.Estado);

            if (empleado == null)
            {
                throw new InvalidOperationException(
                    "El empleado seleccionado no existe o está inactivo.");
            }

            var servicio = await _context.ServicioSucursal
                .Include(item => item.Servicio)
                .SingleOrDefaultAsync(item =>
                    item.IdServicioSucursal == dto.IdServicioSucursal &&
                    item.Estado &&
                    item.Servicio.Estado);

            if (servicio == null)
            {
                throw new InvalidOperationException(
                    "El servicio seleccionado no está disponible.");
            }

            if (dto.HoraFin <= dto.HoraInicio ||
                dto.HoraFin - dto.HoraInicio !=
                TimeSpan.FromMinutes(servicio.Servicio.DuracionMinutos))
            {
                throw new InvalidOperationException(
                    "La duración de la cita no coincide con el servicio.");
            }

            var fecha = dto.Fecha.Date;

            var existeTraslape = await _context.Citas
                .AnyAsync(cita =>
                    cita.IdEmpleado == dto.IdEmpleado &&
                    cita.Fecha.Date == fecha &&
                    cita.Estado != "Declinada" &&
                    cita.HoraInicio < dto.HoraFin &&
                    dto.HoraInicio < cita.HoraFin);

            if (existeTraslape)
            {
                throw new InvalidOperationException(
                    "El horario seleccionado ya no está disponible.");
            }

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

            if (empleado.IdUsuario > 0)
            {
                await TryNotifyAsync(
                    empleado.IdUsuario,
                    "Nueva cita",
                    "Tienes una nueva cita asignada.",
                    cita.IdCita,
                    "cita_created");
            }
        }

        // HORARIOS DISPONIBLES

        public async Task<List<string>> GetHorariosDisponibles(
            int empleadoId,
            int servicioSucursalId,
            DateTime fecha)
        {
            var empleadoValido = await _context.Empleados
                .AnyAsync(e =>
                    e.IdEmpleado == empleadoId &&
                    e.Estado);

            var servicio = await _context.ServicioSucursal
                .Include(item => item.Servicio)
                .SingleOrDefaultAsync(item =>
                    item.IdServicioSucursal == servicioSucursalId &&
                    item.Estado &&
                    item.Servicio.Estado);

            if (!empleadoValido || servicio == null)
                return new List<string>();

            var horariosSucursal = await _context.HorarioLocal
                .Where(item =>
                    item.Estado &&
                    item.IdSucursal == servicio.IdSucursal)
                .OrderBy(item => item.HoraApertura)
                .ToListAsync();

            var horario = horariosSucursal
                .FirstOrDefault(item =>
                    AplicaAlDia(item.DiaSemana, fecha.DayOfWeek));

            if (horario == null)
                return new List<string>();

            var fechaUtc = DateTime.SpecifyKind(
                fecha.Date,
                DateTimeKind.Utc);

            var ocupadas = await _context.Citas
                .Where(cita =>
                    cita.IdEmpleado == empleadoId &&
                    cita.Fecha.Date == fechaUtc &&
                    cita.Estado != "Declinada")
                .ToListAsync();

            var duracion = TimeSpan.FromMinutes(
                servicio.Servicio.DuracionMinutos);

            var resultado = new List<string>();

            for (
                var inicio = horario.HoraApertura;
                inicio + duracion <= horario.HoraCierre;
                inicio += TimeSpan.FromMinutes(30))
            {
                var fin = inicio + duracion;

                var ocupado = ocupadas.Any(cita =>
                    cita.HoraInicio < fin &&
                    inicio < cita.HoraFin);

                if (!ocupado)
                {
                    resultado.Add(
                        inicio.ToString(@"hh\:mm"));
                }
            }

            return resultado;
        }

        // VALIDAR DÍA

        private static bool AplicaAlDia(
            string? diaSemana,
            DayOfWeek dia)
        {
            var texto = diaSemana?
                .Trim()
                .ToLowerInvariant() ?? string.Empty;

            if (texto.Contains("lunes a domingo"))
                return true;

            if (texto.Contains("lunes a sábado") ||
                texto.Contains("lunes a sabado"))
            {
                return dia != DayOfWeek.Sunday;
            }

            var nombres = new Dictionary<DayOfWeek, string>
            {
                [DayOfWeek.Monday] = "lunes",
                [DayOfWeek.Tuesday] = "martes",
                [DayOfWeek.Wednesday] = "miércoles",
                [DayOfWeek.Thursday] = "jueves",
                [DayOfWeek.Friday] = "viernes",
                [DayOfWeek.Saturday] = "sábado",
                [DayOfWeek.Sunday] = "domingo"
            };

            return texto.Contains(nombres[dia]) ||
                   texto.Contains(dia.ToString().ToLowerInvariant());
        }

        // UPDATE

        public async Task Update(int id, CitaDTO dto)
        {
            var cita = await _repo.GetById(id);

            if (cita == null)
            {
                throw new Exception("Cita no encontrada");
            }

            var empleado = await _context.Empleados
                .FirstOrDefaultAsync(e =>
                    e.IdEmpleado == dto.IdEmpleado &&
                    e.Estado);

            if (empleado == null)
            {
                throw new InvalidOperationException(
                    "El empleado seleccionado no existe o está inactivo.");
            }

            var servicio = await _context.ServicioSucursal
                .Include(s => s.Servicio)
                .SingleOrDefaultAsync(s =>
                    s.IdServicioSucursal == dto.IdServicioSucursal &&
                    s.Estado &&
                    s.Servicio.Estado);

            if (servicio == null)
            {
                throw new InvalidOperationException(
                    "El servicio seleccionado no está disponible.");
            }

            if (dto.HoraFin <= dto.HoraInicio ||
                dto.HoraFin - dto.HoraInicio !=
                TimeSpan.FromMinutes(servicio.Servicio.DuracionMinutos))
            {
                throw new InvalidOperationException(
                    "La duración de la cita no coincide con el servicio.");
            }

            var fecha = dto.Fecha.Date;

            var existeTraslape = await _context.Citas
                .AnyAsync(c =>
                    c.IdCita != id &&
                    c.IdEmpleado == dto.IdEmpleado &&
                    c.Fecha.Date == fecha &&
                    c.Estado != "Declinada" &&
                    c.HoraInicio < dto.HoraFin &&
                    dto.HoraInicio < c.HoraFin);

            if (existeTraslape)
            {
                throw new InvalidOperationException(
                    "El horario seleccionado ya no está disponible para ese empleado.");
            }

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
            {
                throw new Exception("Cita no encontrada");
            }

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

        // CAMBIAR ESTADO

        public async Task<bool> ChangeStatusForAssignedUser(
            int id,
            int userId,
            bool isAdmin,
            string status)
        {
            var cita = await _repo.GetById(id);

            if (cita == null)
                return false;

            if (isAdmin)
            {
                cita.Estado = status;

                await _repo.Update(cita);

                await _logService.Crear(new LogDTO
                {
                    IdUsuario = userId,
                    Accion = "CAMBIAR_ESTADO_CITA",
                    Descripcion =
                        $"La cita ID {id} cambió a {status}",
                    TablaAfectada = "Citas",
                    RegistroId = id,
                    Ip = null
                });

                await TryNotifyAsync(
                    cita.IdCliente,
                    "Actualización de cita",
                    $"Tu cita fue {status.ToLowerInvariant()}.",
                    cita.IdCita,
                    status == "Aceptada"
                        ? "cita_accepted"
                        : "cita_declined");

                return true;
            }

            var empleado = await _context.Empleados
                .FirstOrDefaultAsync(e =>
                    e.IdUsuario == userId &&
                    e.Estado);

            if (empleado == null)
                return false;

            if (cita.IdEmpleado != empleado.IdEmpleado)
                return false;

            cita.Estado = status;

            await _repo.Update(cita);

            await _logService.Crear(new LogDTO
            {
                IdUsuario = userId,
                Accion = "CAMBIAR_ESTADO_CITA",
                Descripcion =
                    $"La cita ID {id} cambió a {status}",
                TablaAfectada = "Citas",
                RegistroId = id,
                Ip = null
            });

            await TryNotifyAsync(
                cita.IdCliente,
                "Actualización de cita",
                $"Tu cita fue {status.ToLowerInvariant()}.",
                cita.IdCita,
                status == "Aceptada"
                    ? "cita_accepted"
                    : "cita_declined");

            return true;
        }

        // NOTIFICACIÓN
        private async Task TryNotifyAsync(
            int recipientUserId,
            string title,
            string body,
            int citaId,
            string eventName)
        {
            try
            {
                await _pushNotificationService.NotifyAsync(
                    recipientUserId,
                    title,
                    body,
                    citaId,
                    eventName);
            }
            catch
            {

            }
        }
    }
}
