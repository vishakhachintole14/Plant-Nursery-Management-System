
using ASP.NET_PlantsOrder.Data;
using ASP.NET_PlantsOrder.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ASP.NET_PlantsOrder.Controllers
{
    public class TypesController : Controller
    {
        private readonly ApplicationDbContext context;

        public TypesController(ApplicationDbContext _context)
        {
            context = _context;
        }

        public IActionResult Index(int? categoryId)
        {
            // Get all categories
            ViewBag.Categories = context.Categories.ToList();

            // Get available plants and filter by category
            var plants = context.Plants
                .Include(p => p.Category)
                .Where(p => p.IsAvailable &&
                    (categoryId == null || p.CategoryId == categoryId))
                .ToList();

            return View(plants);
        }
    }
}
