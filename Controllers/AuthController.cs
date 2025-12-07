using AccountAPI.DTOs;
using AccountAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace AccountAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _auth;

        public AuthController(AuthService auth)
        {
            _auth = auth;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            var user = await _auth.RegisterAsync(dto.Email, dto.Password);
            return Ok(new { user.Id, user.Email });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            var user = await _auth.LoginAsync(dto.Email, dto.Password);

            if (user == null)
                return Unauthorized("Invalid email or password.");

            return Ok(new { Message = "Login successful", user.Id, user.Email });
        }
    }
}
