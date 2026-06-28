namespace WebApplication4.Application.Auth_Component.Dto
{
    public class RegisterResult
    {
        public bool Succeeded { get; set; }
        public IEnumerable<string> Errors { get; set; } = Enumerable.Empty<string>();
    }
}
