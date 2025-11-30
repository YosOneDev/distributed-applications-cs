using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SportsEvents.Web.Data;
using SportsEvents.Web.Models;

namespace SportsEvents.Web.Controllers
{
    [Authorize]
    public class EventImagesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public EventImagesController(
            ApplicationDbContext context,
            UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // ADMIN: list of pending images
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Pending()
        {
            var images = await _context.EventImages
                .Where(e => !e.IsApproved)
                .Include(e => e.SportEvent)
                .ToListAsync();


            var userIds = images.Select(i => i.UploaderUserId).Distinct().ToList();
            var users = await _userManager.Users
                .Where(u => userIds.Contains(u.Id))
                .ToListAsync();

            var emails = users.ToDictionary(u => u.Id, u => u.Email);

            ViewBag.UploaderEmails = emails;


            return View(images);
        }


        // GET: add image URL for event
        [HttpGet]
        public async Task<IActionResult> Create(int eventId)
        {
            var ev = await _context.SportEvents.FindAsync(eventId);
            if (ev == null) return NotFound();

            ViewData["EventId"] = eventId;
            ViewData["EventName"] = ev.Name;

            return View();
        }

        // POST: add image URL
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int eventId, string imageUrl)
        {
            var ev = await _context.SportEvents.FindAsync(eventId);
            if (ev == null) return NotFound();

            if (string.IsNullOrWhiteSpace(imageUrl))
            {
                ModelState.AddModelError(string.Empty, "Please enter an image URL.");
            }

            if (!ModelState.IsValid)
            {
                ViewData["EventId"] = eventId;
                ViewData["EventName"] = ev.Name;
                return View();
            }

            var user = await _userManager.GetUserAsync(User);

            var img = new EventImage
            {
                SportEventId = eventId,
                ImageUrl = imageUrl,
                UploaderUserId = user.Id,
                IsApproved = false,
                UploadedAt = DateTime.Now
            };

            _context.EventImages.Add(img);
            await _context.SaveChangesAsync();

            return RedirectToAction("Details", "SportEvents", new { id = eventId });
        }

        // POST: approve image (Admin)
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Approve(int id)
        {
            var img = await _context.EventImages.FindAsync(id);
            if (img == null) return NotFound();

            img.IsApproved = true;
            await _context.SaveChangesAsync();

            return RedirectToAction("Pending");
        }

        // POST: delete image (Admin or owner)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var img = await _context.EventImages.FindAsync(id);
            if (img == null) return NotFound();

            var user = await _userManager.GetUserAsync(User);
            var isAdmin = await _userManager.IsInRoleAsync(user, "Admin");

            if (!isAdmin && img.UploaderUserId != user.Id)
            {
                return Forbid();
            }

            _context.EventImages.Remove(img);
            await _context.SaveChangesAsync();

            return RedirectToAction("Details", "SportEvents", new { id = img.SportEventId });
        }
    }
}
