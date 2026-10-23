using Inventory.Api.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Inventory.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        /// <summary>
        /// Inicializa una nueva instancia de la clase AuthController con la configuración especificada.
        /// </summary>
        /// <param name="configuration">Configuración de la aplicación.</param>
        public AuthController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        //Post: api/auth/login
        /// <summary>
        /// Autentica un usuario y genera un token JWT si las credenciales son válidas.
        /// </summary>
        /// <param name="request">Datos de inicio de sesión proporcionados por el usuario.</param>
        /// <returns>Un objeto que contiene el token JWT si la autenticación es exitosa; de lo contrario, un código de error adecuado.</returns>
        [AllowAnonymous]
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequestDto request)
        {
            var expectedUsername = _configuration["Jwt:DemoUser"];
            var expectedPassword = _configuration["Jwt:DemoPassword"];

            if (string.IsNullOrWhiteSpace(expectedUsername) || string.IsNullOrWhiteSpace(expectedPassword))
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "La autenticación no está configurada." });
            }

            var usernameMatches = string.Equals(request.Username, expectedUsername, StringComparison.Ordinal);

            var providedPassword = Encoding.UTF8.GetBytes(request.Password);
            var configuredPassword = Encoding.UTF8.GetBytes(expectedPassword);

            var passwordMatches = providedPassword.Length == configuredPassword.Length && CryptographicOperations.FixedTimeEquals(providedPassword, configuredPassword);

            if (!usernameMatches || !passwordMatches)
            {
                return Unauthorized(new { message = "Usuario o contraseña incorrectos." });
            }

            var key = _configuration["Jwt:Key"];
            var issuer = _configuration["Jwt:Issuer"];
            var audience = _configuration["Jwt:Audience"];

            if (string.IsNullOrWhiteSpace(key) || string.IsNullOrWhiteSpace(issuer) || string.IsNullOrWhiteSpace(audience))
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "La configuración JWT está incompleta." });
            }

            var now = DateTime.UtcNow;
            var expires = now.AddMinutes(60);

            var claims = new[] { new Claim(ClaimTypes.NameIdentifier, expectedUsername), new Claim(ClaimTypes.Name, expectedUsername) };

            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));

            var credentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken
                (
                    issuer: issuer,
                    audience: audience,
                    claims: claims,
                    notBefore: now,
                    expires: expires,
                    signingCredentials: credentials
                );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            return Ok(new LoginResponseDto { AccessToken = tokenString, TokenType = "Bearer", ExpiresAtUtc = expires });
        }
    }
}
