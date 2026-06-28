using System.Security.Claims;

namespace ReseauUniversitaire.Helpers;

public static class ClaimsHelper
{
    public static int GetUserId(this ClaimsPrincipal user)
    {
        var id = user.FindFirstValue(ClaimTypes.NameIdentifier);
        return int.Parse(id!);
    }
}