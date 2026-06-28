using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApplication4.Application.Medcine_Component.IService;
using WebApplication4.Application.Medcine_Component.Medcine;
using WebApplication4.Domain.Models;

namespace WebApplication4.Pressention.Controllers
{
    [Authorize]
    public class MedicineController : Controller
    {
        private readonly IMedicineService _medicineService;
        private readonly ISupplierService _supplierService;
        private readonly ICategoryService _categoryService;

        public MedicineController(
            IMedicineService medicineService,
            ICategoryService categoryService,
            ISupplierService supplierService)
        {
            _medicineService = medicineService;
            _categoryService = categoryService;
            _supplierService = supplierService;
        }

        public async Task<IActionResult> Index()
        {
            var medicines = await _medicineService.GetAllMedicinesAsync();
            return View(medicines.Data);
        }

        public async Task<IActionResult> Details(int id)
        {
            var medicine = await _medicineService.GetByIdAsync(id);
            if (medicine == null)
                return NotFound();
            return View(medicine.Data);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var Category = await _categoryService.GetAllCategoriesAsync();
            var Supplier = await _supplierService.GetAllSuppliersAsync();
            ViewBag.Categories = new SelectList(Category.Data,"CategoryId", "Name");
            ViewBag.Suppliers = new SelectList(Supplier.Data, "SupplierId", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RequestCreateMedcine dto)
        {
            if (!ModelState.IsValid)
            {
                var Category = await _categoryService.GetAllCategoriesAsync();
                var Supplier = await _supplierService.GetAllSuppliersAsync();
                ViewBag.Categories = new SelectList(Category.Data, "CategoryId", "Name");
                ViewBag.Suppliers = new SelectList(Supplier.Data, "SupplierId", "Name");
                return View(dto);
            }

            var res =await _medicineService.CreateAsync(dto);
            if (!res.IsSuccess)
                TempData["CreatedMessage"] = res.ErrorMessage;
            else
                TempData["CreatedMessage"] = "Medicine created successfully";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var medicine = await _medicineService.GetByIdAsync(id);
            if (medicine == null)
                return NotFound();

            var inventory = medicine.Data?.Inventory;
            var Category = await _categoryService.GetAllCategoriesAsync();
            var Supplier = await _supplierService.GetAllSuppliersAsync();

            var dto = new UpdateMedcineDto
            {
                Name = medicine.Data?.Name,
                Description = medicine.Data?.Description,
                DosageForm = medicine.Data?.DosageForm,
                Strength = medicine.Data?.Strength,
                Price = medicine.Data?.Price,
                CategoryId = medicine.Data?.CategoryId,
                SupplierId = medicine.Data?.SupplierId,
                Quantity = inventory?.Quantity,
                ExpiryDate = inventory?.ExpiryDate
            };

            ViewBag.Categories = new SelectList(
                Category.Data,
                "CategoryId",
                "Name",
                dto.CategoryId
            );

            ViewBag.Suppliers = new SelectList(
                Supplier.Data,
                "SupplierId",
                "Name",
                dto.SupplierId
            );

            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UpdateMedcineDto dto)
        {
            if (!ModelState.IsValid)
            {
                var Category = await _categoryService.GetAllCategoriesAsync();
                var Supplier = await _supplierService.GetAllSuppliersAsync();
                ViewBag.Categories = new SelectList(Category.Data, "CategoryId", "Name", dto.CategoryId);
                ViewBag.Suppliers = new SelectList(Supplier.Data, "SupplierId", "Name", dto.SupplierId);
                return View(dto);
            }

            var res = await _medicineService.UpdateAsync(id, dto);
            if (!res.IsSuccess)
                TempData["UpdatedMessage"] = res.ErrorMessage;
            else
                TempData["UpdatedMessage"] = "Medicine updated successfully";

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var medicine = await _medicineService.GetByIdAsync(id);
            if (medicine == null) return NotFound();
            return View(medicine.Data);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var res = await _medicineService.DeleteAsync(id);
                if (!res.IsSuccess)
                    TempData["DeletedMessage"] = res.ErrorMessage;
                else
                    TempData["DeletedMessage"] = "Medicine deleted successfully!";

                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException ex)
            {
                var medicine = await _medicineService.GetByIdAsync(id);
                if (medicine == null)
                    return NotFound();

                return NotFound();
            }
        }
    }
}
