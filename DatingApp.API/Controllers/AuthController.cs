using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using DatingApp.API.Data;
using DatingApp.API.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace DatingApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _repo;
        private readonly IConfiguration _config;

        public AuthController(IAuthRepository repo, IConfiguration config)
        {
            _repo = repo;
            _config = config;
        }

        [HttpPost("register")]
        //public async Task<IActionResult> Register(string username, string password)
        public async Task<IActionResult> Register(/*[FromBody]*/UserForRegisterDto dto)
        {
            // Validate Request (only needed if the controller do not have [ApiController])
            // if(!ModelState.IsValid)
            // {
            //     return BadRequest(ModelState);
            // }

            dto.Username = dto.Username.ToLowerInvariant();

            if(await _repo.UserExists(dto.Username))
            {
                return BadRequest("Username already exists");
            }
            else
            {
                Models.User user = await _repo.Register(new Models.User() { Username = dto.Username }, dto.Password);
                //return CreatedAtRoute("User/{id}")
                return StatusCode(201);
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserForLoginDto dto)
        {
            var usr = await _repo.Login(dto.Username.ToLowerInvariant(), dto.Password);

            if(usr == null)
            {
                return Unauthorized();
            }

            var claims = new[]{
                new Claim(ClaimTypes.NameIdentifier, usr.Id.ToString()),
                new Claim(ClaimTypes.Name, usr.Username)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.GetSection("AppSettings:Token").Value));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = creds
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return Ok(new {
                token = tokenHandler.WriteToken(token)
            });
        }
    }
}