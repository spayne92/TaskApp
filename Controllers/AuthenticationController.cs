using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using BaseCoreAPI.Data.Entities;
using BaseCoreAPI.Infrastructure;

namespace BaseCoreAPI.Controllers
{
    [Route("/api/[Controller]")]
    public class AuthenticationController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly UserManager<User> _userManager;
        private readonly JwtTokenConfig _jwtTokenConfig;

        public AuthenticationController(IConfiguration config, UserManager<User> userManager)
        {
            _config = config;
            _userManager = userManager;

            _jwtTokenConfig = _config.GetSection("Tokens").Get<JwtTokenConfig>();
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login(string username, string password)
        {
            var user = await _userManager.FindByNameAsync(username);

            if (user != null && await _userManager.CheckPasswordAsync(user, password))
            {
                var userRoles = await _userManager.GetRolesAsync(user);

                var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                };

                foreach (var userRole in userRoles)
                {
                    authClaims.Add(new Claim(ClaimTypes.Role, userRole));
                }

                var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtTokenConfig.Key));

                var jwtSecurityToken = new JwtSecurityToken(
                    issuer: _jwtTokenConfig.Issuer,
                    audience: _jwtTokenConfig.Audience,
                    expires: DateTime.Now.AddMinutes(_jwtTokenConfig.AccessTokenExpiration),
                    claims: authClaims,
                    signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                    );

                var token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
                return Ok(new { token, jwtSecurityToken.ValidTo });
            }

            return Unauthorized();
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register(string username, string email, string password)
        {
            var userExists = await _userManager.FindByNameAsync(username);
            if (userExists != null)
                return StatusCode(StatusCodes.Status500InternalServerError, new { Status = "Error", Message = "User already exists!" });

            User user = new()
            {
                Email = email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = username
            };

            var result = await _userManager.CreateAsync(user, password);
            if (!result.Succeeded)
                return StatusCode(StatusCodes.Status500InternalServerError, new { Status = "Error", Message = result.Errors.FirstOrDefault()?.Description });

            return Ok(new { Status = "Success", Message = "User created successfully!" });
        }

        [AllowAnonymous]
        [Route("validateToken")]
        [HttpGet]
        public IActionResult ValidateToken(string token)
        {
            IActionResult response = BadRequest("An error occured..."); ;

            if (token != null)
            {
                response = BadRequest("Invalid Token");
                if (ValidateToken(_jwtTokenConfig, token))
                {
                    response = Ok("Valid Token");
                }
            }

            return response;
        }

        private static bool ValidateToken(JwtTokenConfig jwtTokenConfig, string token)
        {
            var mySecret = Encoding.UTF8.GetBytes(jwtTokenConfig.Key);
            var mySecurityKey = new SymmetricSecurityKey(mySecret);
            var tokenHandler = new JwtSecurityTokenHandler();

            try
            {
                tokenHandler.ValidateToken(token,
                    new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidIssuer = jwtTokenConfig.Issuer,
                        ValidAudience = jwtTokenConfig.Audience,
                        IssuerSigningKey = mySecurityKey,
                    }, out SecurityToken validatedToken);
            }
            catch
            {
                return false;
            }

            return true;
        }
    }
}
