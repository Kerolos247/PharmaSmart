using Microsoft.AspNetCore.Identity;
using WebApplication4.Application.Auth_Component.Dto;
using WebApplication4.Domain.Models;
namespace WebApplication4.Application.Auth_Component.IService
{
    public interface IAuthService
    {

        Task<AuthResult> RegisterAsync(PharmacistRegisterDto dto);
        Task<AuthResult> LoginAsync(PharmacistLoginDto dto);
        Task LogoutAsync();

        Task<IdentityResult> ResetPasswordAsync(string email, string token, string newPassword);
        Task<Pharmacist?> FindUserByEmailAsync(string email);

        Task SendPasswordResetEmailAsync(string email);
    }
}
