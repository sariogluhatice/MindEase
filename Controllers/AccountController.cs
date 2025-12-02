using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MindEase.Data;
using MindEase.Models;

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
            return View();
        }

        // POST: /Account/Login
        [HttpPost]
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
                ModelState.AddModelError(string.Empty, "Kullanıcı adı veya şifre hatalı.");
                return View(model);
            }

            // UserType'a göre yönlendirme
            if (user.UserType == "admin")
            {
                return RedirectToAction("AdminDashboard");
            }
            else if (user.UserType == "therapist")
            {
                return RedirectToAction("TherapistDashboard");
            }
            else // user
            {
                return RedirectToAction("UserDashboard");
            }
        }

        public IActionResult AdminDashboard()
        {
            ViewBag.Message = "Admin paneline hoş geldin 🌟";
            return View();
        }

        public IActionResult TherapistDashboard()
        {
            ViewBag.Message = "Terapist paneline hoş geldin 🧠";
            return View();
        }

        public IActionResult UserDashboard()
        {
            ViewBag.Message = "Kullanıcı paneline hoş geldin 😊";
            return View();
        }
    }
}
