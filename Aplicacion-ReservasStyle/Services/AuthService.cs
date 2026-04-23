using Aplicacion_ReservasStyle.DTOs;
using Dominio_ReservasStyle.Entities;
using Infraestructura_ReservasStyle.Repositories;
using Aplicacion_ReservasStyle.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BCrypt.Net;

namespace Aplicacion_ReservasStyle.Services
{
    public class AuthService
    {
        private readonly IGenericRepository<Usuario> _repo;
        private readonly LogService _logService;
        private static Dictionary<string, (int Intentos, DateTime? BloqueadoHasta)> _loginAttempts = new();

        public AuthService(IGenericRepository<Usuario> repo,LogService logService)
        {
            _repo = repo;
            _logService = logService;
        }

        // LISTA DE USUARIOS
        public async Task<List<Usuario>> GetUsuarios()
        {
            return await _repo.GetAll();
        }

        // REGISTRO
        public async Task Register(RegisterDTO dto, string? fotoRuta)
        {
            var usuarios = await _repo.GetAll();

            if (usuarios.Any(x => x.Email == dto.Email))
                throw new Exception("El usuario ya existe");

            var user = new Usuario
            {
                Nombre = dto.Nombre,
                Apellido = dto.Apellido,
                Email = dto.Email,
                ContrasenaHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                IdRol = dto.IdRol,
                FechaRegistro = DateTime.UtcNow,
                Estado = true,
                //IdSucursal = dto.IdSucursal,
                //Telefono = dto.Telefono,
                FotoPerfil = fotoRuta
            };

            await _repo.Add(user);

            // LOG
            await _logService.Crear(new LogDTO
            {
                IdUsuario = user.IdUsuario,
                Accion = "REGISTRO_USUARIO",
                Descripcion = $"Se registró el usuario {user.Email}",
                TablaAfectada = "Usuarios",
                RegistroId = user.IdUsuario,
                Ip = null
            });
        }

        // LOGIN
        public async Task<Usuario> Login(LoginDTO dto)
        {
            var usuarios = await _repo.GetAll();

            var user = usuarios.FirstOrDefault(x => x.Email == dto.Email);

            if (user == null)
            {
                await _logService.Crear(new LogDTO
                {
                    IdUsuario = null,
                    Accion = "LOGIN_FALLIDO",
                    Descripcion = $"Intento de login con email inexistente: {dto.Email}",
                    TablaAfectada = "Usuarios",
                    RegistroId = null,
                    Ip = null
                });

                return null;
            }

            if (_loginAttempts.ContainsKey(dto.Email))
            {
                var estado = _loginAttempts[dto.Email];

                if (estado.BloqueadoHasta.HasValue &&
                    estado.BloqueadoHasta > DateTime.UtcNow)
                {
                    throw new Exception("Usuario bloqueado temporalmente. Intenta más tarde.");
                }
            }

            if (!BCrypt.Net.BCrypt.Verify(dto.Password, user.ContrasenaHash))
            {
                if (!_loginAttempts.ContainsKey(dto.Email))
                    _loginAttempts[dto.Email] = (0, null);

                var actual = _loginAttempts[dto.Email];

                actual.Intentos++;

                // SI LLEGA A 5 INTENTOS SE BLOQUEA 30 SEGUNDOS
                if (actual.Intentos >= 5)
                {
                    _loginAttempts[dto.Email] =
                        (0, DateTime.UtcNow.AddSeconds(30));

                    await _logService.Crear(new LogDTO
                    {
                        IdUsuario = user.IdUsuario,
                        Accion = "USUARIO_BLOQUEADO",
                        Descripcion = $"Usuario bloqueado por 30 segundos por intentos fallidos",
                        TablaAfectada = "Usuarios",
                        RegistroId = user.IdUsuario,
                        Ip = null
                    });

                    throw new Exception("Demasiados intentos. Usuario bloqueado 30 segundos.");
                }
                else
                {
                    _loginAttempts[dto.Email] = actual;
                }

                await _logService.Crear(new LogDTO
                {
                    IdUsuario = user.IdUsuario,
                    Accion = "LOGIN_FALLIDO",
                    Descripcion = $"Contraseña incorrecta ({actual.Intentos} intentos)",
                    TablaAfectada = "Usuarios",
                    RegistroId = user.IdUsuario,
                    Ip = null
                });

                return null;
            }

            // LOGIN EXITOSO Y RESET INTENTOS
            if (_loginAttempts.ContainsKey(dto.Email))
                _loginAttempts.Remove(dto.Email);

            await _logService.Crear(new LogDTO
            {
                IdUsuario = user.IdUsuario,
                Accion = "LOGIN_EXITOSO",
                Descripcion = $"Login exitoso de {user.Email}",
                TablaAfectada = "Usuarios",
                RegistroId = user.IdUsuario,
                Ip = null
            });

            return user;
        }

    }
}
