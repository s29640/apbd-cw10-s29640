using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using UserPanelMvcAuth.Data;
using UserPanelMvcAuth.Models;
using UserPanelMvcAuth.ViewModels;

namespace UserPanelMvcAuth.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly AppDbContext _context;

        public DashboardController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var userId = GetCurrentUserId();

            var notes = await _context.UserNotes
                .Where(n => n.AppUserId == userId)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();

            var model = new DashboardViewModel
            {
                Notes = notes
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddNote(DashboardViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Notes = await GetCurrentUserNotesAsync();
                return View("Index", model);
            }

            var userId = GetCurrentUserId();

            var note = new UserNote
            {
                AppUserId = userId,
                Title = model.Title,
                Content = model.Content,
                CreatedAt = DateTime.UtcNow
            };

            _context.UserNotes.Add(note);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private int GetCurrentUserId()
        {
            var userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userIdValue is null)
                throw new InvalidOperationException("Brak identyfikatora użytkownika.");

            return int.Parse(userIdValue);
        }

        private async Task<List<UserNote>> GetCurrentUserNotesAsync()
        {
            var userId = GetCurrentUserId();

            return await _context.UserNotes
                .Where(n => n.AppUserId == userId)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();
        }
    }
}
