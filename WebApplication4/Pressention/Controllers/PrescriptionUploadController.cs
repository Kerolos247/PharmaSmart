using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using WebApplication4.Application.PrescriptionUpload_Component.PrescriptionUpload;

namespace WebApplication4.Pressention.Controllers
{
    public class PrescriptionUploadController : Controller
    {
        private readonly IPrescriptionUploadService _service;
        public PrescriptionUploadController(IPrescriptionUploadService service)
        {
            _service = service;
        }
        [HttpGet]
        public IActionResult Upload()
        {
           
            var model = new PrescriptionUploadDto();
            return View(model);
        }
        [HttpPost]
        [EnableRateLimiting("UploadRateLimit")]
        public async Task<IActionResult> Upload(PrescriptionUploadDto prescriptionUploadDto)
        {
            if(!ModelState.IsValid)
            {
                return View(prescriptionUploadDto);
            }
            var res=await _service.UploadPrescriptionAsync(prescriptionUploadDto);
            if (res.IsSuccess)
            {
                TempData["UploadMessage"] = "فريقنا الصيدلي يراجع الطلب الآن وسنتواصل معك قريبًا. شكراً لثقتك";
                return RedirectToAction("Index","Home");
            }
            else
            {
                TempData["UploadMessage"] = res.ErrorMessage;
                return View(prescriptionUploadDto);
            }
        }
        [HttpGet]
        public async Task<IActionResult> GetAllPrescriptionUploaded(int page = 1)
        {
            int pageSize = 4; 

            var res = await _service.GetAllPrescriptionsAsync();

            var data = res.Data
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling(res.Data.Count / (double)pageSize);

            return View(data);
        }
        [HttpPost]
        public async Task<IActionResult> DeletePrescription(int id)
        {
            var res = await _service.DeletePrescriptionAsync(id);
            if (res.IsSuccess)
            {
                TempData["DeleteMessage"] = "Prescription deleted successfully";
            }
            else
            {
                TempData["DeleteMessage"] = res.ErrorMessage;
            }
            return RedirectToAction("GetAllPrescriptionUploaded");
        }
    }
}
