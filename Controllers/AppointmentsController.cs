using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MindEase.Data;
using MindEase.Models;

namespace MindEase.Controllers
{
    public class AppointmentsController : Controller
    {
        private readonly MindEaseContext _context;

        public AppointmentsController(MindEaseContext context)
        {
            _context = context;
        }

        // GET: /Appointments
        public async Task<IActionResult> Index(string? searchString)
        {
            // Kullanıcı giriş yapmış mı kontrol et
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var appointments = _context.Appointments
                .Include(a => a.User)
                .Where(a => a.UserId == userId);

            // Arama özelliği
            if (!string.IsNullOrEmpty(searchString))
            {
                appointments = appointments.Where(a => 
                    a.Title.Contains(searchString) || 
                    a.Notes!.Contains(searchString));
            }

            ViewBag.SearchString = searchString;
            return View(await appointments.OrderByDescending(a => a.AppointmentDate).ToListAsync());
        }

        // GET: /Appointments/Create
        public IActionResult Create()
        {
            // Kullanıcı giriş yapmış mı kontrol et
            if (HttpContext.Session.GetInt32("UserId") == null)
            {
                return RedirectToAction("Login", "Account");
            }
            return View();
        }

        // POST: /Appointments/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Appointment appointment)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // UserId'yi session'dan al
            appointment.UserId = userId.Value;

            // User navigation property'yi temizle (validation için)
            ModelState.Remove("User");

            if (ModelState.IsValid)
            {
                _context.Appointments.Add(appointment);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Appointment created successfully!";
                return RedirectToAction(nameof(Index));
            }
            return View(appointment);
        }

        // GET: /Appointments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (id == null)
            {
                return NotFound();
            }

            var appointment = await _context.Appointments
                .FirstOrDefaultAsync(a => a.Id == id && a.UserId == userId);

            if (appointment == null)
            {
                return NotFound();
            }

            return View(appointment);
        }

        // POST: /Appointments/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Appointment appointment)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (id != appointment.Id)
            {
                return NotFound();
            }

            // UserId'yi session'dan al
            appointment.UserId = userId.Value;
            ModelState.Remove("User");

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(appointment);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Appointment updated successfully!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AppointmentExists(appointment.Id))
                    {
                        return NotFound();
                    }
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(appointment);
        }

        // GET: /Appointments/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (id == null)
            {
                return NotFound();
            }

            var appointment = await _context.Appointments
                .Include(a => a.User)
                .FirstOrDefaultAsync(a => a.Id == id && a.UserId == userId);

            if (appointment == null)
            {
                return NotFound();
            }

            return View(appointment);
        }

        // POST: /Appointments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var appointment = await _context.Appointments
                .FirstOrDefaultAsync(a => a.Id == id && a.UserId == userId);

            if (appointment != null)
            {
                _context.Appointments.Remove(appointment);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Appointment deleted successfully!";
            }

            return RedirectToAction(nameof(Index));
        }

        private bool AppointmentExists(int id)
        {
            return _context.Appointments.Any(e => e.Id == id);
        }
    }
}

