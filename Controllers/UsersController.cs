using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MindEase.Data;
using MindEase.Models;

namespace MindEase.Controllers
{
    public class UsersController : Controller
    {
        private readonly MindEaseContext _context;

        public UsersController(MindEaseContext context)
        {
            _context = context;
        }

        // GET: /Users
        public async Task<IActionResult> Index()
        {
            var users = await _context.Users.ToListAsync();
            return View(users);
        }
    }
}
