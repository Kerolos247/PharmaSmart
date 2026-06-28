using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using WebApplication4.Application.Common.PaymentStrategies;
using WebApplication4.Application.Common.Results;
using WebApplication4.Application.Prescription_Component.Prescription;
using WebApplication4.Domain.Models;

namespace WebApplication4.Pressention.Controllers
{
    [Authorize]
    public class PrescriptionController : Controller
    {
        private readonly IPrescriptionService _prescriptionService;
        private readonly IPatientService _patientService;

        public PrescriptionController(IPrescriptionService prescriptionService, IPatientService patientService)
        {
            _prescriptionService = prescriptionService;
            _patientService = patientService;
        }

        public async Task<IActionResult> Index()
        {
            var prescriptions = await _prescriptionService.GetAllPrescriptionsAsync();
            return View(prescriptions.Data);
        }

        public async Task<IActionResult> Details(int id)
        {
            var prescription = await _prescriptionService.GetByIdAsync(id);
            if (prescription == null)
                return NotFound();
            return View(prescription.Data);
        }

        public async Task<IActionResult> Create()
        {
            var patients = await _patientService.GetAllPatientsAsync();
            ViewBag.Patients = new SelectList(patients.Data, "PatientId", "FullName");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RequestCreatePrescription dto)
        {
            if (!ModelState.IsValid)
            {
                var Patients = await _patientService.GetAllPatientsAsync();
                ViewBag.Patients = new SelectList(Patients.Data, "PatientId", "FullName");
                return View(dto);
            }

            dto.PharmacistId = User.FindFirst(ClaimTypes.NameIdentifier).Value;

            var res = await _prescriptionService.CreateAsync(dto);
            if (!res.IsSuccess)
                TempData["CreatedMessage"] = res.ErrorMessage;
            else
                TempData["CreatedMessage"] = "Prescription created successfully";

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var prescription = await _prescriptionService.GetByIdAsync(id);
            if (prescription == null) return NotFound();

            var dto = new UpdatePrescriptionDto
            {
                Date = prescription.Data?.Date,
                Notes = prescription.Data?.Notes,
                PatientId = prescription.Data?.PatientId
            };

            var patients = await _patientService.GetAllPatientsAsync();
            ViewBag.Patients = new SelectList(patients.Data, "PatientId", "FullName", dto.PatientId);

            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UpdatePrescriptionDto dto)
        {
            if (!ModelState.IsValid)
            {
                return View(dto);
            }

            var claim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (claim == null)
                return Unauthorized();

            Result<bool> updated = await _prescriptionService.UpdateAsync(id, dto, claim.Value);

            if (!updated.IsSuccess)
            {
                TempData["UpdateMessage"] = updated.ErrorMessage;
                return View(dto);
            }

            TempData["UpdateMessage"] = "Prescription updated successfully";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var prescription = await _prescriptionService.GetByIdAsync(id);
            if (prescription == null) return NotFound();

            TempData["DeleteMessage"] = "Prescription deleted successfully";
            return View(prescription.Data);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _prescriptionService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Pay(int id)
        {
            IDiscountStrategy payment = PaymentFactory.GetStrategy();
            var success = await _prescriptionService.PayAsync(id, payment);

            if (!success.IsSuccess)
            {
               
                TempData["PaymentMessage"] = success.ErrorMessage;
                return RedirectToAction(nameof(Index));
            }

            TempData["PaymentMessage"] = "Payment successful";
            TempData["TotalCost"] = success.Data.Cost.ToString("F2");
            return RedirectToAction(nameof(Index));
        }
    }
}
