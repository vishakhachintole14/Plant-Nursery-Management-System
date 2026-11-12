using ASP.NET_PlantsOrder.Data;
using Microsoft.AspNetCore.Mvc;

namespace ASP.NET_PlantsOrder.Controllers
{
    public class HomeeController : Controller
    {
        private readonly ApplicationDbContext context;

        public HomeeController(ApplicationDbContext context)
        {
            this.context = context;
        }

        public IActionResult Index()
        {
            var plantitems = context.Plants.ToList();
            return View(plantitems);
        }
    }
}