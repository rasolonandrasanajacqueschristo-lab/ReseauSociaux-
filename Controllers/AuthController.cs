using Microsoft.AspNetCore.Mvc;
using ReseauUniversitaire.DTOs.Auth;
using ReseauUniversitaire.Services;

namespace ReseauUniversitaire.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDto dto)
    {
        var result = await _authService.RegisterAsync(dto);

        if (result == null)
            return BadRequest(new { message = "Cet email est déjà utilisé." });

        return Ok(result);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto dto)
    {
        var result = await _authService.LoginAsync(dto);

        if (result == null)
            return Unauthorized(new { message = "Email ou mot de passe incorrect." });

        return Ok(result);
    }
}