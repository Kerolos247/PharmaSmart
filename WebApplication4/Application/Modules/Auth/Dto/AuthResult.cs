namespace WebApplication4.Application.Modules.Auth.Dto
{
    public class AuthResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }

        public string? Role { get; set; }
    }
}
