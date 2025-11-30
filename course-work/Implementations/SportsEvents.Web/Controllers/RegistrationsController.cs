using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SportsEvents.Web.Data;
using SportsEvents.Web.Models;

namespace SportsEvents.Web.Controllers
{
    [Authorize]
    public class RegistrationsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public RegistrationsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Registrations
        [AllowAnonymous]
        public async Task<IActionResult> Index(string sortOrder, string searchEvent, string searchParticipant, int page = 1)
        {
            int pageSize = 5;

            ViewData["CurrentSort"] = sortOrder;
            ViewData["CurrentEventFilter"] = searchEvent;
            ViewData["CurrentParticipantFilter"] = searchParticipant;

            ViewData["EventSortParm"] = string.IsNullOrEmpty(sortOrder) ? "event_desc" : "";
            ViewData["DateSortParm"] = sortOrder == "Date" ? "date_desc" : "Date";

            var query = _context.Registrations
                .Include(r => r.SportEvent)
                .Include(r => r.Participant)
                .AsQueryable();

            // SEARCH
            if (!string.IsNullOrEmpty(searchEvent))
            {
                query = query.Where(r =>
                    r.SportEvent != null &&
                    r.SportEvent.Name.Contains(searchEvent));
            }

            if (!string.IsNullOrEmpty(searchParticipant))
            {
                query = query.Where(r =>
                    r.Participant != null &&
                    (
                        r.Participant.Email.Contains(searchParticipant) ||
                        r.Participant.FirstName.Contains(searchParticipant) ||
                        r.Participant.LastName.Contains(searchParticipant)
                    ));
            }

            // SORTING
            switch (sortOrder)
            {
                case "event_desc":
                    query = query.OrderByDescending(r => r.SportEvent.Name);
                    break;
                case "Date":
                    query = query.OrderBy(r => r.RegisteredAt);
                    break;
                case "date_desc":
                    query = query.OrderByDescending(r => r.RegisteredAt);
                    break;
                default:
                    // default sort by event name ascending
                    query = query.OrderBy(r => r.SportEvent.Name);
                    break;
            }

            // PAGINATION
            int totalItems = await query.CountAsync();
            var registrations = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewData["CurrentPage"] = page;
            ViewData["TotalPages"] = (int)Math.Ceiling((double)totalItems / pageSize);

            return View(registrations);
        }


        // GET: Registrations/Details/5
        [AllowAnonymous]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var registration = await _context.Registrations
                .Include(r => r.SportEvent)
                .Include(r => r.Participant)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (registration == null)
            {
                return NotFound();
            }

            return View(registration);
        }

        // GET: Registrations/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            ViewData["SportEventId"] = new SelectList(_context.SportEvents, "Id", "Name");
            ViewData["ParticipantId"] = new SelectList(_context.Participants, "Id", "Email");

            return View();
        }

        // POST: Registrations/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("Id,SportEventId,ParticipantId,RegisteredAt,Status,IsPaid,PaidAmount,Notes")] Registration registration)
        {
            if (ModelState.IsValid)
            {
                _context.Add(registration);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["SportEventId"] = new SelectList(_context.SportEvents, "Id", "Name", registration.SportEventId);
            ViewData["ParticipantId"] = new SelectList(_context.Participants, "Id", "Email", registration.ParticipantId);

            return View(registration);
        }

        // GET: Registrations/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var registration = await _context.Registrations.FindAsync(id);
            if (registration == null)
            {
                return NotFound();
            }

            ViewData["SportEventId"] = new SelectList(_context.SportEvents, "Id", "Name", registration.SportEventId);
            ViewData["ParticipantId"] = new SelectList(_context.Participants, "Id", "Email", registration.ParticipantId);

            return View(registration);
        }

        // POST: Registrations/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,SportEventId,ParticipantId,RegisteredAt,Status,IsPaid,PaidAmount,Notes")] Registration registration)
        {
            if (id != registration.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(registration);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RegistrationExists(registration.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            ViewData["SportEventId"] = new SelectList(_context.SportEvents, "Id", "Name", registration.SportEventId);
            ViewData["ParticipantId"] = new SelectList(_context.Participants, "Id", "Email", registration.ParticipantId);

            return View(registration);
        }

        // GET: Registrations/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var registration = await _context.Registrations
                .Include(r => r.SportEvent)
                .Include(r => r.Participant)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (registration == null)
            {
                return NotFound();
            }

            return View(registration);
        }

        // POST: Registrations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var registration = await _context.Registrations.FindAsync(id);
            if (registration != null)
            {
                _context.Registrations.Remove(registration);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RegistrationExists(int id)
        {
            return _context.Registrations.Any(e => e.Id == id);
        }
    }
}
