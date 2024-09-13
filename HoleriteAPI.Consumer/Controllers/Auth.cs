using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HoleriteAPI.Consumer.Controllers
{
    [ApiController]
    [Route("auth")]
    public class AuthController : Controller
    {
        [HttpGet("login")]
        [AllowAnonymous]
        public async Task<IActionResult> LoginAsync()
        {
            // Troca o código de autorização por um token de acesso
            var result = await HttpContext.AuthenticateAsync(GoogleDefaults.AuthenticationScheme);
            var externalUser = result.Principal;

            // Cria um claims principal para o usuário
            var claims = new List<Claim>
            {
                new Claim("external_user_id", externalUser.FindFirstValue(ClaimTypes.NameIdentifier))
            };
            var identity = new ClaimsIdentity(claims, "Google");
            ClaimsPrincipal principal = new ClaimsPrincipal(identity);

            // Gera um token JWT (você pode personalizar a geração)
            var token = GenerateJwtToken(principal);

            return Ok(new { token });
        }

        private string GenerateJwtToken(ClaimsPrincipal principal)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes("your_secret_key"); // Substitua pela sua chave secreta
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = principal,
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)

            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);

        }
    }

}