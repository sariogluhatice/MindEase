using Microsoft.AspNetCore.Mvc;
using MindEase.Data;
using MindEase.Models;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace MindEase.Controllers
{
    public class AppointmentsController : Controller
    {
        private readonly MindEaseContext _context;

        public AppointmentsController(MindEaseContext context)
        {
            _context = context;
        }

        // GET: Appointments
        // GET: Appointments
        public IActionResult Index(string searchString)
        {
            var isTherapist = IsCurrentUserTherapist();

            var list = _context.Appointments
                .Where(a => string.IsNullOrWhiteSpace(searchString) || a.Title.Contains(searchString))
                .OrderBy(a => a.AppointmentDate)
                .ToList();

            ViewBag.IsTherapist = isTherapist;
            ViewBag.Now = DateTime.UtcNow;
            ViewBag.SearchString = searchString;

            return View(list);
        }


        // GET: /Appointments/Create
        public IActionResult Create()
        {
            if (HttpContext.Session.GetInt32("UserId") == null)
            {
                return RedirectToAction("Login", "Account");
            }
            return View();
        }

        // POST: /Appointments/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Appointment appointment)
        {
            if (!ModelState.IsValid)
                return View(appointment);

            var isTherapist = IsCurrentUserTherapist();

            // Therapist değilse veya boşsa Pending
            if (!isTherapist || string.IsNullOrWhiteSpace(appointment.Status))
            {
                appointment.Status = "Pending";
            }

            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId != null)
            {
                appointment.UserId = userId.Value;
            }
            ModelState.Remove("User");

            _context.Appointments.Add(appointment);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
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

            appointment.UserId = userId.Value;
            ModelState.Remove("User");

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(appointment);
                    await _context.SaveChangesAsync();
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
            }

            return RedirectToAction(nameof(Index));
        }

        // Therapist'ler Status güncelleyebilir (DB şemasını değiştirmeden)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateStatus(int id, string status)
        {
            if (!IsCurrentUserTherapist())
                return Forbid();

            var appointment = _context.Appointments.FirstOrDefault(a => a.Id == id);
            if (appointment == null)
                return NotFound();

            var allowed = new[] { "Pending", "Confirmed", "Completed", "Cancelled" };
            if (!allowed.Contains(status))
                return BadRequest("Invalid status");

            appointment.Status = status;
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Approve(int id)
        {
            if (!IsCurrentUserTherapist())
                return Forbid();

            var appt = _context.Appointments.FirstOrDefault(a => a.Id == id);
            if (appt == null) return NotFound();

            // Sadece Completed ise approve yok. Cancelled artık approve edilebilir.
            if (EqualsIgnoreCase(appt.Status, "Completed"))
                return RedirectToAction(nameof(Index));

            appt.Status = "Confirmed";
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Cancel(int id)
        {
            if (!IsCurrentUserTherapist())
                return Forbid();

            var appt = _context.Appointments.FirstOrDefault(a => a.Id == id);
            if (appt == null) return NotFound();

            // Completed ise cancel yok
            if (EqualsIgnoreCase(appt.Status, "Completed"))
                return RedirectToAction(nameof(Index));

            appt.Status = "Cancelled";
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        private bool AppointmentExists(int id)
        {
            return _context.Appointments.Any(e => e.Id == id);
        }
        private User GetCurrentUser()
        {
            // Önce Session'dan Username'i al
            var username = HttpContext.Session.GetString("Username");

            // Eğer Session'da yoksa, fallback olarak Identity'den dene
            if (string.IsNullOrEmpty(username))
            {
                username = User?.Identity?.Name;
            }

            if (string.IsNullOrEmpty(username))
                return null;

            return _context.Users.FirstOrDefault(u => u.Username == username);
        }

        private bool IsCurrentUserTherapist()
        {
            var user = GetCurrentUser();
            // DB’de "therapist" küçük olabilir, case-insensitive kontrol yap
            return user != null && string.Equals(user.UserType, "therapist", StringComparison.OrdinalIgnoreCase);
        }

        private static bool EqualsIgnoreCase(string? a, string b)
            => string.Equals(a ?? "", b, StringComparison.OrdinalIgnoreCase);

    }

}

