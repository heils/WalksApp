using APPAPI.Models.Dto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace APPAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly IConfiguration configuration;

        public AuthController(UserManager<IdentityUser> userManager, IConfiguration configuration)
        {
            this.userManager = userManager;
            this.configuration = configuration;
        }
        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserDto registerUserDto)
        {
            var identityUser = new IdentityUser
            {
                UserName = registerUserDto.Username,
                Email = registerUserDto.Username
            };
            var identityResult = await userManager.CreateAsync(identityUser, registerUserDto.Password);

            if(identityResult.Succeeded)
            {
                if(registerUserDto.Roles != null && registerUserDto.Roles.Any())
                {
                    identityResult = await userManager.AddToRolesAsync(identityUser, registerUserDto.Roles);
                    if(identityResult.Succeeded)
                    {
                        return Ok("User registered, please login.");
                    }

                }

            }
            var errors = identityResult.Errors.Select(e => e.Description);
            return BadRequest(new { errors });
        }
        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            var user = await userManager.FindByEmailAsync(loginDto.Username);
            if(user != null)
            {
                var checkResult = await userManager.CheckPasswordAsync(user, loginDto.Password);
                if (checkResult)
                {
                    var claims = new List<Claim>();
                    claims.Add(new Claim(ClaimTypes.Email, user.Email));
                    var roles = await userManager.GetRolesAsync(user);
                    if(roles == null)  return BadRequest(); 
                    foreach (var role in roles)
                    {
                        claims.Add(new Claim(ClaimTypes.Role, role));
                    }
                    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]));
                    var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                    var token = new JwtSecurityToken(
                        configuration["Jwt:Issuer"],
                        configuration["Jwt:Audience"],
                        claims,
                        expires: DateTime.Now.AddHours(5),
                        signingCredentials: credentials);
                    var tokenHandler = new JwtSecurityTokenHandler();
                    var stringToken = tokenHandler.WriteToken(token);
                    var response = new LoginResponseDto
                    {
                        JwtToken = stringToken
                    };
                    return Ok(response);
                }
            }
            return BadRequest("Username or password incorrect");
        }
    }
}
