using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication4.Application.Auth_Component.Dto;
using WebApplication4.Application.Auth_Component.IService;
using WebApplication4.Application.Common.Dtos.Dashboard;
using WebApplication4.Application.Feedback_Component.IService;
using WebApplication4.Application.Medcine_Component.IService;


namespace WebApplication4.Pressention.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly IFeedBackService _feedBackService;
        private readonly IMedicineService _medicineService;
        private readonly IAuthService _authService;
        private readonly IPatientService _patientService;
        private readonly ISupplierService _supplierService;
        private readonly IPrescriptionService _prescriptionService;
        public AdminController(
          ISupplierService supplierService,
          IPatientService patientService,
          IMedicineService medicineService,
          IPrescriptionService prescriptionService,
          IFeedBackService feedBackService,IAuthService authService)
        {
            _supplierService = supplierService;
            _patientService = patientService;
            _medicineService = medicineService;
            _prescriptionService = prescriptionService;
            _feedBackService = feedBackService;
            _authService = authService;
        }
        public async Task<IActionResult> FeedBack()
        {
            var res =await _feedBackService.GetAllAsync();
            return View(res.Data);
        }
        [HttpGet]
        public IActionResult Register() => View();

        [HttpPost]
        public async Task<IActionResult> Register(PharmacistRegisterDto dto)
        {
            if (!ModelState.IsValid) return View(dto);

            var success = await _authService.RegisterAsync(dto);
            if (success.Success)
            {
                return RedirectToAction("AdminDashboard");
            }

            ModelState.AddModelError("", success.Message);
            return View(dto);
            
        }
        [HttpGet]
        public async Task<IActionResult> AdminDashboard()
        {
            var DashboardAdmin = new DashboardViewAdmin
            {
                MedinesCountLow = await _medicineService.GetMedicinesCountAsync(),
                PharmscistCount = await _prescriptionService.GetPharmacistsCount(),
                PatientsCount = await _patientService.GetPatientCountAsync(),
                SuppliersCount = await _supplierService.GetSupplierCountAsync(),
                FeedbacksCount = await _feedBackService.GetFeedbackCountAsync(),
            };
            return View(DashboardAdmin);
        }



    }
}
