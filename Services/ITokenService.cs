using BaseCoreAPI.Data.DTOs;
using BaseCoreAPI.Infrastructure;

namespace BaseCoreAPI.Services
{
    public interface ITokenService
    {
        string BuildToken(JwtTokenConfig jwtTokenConfig,  UserDTO user);
        bool ValidateToken(JwtTokenConfig jwtTokenConfig, string token);
    }
}
