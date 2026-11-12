
using ASP.NET_PlantsOrder.Data;
using ASP.NET_PlantsOrder.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ASP.NET_PlantsOrder.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        private bool IsAdmin()
        {
            return HttpContext.Session.GetString("IsAdmin") == "True";
        }

        // ADMIN DASHBOARD
        public IActionResult Dashboard()
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Account");

            ViewBag.TotalOrders = _context.Orders.Count();
            ViewBag.TotalCustomers = _context.Customers.Count();
            ViewBag.TotalPlants = _context.Plants.Count();
            ViewBag.TotalCategories = _context.Categories.Count();

            return View();
        }


        public IActionResult OrderDetails(int id)
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Account");

            var order = _context.Orders
     .Include(o => o.Customer)
     .Include(o => o.OrderItems)
         .ThenInclude(oi => oi.Plant)
     .FirstOrDefault(o => o.OrderId == id);

            if (order == null)
                return NotFound();

            return View(order);
        }


        // MANAGE PLANTS
        public IActionResult ManagePlants(
            int? categoryId,
            string? searchString,
            bool? isAvailable,
            decimal? minPrice,
            decimal? maxPrice,
            string? sortOrder)
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Account");

            ViewBag.CurrentSort = sortOrder;

            ViewBag.NameSortParam =
                sortOrder == "name_desc" ? "name_asc" : "name_desc";

            ViewBag.PriceSortParam =
                sortOrder == "price_asc" ? "price_desc" : "price_asc";

            ViewBag.CategorySortParam =
                sortOrder == "category" ? "category_desc" : "category";

            var plants = _context.Plants
                .Include(p => p.Category)
                .AsQueryable();

            if (categoryId.HasValue && categoryId.Value > 0)
            {
                plants = plants.Where(
                    p => p.CategoryId == categoryId.Value);

                ViewBag.CurrentCategory = categoryId.Value;
            }

            if (!string.IsNullOrWhiteSpace(searchString))
            {
                plants = plants.Where(
                    p => p.PlantName.Contains(searchString));

                ViewBag.CurrentSearch = searchString;
            }

            if (isAvailable.HasValue)
            {
                plants = isAvailable.Value
                    ? plants.Where(p => p.Quantity > 0)
                    : plants.Where(p => p.Quantity <= 0);

                ViewBag.CurrentAvailability = isAvailable.Value;
            }

            if (minPrice.HasValue)
            {
                plants = plants.Where(p => p.Price >= minPrice.Value);
                ViewBag.MinPrice = minPrice.Value;
            }

            if (maxPrice.HasValue)
            {
                plants = plants.Where(p => p.Price <= maxPrice.Value);
                ViewBag.MaxPrice = maxPrice.Value;
            }

            plants = sortOrder switch
            {
                "name_asc" => plants.OrderBy(p => p.PlantName),
                "name_desc" => plants.OrderByDescending(p => p.PlantName),
                "price_asc" => plants.OrderBy(p => p.Price),
                "price_desc" => plants.OrderByDescending(p => p.Price),
                "category" => plants.OrderBy(p => p.Category!.CategoryName),
                "category_desc" => plants.OrderByDescending(p => p.Category!.CategoryName),
                _ => plants.OrderBy(p => p.CategoryId)
                      .ThenBy(p => p.PlantName)
            };

            ViewBag.TotalPlants = _context.Plants.Count();
            ViewBag.AvailablePlants =
                _context.Plants.Count(p => p.Quantity > 0);
            ViewBag.UnavailablePlants =
                _context.Plants.Count(p => p.Quantity <= 0);
            ViewBag.CategoryCount = _context.Categories.Count();
            ViewBag.Categories = _context.Categories.ToList();

            return View("ManagePlants", plants.ToList());
        }

        // CREATE PLANT - GET
        [HttpGet]
        public IActionResult CreatePlant()
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Account");

            ViewBag.Categories = _context.Categories.ToList();

            return View();
        }

        // CREATE PLANT - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreatePlant(Plants plant)
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Account");

            if (!string.IsNullOrWhiteSpace(plant.PlantName))
            {
                bool exists = _context.Plants.Any(p =>
                    p.PlantName.ToLower() == plant.PlantName.ToLower());

                if (exists)
                {
                    ModelState.AddModelError(
                        "PlantName",
                        "Plant already exists. Cannot register duplicate item.");
                }
            }

            if (ModelState.IsValid)
            {
                _context.Plants.Add(plant);
                _context.SaveChanges();

                TempData["Success"] = "Plant added successfully!";

                return RedirectToAction("ManagePlants");
            }

            ViewBag.Categories = _context.Categories.ToList();
            return View(plant);
        }

        // TOGGLE PLANT AVAILABILITY
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ToggleAvailability(int id)
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Account");

            var plant = _context.Plants.Find(id);

            if (plant == null)
                return NotFound();

            // Note: This sets quantity to 0 or 1.
            if (plant.Quantity > 0)
                plant.Quantity = 0;
            else
                plant.Quantity = 1;

            _context.SaveChanges();

            TempData["Success"] =
                $"{plant.PlantName} availability updated successfully.";

            return RedirectToAction("ManagePlants");
        }

        // EDIT PLANT - GET
        [HttpGet]
        public IActionResult EditPlant(int id)
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Account");

            var plant = _context.Plants.Find(id);

            if (plant == null)
                return NotFound();

            ViewBag.Categories = _context.Categories.ToList();

            return View(plant);
        }

        // EDIT PLANT - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditPlant(
            [Bind("PlantId,PlantName,Price,Quantity,CategoryId,ImageUrl")]
            Plants plant)
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Account");

            if (ModelState.IsValid)
            {
                var existingPlant =
                    _context.Plants.Find(plant.PlantId);

                if (existingPlant == null)
                    return NotFound();

                existingPlant.PlantName = plant.PlantName;
                existingPlant.Price = plant.Price;
                existingPlant.Quantity = plant.Quantity;
                existingPlant.CategoryId = plant.CategoryId;
                existingPlant.ImageUrl = plant.ImageUrl;

                _context.SaveChanges();

                TempData["Success"] =
                    $"{plant.PlantName} has been updated successfully!";

                return RedirectToAction("ManagePlants");
            }

            ViewBag.Categories = _context.Categories.ToList();

            return View(plant);
        }

        // MANAGE ORDERS
        // MANAGE ORDERS
        [HttpGet]
        public IActionResult Orders(
            string? searchString,
            DateTime? fromDate,
            DateTime? toDate,
            string? sortOrder)
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Account");

            var orders = _context.Orders.AsQueryable();

            // Filter by starting date
            if (fromDate.HasValue)
            {
                orders = orders.Where(
                    o => o.OrderDate >= fromDate.Value);

                ViewBag.FromDate = fromDate.Value.ToString("yyyy-MM-dd");
            }

            // Filter by ending date
            if (toDate.HasValue)
            {
                DateTime endDate = toDate.Value.Date.AddDays(1);

                orders = orders.Where(o => o.OrderDate < endDate);

                ViewBag.ToDate = toDate.Value.ToString("yyyy-MM-dd");
            }

            // Sorting
            orders = sortOrder switch
            {
                "date_asc" => orders.OrderBy(o => o.OrderDate),
                "total_asc" => orders.OrderBy(o => o.TotalAmount),
                "total_desc" => orders.OrderByDescending(o => o.TotalAmount),
                _ => orders.OrderByDescending(o => o.OrderDate)
            };

            ViewBag.CurrentSort = sortOrder;
            ViewBag.DateSortParam =
                sortOrder == "date_asc" ? "date_desc" : "date_asc";
            ViewBag.TotalSortParam =
                sortOrder == "total_asc" ? "total_desc" : "total_asc";

            ViewBag.SearchString = searchString;

            // Order statistics
            ViewBag.TotalOrders = _context.Orders.Count();

            ViewBag.TodayCount = _context.Orders.Count(
                o => o.OrderDate.Date == DateTime.Today);

            ViewBag.TotalRevenue = _context.Orders
                .Sum(o => (decimal?)o.TotalAmount) ?? 0;

            return View(orders.ToList());
        }
    }
}
