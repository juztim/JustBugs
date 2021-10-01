using System;
using System.Threading.Tasks;
using JustBugs.Database;
using JustBugs.Database.Models;
using JustBugs.DTOs.StatusCodes;
using Microsoft.EntityFrameworkCore;

namespace JustBugs.Services
{
    public class AuthService
    {
        private readonly JustBugsContext _dbContext;
        private readonly JwtService _jwtService;
        
        public AuthService(JustBugsContext dbContext)
        {
            _dbContext = dbContext;
        }
        
        public async Task<RegisterResult> CreateUser(string username, string password, string email)
        {
            var usernameExists = await _dbContext.Users.FirstOrDefaultAsync(u => u.Username == username);
            if (usernameExists != null) return RegisterResult.UsernameExists;

            var emailExists = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (emailExists != null) return RegisterResult.EmailExists;

            _dbContext.Users.Add(new User
            {   
                Username = username,
                Email = email,
                Password = BCrypt.Net.BCrypt.HashPassword(password)
            });

            await _dbContext.SaveChangesAsync();

            return RegisterResult.Success;
        }
        
        public async Task<User> GetUserByToken(string authToken)
        {
            var token = _jwtService.Verify(authToken);
            var userId = Guid.Parse(token.Issuer);
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId);
            return user;
        }
    }
}