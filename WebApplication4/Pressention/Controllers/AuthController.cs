using System.Web;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebApplication4.Application.Common.Dtos;
using WebApplication4.Domain.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using WebApplication4.Application.Auth_Component.Dto;
using WebApplication4.Application.Auth_Component.IService;
using Microsoft.AspNetCore.RateLimiting;

namespace WebApplication4.Pressention.Controllers
{

    public class AuthController : Controller
    {
        private readonly IAuthService _authService;
        public AuthController(IAuthService authService)
        {
            _authService = authService;
            
        }
        //[HttpGet]
        //public IActionResult Register() => View();

        //[HttpPost]
        //public async Task<IActionResult> Register(PharmacistRegisterDto dto)
        //{
        //    if (!ModelState.IsValid) return View(dto);

        //    var success = await _authService.RegisterAsync(dto);
        //    if (!success.Success)
        //    {
        //        ModelState.AddModelError("",success.Message);
        //        return View(dto);
        //    }

        //    return RedirectToAction("Login");
        //}

        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        [EnableRateLimiting("LoginRateLimit")]
        public async Task<IActionResult> Login(PharmacistLoginDto dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            var success = await _authService.LoginAsync(dto);

            if (!success.Success)
            {
                ModelState.AddModelError("", success.Message);
                return View(dto);
            }
            if (success.Message == "Account Is Block")
            {
                ModelState.AddModelError("", success.Message);
                return View(dto);
            }
           
            switch (success.Role)
            {
                case "Admin":
                    return RedirectToAction("AdminDashboard", "Admin");
                case "Pharmacist":
                    return RedirectToAction("PharmacistDashboard", "Pharmacist");
                default:
                    return RedirectToAction("Not_Found");
            }
        }
       
        [HttpGet]
        public IActionResult Not_Found() => View();
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _authService.LogoutAsync();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult ForgotPassword() => View();

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordDto model, [FromServices] IAuthService authService)
        {
            if (!ModelState.IsValid)
                return View(model);

            await authService.SendPasswordResetEmailAsync(model.Email);

            return RedirectToAction("ForgotPasswordConfirmation");
        }

        [HttpGet]
        public IActionResult ForgotPasswordConfirmation() => View();

        [HttpGet]
        public IActionResult ResetPassword(string token, string email)
        {
            if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(email))
            {
                TempData["Error"] = "Invalid reset link.";
                return RedirectToAction("ForgotPassword");
            }

            var model = new ResetPasswordDto
            {
                Token = token,
                Email = email
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordDto model, [FromServices] IAuthService authService)
        {
            if (!ModelState.IsValid) return View(model);

            var result = await authService.ResetPasswordAsync(model.Email, model.Token, model.Password);

            if (result.Succeeded)
            {
                TempData["Success"] = "Password reset successfully!";
                return RedirectToAction("Login");
            }

            
            if (result.Errors.Any(e => e.Description == "User not found."))
            {
                TempData["Error"] = "User not found.";
                return RedirectToAction("ForgotPassword");
            }

            foreach (var error in result.Errors)
                ModelState.AddModelError(string.Empty, error.Description);

            return View(model);
        }

    }
}
