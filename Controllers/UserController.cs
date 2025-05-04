using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using studentsapi.DTO;

namespace studentsapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class UserController : ControllerBase
    {
        private readonly ILogger<StudentsController> _logger;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        public UserController(ILogger<StudentsController> logger, IMapper mapper, IConfiguration configuration)
        {
            _logger = logger;
            _mapper = mapper;
            _configuration = configuration;
        }

        [HttpPost]
        public ActionResult Login(LoginModelDto model)
        {
            if(!ModelState.IsValid)
            {
               return BadRequest("Please Provide UserName and Password");
            }
            if (string.IsNullOrEmpty(model.Email) || string.IsNullOrEmpty(model.Password))
            {
                return BadRequest("Please Provide UserName and Password");
            }

            byte[] key = null;
            if (model.Policy == "Local")
            {
                key = Encoding.ASCII.GetBytes(_configuration.GetValue<string>("JWTSecretLocal"));
            } else if (model.Policy == "LoginforAppUSers")
            {
                key = Encoding.ASCII.GetBytes(_configuration.GetValue<string>("JWTSecret"));
            }


            LoginResponseDto response = new() { Email = model.Email };
            if (model.Email == "admin@email.cz")
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new Claim[]
                    {
                        new Claim(ClaimTypes.Name, model.Email),
                        new Claim(ClaimTypes.Role, "Admin")

                    }),
                    Expires = DateTime.Now.AddHours(1),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };
                var token = tokenHandler.CreateToken(tokenDescriptor);
                response.Token = tokenHandler.WriteToken(token);
            }
            else
            {
                return Unauthorized("Invalid UserName or Password");
            }
            return Ok(response);
        }

    }
}
