using calendarProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using System.Net;


namespace calendarProject.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class loginController : Controller
    {

        private readonly connectionDB _context;
        private readonly IConfiguration _configuration;

        public loginController(connectionDB context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel loginModel)
        {

            try
            {
                var user = await _context.users
           .FirstOrDefaultAsync(u => u.userName == loginModel.userName);

                if (user == null || user.password != loginModel.password)
                {
                    return BadRequest("שם משתמש או סיסמה שגויים");
                }

                // יצירת טוקן
                var token = GenerateJwtToken(user);

                return Ok(new
                {
                    token = token,
                    userNameId = user.userNameId,
                    userName = user.userName
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "שגיאה בתהליך ההתחברות");

            }

        }

        //יצירת טוקן
        private string GenerateJwtToken(users user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: new[]
                {
                new Claim(ClaimTypes.NameIdentifier, user.userNameId.ToString()),
                new Claim(ClaimTypes.Name, user.userName)
                },
                expires: DateTime.Now.AddHours(3),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }


public class LoginModel
    {
        [Required]
        public string? userName { get; set; }

        [Required]
        public string? password { get; set; }
    }
}
