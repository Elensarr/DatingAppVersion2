using System.Security.Claims;

namespace API.Extentions
{
    public static class ClaimsPrincipleExtention
    {
        public static string GetUserName(this ClaimsPrincipal user)
        {
            //whoever logged in gives name  from claim (in token)
            return user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }
    }
}
