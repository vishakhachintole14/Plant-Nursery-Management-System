
using ASP.NET_PlantsOrder.Data;
using ASP.NET_PlantsOrder.Models;
using ASP.NET_PlantsOrder.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ASP.NET_PlantsOrder.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly PasswordHasher<Customer> _passwordHasher = new();

        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }

        // REGISTER PAGE
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // REGISTER CUSTOMER
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            bool customerExists = _context.Customers
                .Any(c => c.Email == model.Email);

            if (customerExists)
            {
                ModelState.AddModelError(
                    "Email", "This email is already registered.");
                return View(model);
            }

            var customer = new Customer
            {
                CustomerName = !string.IsNullOrWhiteSpace(model.FullName)
                    ? model.FullName!
                    : model.Username,
                Email = model.Email,
                Phone = model.Phone,
                Address = model.Address,
                IsAdmin = false
            };

            customer.PasswordHash = _passwordHasher.HashPassword(
                customer, model.Password);

            _context.Customers.Add(customer);
            _context.SaveChanges();

            TempData["Success"] =
                "Registration successful. Please log in.";

            return RedirectToAction(nameof(Login));
        }

        // LOGIN PAGE
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // LOGIN CUSTOMER
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var customer = _context.Customers
                .FirstOrDefault(c => c.Email == model.Email);

            if (customer == null ||
                string.IsNullOrEmpty(customer.PasswordHash))
            {
                ModelState.AddModelError(
                    string.Empty, "Invalid email or password.");
                return View(model);
            }

            var result = _passwordHasher.VerifyHashedPassword(
                customer, customer.PasswordHash, model.Password);

            if (result == PasswordVerificationResult.Failed)
            {
                ModelState.AddModelError(
                    string.Empty, "Invalid email or password.");
                return View(model);
            }

            HttpContext.Session.SetInt32(
                "CustomerId", customer.CustomerId);

            HttpContext.Session.SetString(
                "CustomerName", customer.CustomerName);

            HttpContext.Session.SetString(
                "IsAdmin", customer.IsAdmin.ToString());

            return RedirectToAction("Index", "Homee");
        }

        // VIEW PROFILE
        [HttpGet]
        public IActionResult Profile()
        {
            var customerId = HttpContext.Session.GetInt32("CustomerId");

            if (customerId == null)
                return RedirectToAction(nameof(Login));

            var customer = _context.Customers.Find(customerId.Value);

            if (customer == null)
                return NotFound();

            var model = new ProfileViewModel
            {
                CustomerId = customer.CustomerId,
                CustomerName = customer.CustomerName,
                Email = customer.Email,
                Phone = customer.Phone,
                Address = customer.Address
            };

            return View(model);
        }

        // UPDATE PROFILE
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Profile(ProfileViewModel model)
        {
            var customerId = HttpContext.Session.GetInt32("CustomerId");

            if (customerId == null)
                return RedirectToAction(nameof(Login));

            if (!ModelState.IsValid)
                return View(model);

            var customer = _context.Customers.Find(customerId.Value);

            if (customer == null)
                return NotFound();

            bool emailExists = _context.Customers.Any(c =>
                c.Email == model.Email &&
                c.CustomerId != customerId.Value);

            if (emailExists)
            {
                ModelState.AddModelError(
                    "Email",
                    "This email is already registered to another account.");
                return View(model);
            }

            customer.CustomerName = model.CustomerName;
            customer.Email = model.Email;
            customer.Phone = model.Phone;
            customer.Address = model.Address;

            _context.SaveChanges();

            HttpContext.Session.SetString(
                "CustomerName", customer.CustomerName);

            TempData["Success"] = "Profile updated successfully!";

            return RedirectToAction(nameof(Profile));
        }

        // DELETE ACCOUNT
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteAccount()
        {
            var customerId = HttpContext.Session.GetInt32("CustomerId");

            if (customerId == null)
                return RedirectToAction(nameof(Login));

            var customer = _context.Customers.Find(customerId.Value);

            if (customer == null)
            {
                HttpContext.Session.Clear();
                return RedirectToAction("Index", "Homee");
            }

            try
            {
                _context.Customers.Remove(customer);
                _context.SaveChanges();

                HttpContext.Session.Clear();

                TempData["Success"] =
                    "Your account has been deleted successfully!";

                return RedirectToAction("Index", "Homee");
            }
            catch (DbUpdateException)
            {
                TempData["Error"] =
                    "Your account cannot be deleted because related records still exist.";

                return RedirectToAction(nameof(Profile));
            }
        }

        // LOGOUT
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Homee");
        }

        // CHANGE PASSWORD PAGE
        [HttpGet]
        public IActionResult ChangePassword()
        {
            if (HttpContext.Session.GetInt32("CustomerId") == null)
                return RedirectToAction(nameof(Login));

            return View(new ChangePasswordViewModel());
        }

        // CHANGE PASSWORD
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ChangePassword(ChangePasswordViewModel model)
        {
            var customerId = HttpContext.Session.GetInt32("CustomerId");

            if (customerId == null)
                return RedirectToAction(nameof(Login));

            if (!ModelState.IsValid)
                return View(model);

            var customer = _context.Customers
                .FirstOrDefault(c => c.CustomerId == customerId.Value);

            if (customer == null)
            {
                HttpContext.Session.Clear();
                return RedirectToAction(nameof(Login));
            }

            if (string.IsNullOrEmpty(customer.PasswordHash))
            {
                ModelState.AddModelError(
                    string.Empty, "The stored password hash is invalid.");
                return View(model);
            }

            var result = _passwordHasher.VerifyHashedPassword(
                customer, customer.PasswordHash, model.CurrentPassword);

            if (result == PasswordVerificationResult.Failed)
            {
                ModelState.AddModelError(
                    "CurrentPassword", "Current password is incorrect.");
                return View(model);
            }

            if (model.CurrentPassword == model.NewPassword)
            {
                ModelState.AddModelError(
                    "NewPassword",
                    "Choose a new password different from your current password.");
                return View(model);
            }

            // Generate the new hash
            string newHash = _passwordHasher.HashPassword(
                customer, model.NewPassword);

            // Save the new hash
            customer.PasswordHash = newHash;

            try
            {
                _context.Entry(customer)
                    .Property(c => c.PasswordHash)
                    .IsModified = true;

                _context.SaveChanges();

                // Read the saved value from the database
                string? savedHash = _context.Customers
                    .AsNoTracking()
                    .Where(c => c.CustomerId == customerId.Value)
                    .Select(c => c.PasswordHash)
                    .FirstOrDefault();

                if (savedHash != newHash)
                {
                    TempData["Error"] =
                        "The saved password hash could not be verified. Check the database connection.";
                    return RedirectToAction(nameof(Profile));
                }

                TempData["Success"] =
                    "Password changed and saved to the database successfully!";

                return RedirectToAction(nameof(Profile));
            }
            catch (DbUpdateException)
            {
                TempData["Error"] =
                    "The password could not be saved. Check the database and application logs.";

                return RedirectToAction(nameof(Profile));
            }
        }
    }
}
