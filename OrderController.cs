
using ASP.NET_PlantsOrder.Data;
using ASP.NET_PlantsOrder.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ASP.NET_PlantsOrder.Controllers
{
    public class OrderController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OrderController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ADD PLANT TO CART
        public IActionResult AddToCart(int plantId, int quantity = 1)
        {
            if (quantity < 1)
                quantity = 1;

            var plant = _context.Plants
                .FirstOrDefault(p => p.PlantId == plantId);

            if (plant == null)
                return NotFound("Plant not found.");

            var cart = GetCart();

            string imageUrl =
                string.Equals(
                    plant.PlantName?.Trim(),
                    "Rose",
                    StringComparison.OrdinalIgnoreCase)
                    ? "/PLANT/rose.jpeg"
                    : plant.ImageUrl ?? "";

            var existingItem = cart.FirstOrDefault(
                c => c.PlantId == plantId);

            if (existingItem != null)
            {
                existingItem.Quantity += quantity;
                existingItem.ImageUrl = imageUrl;
            }
            else
            {
                cart.Add(new CartItem
                {
                    PlantId = plant.PlantId,
                    PlantName = plant.PlantName ?? "",
                    Price = plant.Price,
                    Quantity = quantity,
                    ImageUrl = imageUrl
                });
            }

            SaveCart(cart);

            return RedirectToAction(nameof(Index));
        }

        // VIEW CART
        public IActionResult Index()
        {
            return View(GetCart());
        }

        // UPDATE CART QUANTITY
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateCart(int plantId, int quantity)
        {
            var cart = GetCart();

            var item = cart.FirstOrDefault(
                c => c.PlantId == plantId);

            if (item != null)
            {
                if (quantity <= 0)
                    cart.Remove(item);
                else
                    item.Quantity = quantity;
            }

            SaveCart(cart);

            return RedirectToAction(nameof(Index));
        }

        // REMOVE PLANT FROM CART
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RemoveFromCart(int plantId)
        {
            var cart = GetCart();

            cart.RemoveAll(c => c.PlantId == plantId);

            SaveCart(cart);

            return RedirectToAction(nameof(Index));
        }

        // CHECKOUT PAGE - GET
        [HttpGet]
        public IActionResult Checkout()
        {
            var customerId =
                HttpContext.Session.GetInt32("CustomerId");

            if (customerId == null)
                return RedirectToAction("Login", "Account");

            var cart = GetCart();

            if (!cart.Any())
                return RedirectToAction(nameof(Index));

            ViewBag.Total = cart.Sum(c => c.Price * c.Quantity);

            return View(cart);
        }

        // PLACE ORDER - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Checkout(
            string deliveryAddress,
            string phoneNumber)
        {
            var customerId =
                HttpContext.Session.GetInt32("CustomerId");

            if (customerId == null)
                return RedirectToAction("Login", "Account");

            var cart = GetCart();

            if (!cart.Any())
                return RedirectToAction(nameof(Index));

            if (string.IsNullOrWhiteSpace(deliveryAddress) ||
                string.IsNullOrWhiteSpace(phoneNumber))
            {
                TempData["Error"] =
                    "Please enter your delivery address and phone number.";

                return RedirectToAction(nameof(Checkout));
            }

            phoneNumber = phoneNumber.Trim();

            if (phoneNumber.Length != 10 ||
                !phoneNumber.All(char.IsDigit))
            {
                TempData["Error"] =
                    "Please enter a valid 10-digit phone number.";

                return RedirectToAction(nameof(Checkout));
            }

            // CREATE ORDER WITH STATUS
            var order = new Order
            {
                CustomerId = customerId.Value,
                DeliveryAddress = deliveryAddress.Trim(),
                PhoneNumber = phoneNumber,
                TotalAmount = cart.Sum(c => c.Price * c.Quantity),
                OrderDate = DateTime.Now,
                OrderStatus = "Confirmed",

                OrderItems = cart.Select(c => new OrderItem
                {
                    PlantId = c.PlantId,
                    Quantity = c.Quantity,
                    UnitPrice = c.Price
                }).ToList()
            };

            _context.Orders.Add(order);
            _context.SaveChanges();

            // CLEAR CART AFTER SUCCESSFUL ORDER
            HttpContext.Session.Remove("Cart");

            return RedirectToAction(
                nameof(OrderConfirmation),
                new { orderId = order.OrderId });
        }

        // ORDER CONFIRMATION
        public IActionResult OrderConfirmation(int orderId)
        {
            var customerId =
                HttpContext.Session.GetInt32("CustomerId");

            if (customerId == null)
                return RedirectToAction("Login", "Account");

            var order = _context.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefault(o =>
                    o.OrderId == orderId &&
                    o.CustomerId == customerId.Value);

            if (order == null)
                return NotFound("Order not found.");

            ViewBag.OrderId = order.OrderId;
            ViewBag.TotalAmount = order.TotalAmount;
            ViewBag.OrderStatus = order.OrderStatus;

            return View();
        }

        // GET CART FROM SESSION
        private List<CartItem> GetCart()
        {
            var cartJson =
                HttpContext.Session.GetString("Cart");

            if (string.IsNullOrEmpty(cartJson))
                return new List<CartItem>();

            return JsonConvert.DeserializeObject<List<CartItem>>(cartJson)
                   ?? new List<CartItem>();
        }

        // SAVE CART TO SESSION
        private void SaveCart(List<CartItem> cart)
        {
            HttpContext.Session.SetString(
                "Cart",
                JsonConvert.SerializeObject(cart));
        }

        // MY ORDERS WITH FILTERING AND SORTING
        public IActionResult MyOrders(
            string status,
            string sortOrder,
            DateTime? fromDate,
            DateTime? toDate)
        {
            var customerId =
                HttpContext.Session.GetInt32("CustomerId");

            if (customerId == null)
                return RedirectToAction("Login", "Account");

            var orders = _context.Orders
                .Include(o => o.OrderItems)
                .Where(o => o.CustomerId == customerId.Value);

            // STATUS FILTER
            if (!string.IsNullOrWhiteSpace(status) &&
                status != "All")
            {
                orders = orders.Where(
                    o => o.OrderStatus == status);

                ViewBag.CurrentStatus = status;
            }
            else
            {
                ViewBag.CurrentStatus = "All";
            }

            // FROM DATE
            if (fromDate.HasValue)
            {
                orders = orders.Where(
                    o => o.OrderDate >= fromDate.Value.Date);

                ViewBag.FromDate =
                    fromDate.Value.ToString("yyyy-MM-dd");
            }

            // TO DATE - INCLUDE ENTIRE DAY
            if (toDate.HasValue)
            {
                var endDate = toDate.Value.Date.AddDays(1);

                orders = orders.Where(
                    o => o.OrderDate < endDate);

                ViewBag.ToDate =
                    toDate.Value.ToString("yyyy-MM-dd");
            }

            // SORT ORDERS
            ViewBag.CurrentSort = sortOrder;

            orders = sortOrder switch
            {
                "date_asc" =>
                    orders.OrderBy(o => o.OrderDate),

                "total_desc" =>
                    orders.OrderByDescending(o => o.TotalAmount),

                "total_asc" =>
                    orders.OrderBy(o => o.TotalAmount),

                _ =>
                    orders.OrderByDescending(o => o.OrderDate)
            };

            ViewBag.StatusList = new List<string>
            {
                "All",
                "Pending",
                "Confirmed",
                "Preparing",
                "OutForDelivery",
                "Delivered",
                "Cancelled"
            };

            return View(orders.ToList());
        }

        // TRACK ORDER
        public IActionResult TrackOrder(int id)
        {
            var customerId =
                HttpContext.Session.GetInt32("CustomerId");

            if (customerId == null)
                return RedirectToAction("Login", "Account");

            var order = _context.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefault(o =>
                    o.OrderId == id &&
                    o.CustomerId == customerId.Value);

            if (order == null)
                return NotFound("Order not found.");

            return View(order);
        }

        // CANCEL ORDER
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CancelMyOrder(int orderId)
        {
            var customerId =
                HttpContext.Session.GetInt32("CustomerId");

            if (customerId == null)
                return RedirectToAction("Login", "Account");

            var order = _context.Orders.FirstOrDefault(o =>
                o.OrderId == orderId &&
                o.CustomerId == customerId.Value);

            if (order == null)
                return NotFound("Order not found.");

            if (order.OrderStatus != "Pending" &&
                order.OrderStatus != "Confirmed")
            {
                TempData["Error"] =
                    "Order cannot be cancelled at this stage!";

                return RedirectToAction(
                    nameof(TrackOrder),
                    new { id = orderId });
            }

            order.OrderStatus = "Cancelled";
            _context.SaveChanges();

            TempData["Success"] =
                "Order cancelled successfully!";

            return RedirectToAction(nameof(MyOrders));
        }
    }

    // CART ITEM MODEL
    public class CartItem
    {
        public int PlantId { get; set; }

        public string PlantName { get; set; } = string.Empty;

        public decimal Price { get; set; }

        public int Quantity { get; set; }

        public string ImageUrl { get; set; } = string.Empty;
    }
}
