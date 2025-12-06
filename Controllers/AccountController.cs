using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MindEase.Data;
using MindEase.Models;
using System.Security.Claims;

namespace MindEase.Controllers
{
    public class AccountController : Controller
    {
        private readonly MindEaseContext _context;

        public AccountController(MindEaseContext context)
        {
            _context = context;
        }

        // GET: /Account/Login
        [HttpGet]
        public IActionResult Login()
        {
            // If already logged in, redirect to home
            if (HttpContext.Session.GetString("Username") != null)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == model.Username && u.Password == model.Password);

            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid username or password.");
                return View(model);
            }

            // Store user info in session
            HttpContext.Session.SetString("Username", user.Username);
            HttpContext.Session.SetString("UserType", user.UserType);
            HttpContext.Session.SetInt32("UserId", user.Id);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim("UserType", (user.UserType ?? "").ToLowerInvariant())
            };

            // Redirect based on user type
            return user.UserType switch
            {
                "admin" => RedirectToAction("AdminDashboard"),
                "therapist" => RedirectToAction("TherapistDashboard"),
                _ => RedirectToAction("UserDashboard")
            };
        }

        // GET: /Account/Register
        [HttpGet]
        public IActionResult Register()
        {
            // If already logged in, redirect to home
            if (HttpContext.Session.GetString("Username") != null)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        // POST: /Account/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Check if username already exists
            var existingUser = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == model.Username);

            if (existingUser != null)
            {
                ModelState.AddModelError("Username", "This username is already taken.");
                return View(model);
            }

            // Create new user
            var user = new User
            {
                Username = model.Username,
                Password = model.Password,
                UserType = model.UserType
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Auto-login after registration
            HttpContext.Session.SetString("Username", user.Username);
            HttpContext.Session.SetString("UserType", user.UserType);
            HttpContext.Session.SetInt32("UserId", user.Id);

            // Redirect to appropriate dashboard
            return user.UserType switch
            {
                "therapist" => RedirectToAction("TherapistDashboard"),
                _ => RedirectToAction("UserDashboard")
            };
        }

        // GET: /Account/Logout
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }

        // Dashboard Actions
        public IActionResult AdminDashboard()
        {
            // Check if user is logged in and is admin
            if (HttpContext.Session.GetString("UserType") != "admin")
            {
                return RedirectToAction("Login");
            }

            ViewBag.Username = HttpContext.Session.GetString("Username");
            return View();
        }

        public IActionResult TherapistDashboard()
        {
            // Check if user is logged in and is therapist
            var userType = HttpContext.Session.GetString("UserType");
            if (userType != "therapist" && userType != "admin")
            {
                return RedirectToAction("Login");
            }

            ViewBag.Username = HttpContext.Session.GetString("Username");
            return View();
        }

        public IActionResult UserDashboard()
        {
            // Check if user is logged in
            if (HttpContext.Session.GetString("Username") == null)
            {
                return RedirectToAction("Login");
            }

            ViewBag.Username = HttpContext.Session.GetString("Username");
            return View();
        }
    }
}
