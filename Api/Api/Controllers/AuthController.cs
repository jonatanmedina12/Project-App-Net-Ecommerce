using System;
using System.Threading.Tasks;
using Core.DTOs;
using Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{

    public class AuthController : BaseController
    {
        private readonly AuthService _authService;

        public AuthController(AuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto registerDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new { message = "Datos de registro no válidos.", errors = ModelState });
                }

                var user = await _authService.RegisterAsync(registerDto.Username, registerDto.Email, registerDto.Password);

                if (user == null)
                {
                    return BadRequest(new { message = "El registro falló. Es posible que el nombre de usuario o el correo electrónico ya estén en uso." });
                }

                return Ok(new { message = "Usuario registrado correctamente", user = new { user.Username, user.Email } });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Se produjo un error durante el registro", error = ex.Message });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new { message = "\r\nDatos de inicio de sesión no válidos.", errors = ModelState });
                }

                var token = await _authService.LoginAsync(loginDto.Username, loginDto.Password);

                if (string.IsNullOrEmpty(token))
                {
                    return Unauthorized(new { message = "Nombre de usuario o contraseña no válidos." });
                }

                return Ok(new { message = "Incio de sesión exitosamente", token = token });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Se produjo un error durante el inicio de sesión.", error = ex.Message });
            }
        }
    }
}