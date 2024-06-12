using HomeBankingV9.DTOs;
using System.Security.Claims;

namespace HomeBankingV9.Services
{
    public interface IAuthService
    {
        ClaimsIdentity IsAuthenticated(LoginDTO loginDTO);
    }
}
