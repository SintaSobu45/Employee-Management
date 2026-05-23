using EmployeeMVC.Data;
using EmployeeMVC.Models;
using EmployeeMVC.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EmployeeMVC.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly AppDbContext _context;
        private readonly CloudinaryService _cloudinary;

        public EmployeeController(AppDbContext context, CloudinaryService cloudinary)
        {
            _context = context;
            _cloudinary = cloudinary;
        }

        // LIST
        public IActionResult Index()
        {
            var employees = _context.Employees
                .Include(x => x.Department)
                .ToList();

            return View(employees);
        }

        // CREATE GET
        public IActionResult Create()
        {
            return View(new Employee());
        }

        // CREATE POST
        [HttpPost]
      
        public IActionResult Create(Employee employee, IFormFile imageFile)
        {
            // ⭐ STEP 1 — Ensure departments exist
            if (!_context.Departments.Any())
            {
                _context.Departments.Add(new Department { Name = "IT" });
                _context.Departments.Add(new Department { Name = "HR" });
                _context.SaveChanges();
            }

            // ⭐ STEP 2 — Upload image
            if (imageFile != null)
                employee.ImageUrl = _cloudinary.UploadImage(imageFile);

            // ⭐ STEP 3 — Save employee
            _context.Employees.Add(employee);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        // EDIT GET
        public IActionResult Edit(int id)
        {
            var emp = _context.Employees.Find(id);
            return View(emp);
        }

        [HttpPost]
        public IActionResult Edit(Employee employee, IFormFile imageFile)
        {
            // ensure departments exist
            if (!_context.Departments.Any())
            {
                _context.Departments.Add(new Department { Name = "IT" });
                _context.Departments.Add(new Department { Name = "HR" });
                _context.SaveChanges();
            }

            var old = _context.Employees.AsNoTracking()
                .FirstOrDefault(x => x.Id == employee.Id);

            if (imageFile != null)
                employee.ImageUrl = _cloudinary.UploadImage(imageFile);
            else
                employee.ImageUrl = old.ImageUrl;

            _context.Employees.Update(employee);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        // DELETE
        public IActionResult Delete(int id)
        {
            var emp = _context.Employees.Find(id);
            _context.Employees.Remove(emp);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}