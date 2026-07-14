using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using System.Text;
using WebApplication4.Application.Auth_Component.Dto;
using WebApplication4.Application.Auth_Component.IService;
using WebApplication4.Application.Common.Interfaces;
using WebApplication4.Domain.Models;

namespace WebApplication4.Infrastructure.Auth_Component
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<Pharmacist> _userManager;
        private readonly SignInManager<Pharmacist> _signInManager;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _configuration;

        public AuthService(
            UserManager<Pharmacist> userManager,
            SignInManager<Pharmacist> signInManager,
            IEmailService emailService,
            IConfiguration configuration)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _signInManager = signInManager ?? throw new ArgumentNullException(nameof(signInManager));
            _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public async Task<AuthResult> RegisterAsync(PharmacistRegisterDto dto)
        {
            ArgumentNullException.ThrowIfNull(dto);

            var authResult = new AuthResult();
            var user = new Pharmacist
            {
                UserName = dto.Email,
                Email = dto.Email,
                FullName = dto.FullName
            };

            var result = await _userManager.CreateAsync(user, dto.Password);

            if (!result.Succeeded)
            {
                authResult.Message = string.Join(Environment.NewLine, result.Errors.Select(e => e.Description));
                authResult.Success = false;
                return authResult;
            }

            var roleResult = await _userManager.AddToRoleAsync(user, "Pharmacist");
            if (!roleResult.Succeeded)
            {
                authResult.Message = string.Join(Environment.NewLine, roleResult.Errors.Select(e => e.Description));
                authResult.Success = false;
                return authResult;
            }

            return new AuthResult
            {
                Success = true,
                Message = "Registration was successful."
            };
        }

        public async Task<AuthResult> LoginAsync(PharmacistLoginDto dto)
        {
            ArgumentNullException.ThrowIfNull(dto);

            var user = await _userManager.FindByEmailAsync(dto.Email);

            if (user == null)
            {
                return new AuthResult
                {
                    Success = false,
                    Message = "Invalid Email Or Password"
                };
            }

           
            var result = await _signInManager.PasswordSignInAsync(user, dto.Password, isPersistent: false, lockoutOnFailure: false);

            if (!result.Succeeded)
            {
                return new AuthResult
                {
                    Success = false,
                    Message = "Invalid Email Or Password"
                };
            }

            var roles = await _userManager.GetRolesAsync(user);
            var role = roles.FirstOrDefault();

            if (string.IsNullOrEmpty(role))
            {
                return new AuthResult
                {
                    Success = false,
                    Message = "No role assigned to this user."
                };
            }

            return new AuthResult
            {
                Success = true,
                Message = "Login successful",
                Role = role
            };
        }

        public async Task LogoutAsync()
        {
            await _signInManager.SignOutAsync();
        }

        public async Task<Pharmacist?> FindUserByEmailAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return null;
            return await _userManager.FindByEmailAsync(email);
        }

        public async Task<IdentityResult> ResetPasswordAsync(string email, string token, string newPassword)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(token) || string.IsNullOrWhiteSpace(newPassword))
            {
                return IdentityResult.Failed(new IdentityError { Description = "Invalid request parameters." });
            }

            var user = await FindUserByEmailAsync(email);
            if (user == null)
            {
                return IdentityResult.Failed(new IdentityError { Description = "User not found." });
            }

            try
            {
                var decodedTokenBytes = WebEncoders.Base64UrlDecode(token);
                var decodedToken = Encoding.UTF8.GetString(decodedTokenBytes);

                return await _userManager.ResetPasswordAsync(user, decodedToken, newPassword);
            }
            catch (Exception ex) when (ex is FormatException || ex is ArgumentException)
            {
                return IdentityResult.Failed(new IdentityError { Description = "The reset token is invalid or has expired." });
            }
        }

        public async Task SendPasswordResetEmailAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return;

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return;

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            var tokenBytes = Encoding.UTF8.GetBytes(token);
            var encodedToken = WebEncoders.Base64UrlEncode(tokenBytes);

            var baseUrl = _configuration["AppUrl"]?.TrimEnd('/') ?? "https://localhost:5001";
            var resetLink = $"{baseUrl}/Auth/ResetPassword?token={encodedToken}&email={Uri.EscapeDataString(email)}";

            await _emailService.SendEmailAsync(
                email,
                "Reset Password",
                $"Click <a href='{resetLink}'>here</a> to reset your password."
            );
        }
    }
}