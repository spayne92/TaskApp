using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using BaseCoreAPI.Data;
using BaseCoreAPI.Data.DTOs;
using BaseCoreAPI.Infrastructure;
using BaseCoreAPI.Services;

namespace BaseCoreAPI.Controllers
{
    [Route("/api/[Controller]")]
    public class AuthenticationController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly IUserRepository _userRepository;
        private readonly ITokenService _tokenService;
        private readonly JwtTokenConfig _jwtTokenConfig;
        private string generatedToken = null;

        public AuthenticationController(IConfiguration config, ITokenService tokenService, IUserRepository userRepository)
        {
            _config = config;
            _tokenService = tokenService;
            _userRepository = userRepository;

            _jwtTokenConfig = _config.GetSection("Tokens").Get<JwtTokenConfig>();
        }

        [AllowAnonymous]
        [Route("login")]
        [HttpPost]
        public IActionResult LoginToSession(string userName, string password)
        {
            if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password))
            {
                return BadRequest("An error occured...");
            }

            IActionResult response = Unauthorized();
            var validUser = AuthenticateUser(userName, password);

            if (validUser != null)
            {
                generatedToken = _tokenService.BuildToken(_jwtTokenConfig, validUser);
                if (generatedToken != null)
                {
                    HttpContext.Session.SetString("Token", generatedToken);
                    response = Ok("Login Successful");
                }
            }

            return response;
        }

        [AllowAnonymous]
        [Route("getToken")]
        [HttpPost]
        public IActionResult GetToken(string userName, string password)
        {
            if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password))
            {
                return BadRequest("An error occured...");
            }

            IActionResult response = Unauthorized();
            var validUser = AuthenticateUser(userName, password);

            if (validUser != null)
            {
                generatedToken = _tokenService.BuildToken(_jwtTokenConfig, validUser);
                if (generatedToken != null)
                {
                    response = Ok(generatedToken);
                }
            }

            return response;
        }

        [AllowAnonymous]
        [Route("validateToken")]
        [HttpGet]
        public IActionResult ValidateToken(string token)
        {
            if (token == null)
            {
                return BadRequest("An error occured...");
            }

            IActionResult response = BadRequest("Invalid Token");

            if (_tokenService.ValidateToken(_jwtTokenConfig, token))
            {
                response = Ok("Valid Token");
            }

            return response;
        }

        [AllowAnonymous]
        [Route("checkStatus")]
        [HttpGet]
        public IActionResult CheckSessionStatus()
        {
            string sessionToken = HttpContext.Session.GetString("Token");
            IActionResult response = BadRequest("Session Expired");

            if (sessionToken == null || sessionToken == string.Empty)
            {
                response = BadRequest("No Session Found");
            }

            if (_tokenService.ValidateToken(_jwtTokenConfig, sessionToken))
            {
                response = Ok("Session Valid");
            }

            return response;
        }

        private UserDTO AuthenticateUser(string userName, string password)
        {
            // Make more secure later.   
            return _userRepository.GetUser(userName, password);
        }
    }
}
