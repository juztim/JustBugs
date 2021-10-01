using System.Threading.Tasks;
using JustBugs.Database;
using JustBugs.Database.Models;
using JustBugs.DTOs.Auth;
using JustBugs.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JustBugs.Controllers
{
    [Route("/api/auth")]
    [ApiController]
    public class AuthController : Controller
    {
        private readonly JustBugsContext _dbContext;
        private readonly AuthService _authService;
        private readonly JwtService _jwtService;
        public AuthController(JustBugsContext dbContext, AuthService authService, JwtService jwtService)
        {
            _dbContext = dbContext;
            _authService = authService;
            _jwtService = jwtService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDTO registerData)
        {
            if (registerData == null) return BadRequest("missing credentials");
            if (registerData.Username == null) return BadRequest("missing Username");
            if (registerData.Email == null) return BadRequest("missing Email");
            if (registerData.Password == null) return BadRequest("missing Password");
            await _authService.CreateUser(registerData.Username, registerData.Password, registerData.Email);
            return Created("success", registerData.Username);
        }
        
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDTO loginData)
        {
            if (loginData == null) return BadRequest("missing credentials");
            if (loginData.Username == null) return BadRequest("missing username");
            if (loginData.Password == null) return BadRequest("missing password");
            
            var dbUser = await _dbContext.Users.FirstOrDefaultAsync(u => u.Username == loginData.Username);
            if (dbUser == null) return BadRequest(new {message = "User not found!"});
            if (!BCrypt.Net.BCrypt.Verify(loginData.Password, dbUser.Password)) return BadRequest("Invalid Password!");

            var jwt = _jwtService.Generate(dbUser.Id);
            
            Response.Cookies.Append("jwt", jwt, new CookieOptions
            {
                HttpOnly = true
            });
            
            return Ok(new
            {
                message = "success"
            });
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            Response.Cookies.Delete("jwt");
            return Ok("success");
        }
        
    }
}