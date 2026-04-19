using Aplicacion_ReservasStyle.DTOs;
using Aplicacion_ReservasStyle.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;


namespace ApiReservasStyle.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _auth;
        private readonly JwtService _jwt;

        public AuthController(AuthService auth, JwtService jwt)
        {
            _auth = auth;
            _jwt = jwt;
        }

        // GET  
        [HttpGet("usuarios")]
        public async Task<IActionResult> GetUsuarios()
        {
            try
            {
                var usuarios = await _auth.GetUsuarios();
                return Ok(usuarios);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    error = ex.Message,
                    detalle = ex.InnerException?.Message
                });
            }
        }



        // REGISTRO
        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDTO dto)
        {
            try
            {
                await _auth.Register(dto);
                return Ok(new { message = "Usuario registrado" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // LOGIN
        [EnableRateLimiting("loginPolicy")]
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDTO dto)
        {
            var user = await _auth.Login(dto);

            if (user == null)
                return Unauthorized("Credenciales incorrectas");

            var token = _jwt.GenerateToken(user);

            return Ok(new
            {
                token,
                user.Email,
                user.IdRol
            });
        }
    }
}
