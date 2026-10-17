using Microsoft.AspNetCore.Mvc;
using WebApplication4.Application.Common.Dtos.Dashboard;
using WebApplication4.Application.Modules.Feedback.Interfaces;
using WebApplication4.Application.Modules.Medicines.Interfaces;

namespace WebApplication4.Presentation.Controllers
{
    public class PharmacistController : Controller
    {
        private readonly IMedicineService _medicineService;
        private readonly IPatientService _patientService;
        private readonly ISupplierService _supplierService;
        private readonly IPrescriptionService _prescriptionService;
        public PharmacistController(
           ISupplierService supplierService,
           IPatientService patientService,
           IMedicineService medicineService,
           IPrescriptionService prescriptionService,
           IFeedBackService feedBackService)
        {
            _supplierService = supplierService;
            _patientService = patientService;
            _medicineService = medicineService;
            _prescriptionService = prescriptionService;
        }
        [HttpGet]
        public async Task<IActionResult> PharmacistDashboard()
        {
            var model = new DashboardViewModel
            {
                MedicinesCount = await _medicineService.GetMedicinesCountAsync(),
                PatientsCount = await _patientService.GetPatientCountAsync(),
                SuppliersCount = await _supplierService.GetSupplierCountAsync(),
                PrescriptionsCount = await _prescriptionService.GetPrescriptionCountAsync()
            };


            return View(model);
        }
    }
}
