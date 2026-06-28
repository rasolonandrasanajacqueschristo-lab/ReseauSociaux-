using ReseauUniversitaire.DTOs.Auth;

namespace ReseauUniversitaire.Services;

public interface IAuthService
{
    Task<TokenDto?> RegisterAsync(RegisterDto dto);
    Task<TokenDto?> LoginAsync(LoginDto dto);
}