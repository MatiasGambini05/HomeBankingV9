using HomeBankingV9.Models;
using HomeBankingV9.Repositories.Implementations;
using HomeBankingV9.Repositories;
using HomeBankingV9.DTOs;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;


namespace HomeBankingV9.Services.Implementations
{
    public class AuthService :  IAuthService
    {
        private readonly IClientRepository _clientRepository;

        public AuthService(IClientRepository clientRepository)
        {
            _clientRepository = clientRepository;
        }

        public ClaimsIdentity IsAuthenticated(LoginDTO loginDTO)
        {
            Client user = _clientRepository.FindClientByEmail(loginDTO.Email);

            if (user == null || !BCrypt.Net.BCrypt.Verify(loginDTO.Password, user.Password))
                throw new Exception("Datos incorrectos.");

            var claims = new List<Claim>
                {
                    new Claim("Client", user.Email)
                };

            if (user.Email.Equals("mati@gmail.com"))
            {
                claims.Add(new Claim("Admin", "true"));
            }

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            return claimsIdentity;
        }
    }
}
