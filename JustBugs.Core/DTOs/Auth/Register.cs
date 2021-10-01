using System.Text.Json.Serialization;

namespace JustBugs.DTOs.Auth
{
    public class RegisterDTO
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
    }
}