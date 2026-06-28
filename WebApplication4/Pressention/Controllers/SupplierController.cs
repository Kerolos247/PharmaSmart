using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication4.Application.Supplier_Component.Supplier;
using WebApplication4.Domain.Models;

namespace WebApplication4.Pressention.Controllers
{
    [Authorize]
    public class SupplierController : Controller
    {
        private readonly ISupplierService _supplierService;

        public SupplierController(ISupplierService supplierService)
        {
            _supplierService = supplierService;
        }

        public async Task<IActionResult> Index()
        {
            var suppliers = await _supplierService.GetAllSuppliersAsync();
            return View(suppliers.Data);
        }

        public async Task<IActionResult> Details(int id)
        {
            var supplier = await _supplierService.GetByIdAsync(id);
            if (supplier == null)
                return NotFound();
            return View(supplier.Data);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RequestCreateSupplier dto)
        {
            if (!ModelState.IsValid) 
                return View(dto);

            var res = await _supplierService.CreateAsync(dto);
            if (!res.IsSuccess)
                TempData["CreatedMessage"] = res.ErrorMessage;
            else
                TempData["CreatedMessage"] = "Supplier created successfully";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var supplier = await _supplierService.GetByIdAsync(id);
            if (supplier == null)
                return NotFound();

            var dto = new UpdateSupplierDto
            {
                Name = supplier.Data?.Name,
                Phone = supplier.Data?.Phone,
                Email = supplier.Data?.Email
            };
            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UpdateSupplierDto dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            var res = await _supplierService.UpdateAsync(id, dto);
            if (!res.IsSuccess)
                TempData["UpdateMessage"]=res.ErrorMessage;
            else
                TempData["UpdateMessage"] = "Supplier updated successfully";
            

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var supplier = await _supplierService.GetByIdAsync(id);
            if (supplier == null)
                return NotFound();
            return View(supplier);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var res = await _supplierService.DeleteAsync(id);
            if (!res.IsSuccess)
                return View("No_Delete");
            else
                TempData["DeleteMessage"] = "Supplier deleted successfully";


            return RedirectToAction(nameof(Index));
        }
    }
}
