using ReseauUniversitaire.Models;

namespace ReseauUniversitaire.Services;

public interface ITokenService
{
    string GenererToken(Utilisateur utilisateur);
}