using System;
using System.Collections.Generic;
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
    public class SportEventsController : Controller

    {
        private readonly ApplicationDbContext _context;

        public SportEventsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: SportEvents
        [AllowAnonymous]
        public async Task<IActionResult> Index(string sortOrder, string searchName, string searchLocation, int page = 1)
        {
            int pageSize = 5;

            ViewData["CurrentSort"] = sortOrder;
            ViewData["CurrentNameFilter"] = searchName;
            ViewData["CurrentLocationFilter"] = searchLocation;

            ViewData["NameSortParm"] = string.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["DateSortParm"] = sortOrder == "Date" ? "date_desc" : "Date";

            var eventsQuery = _context.SportEvents.AsQueryable();

            // ТЪРСЕНЕ
            if (!string.IsNullOrEmpty(searchName))
            {
                eventsQuery = eventsQuery.Where(e => e.Name.Contains(searchName));
            }

            if (!string.IsNullOrEmpty(searchLocation))
            {
                eventsQuery = eventsQuery.Where(e => e.Location.Contains(searchLocation));
            }

            // СОРТИРАНЕ
            switch (sortOrder)
            {
                case "name_desc":
                    eventsQuery = eventsQuery.OrderByDescending(e => e.Name);
                    break;
                case "Date":
                    eventsQuery = eventsQuery.OrderBy(e => e.StartDate);
                    break;
                case "date_desc":
                    eventsQuery = eventsQuery.OrderByDescending(e => e.StartDate);
                    break;
                default:
                    eventsQuery = eventsQuery.OrderBy(e => e.Name);
                    break;
            }

            // СТРАНИЦИРАНЕ
            int totalItems = await eventsQuery.CountAsync();
            var events = await eventsQuery
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewData["CurrentPage"] = page;
            ViewData["TotalPages"] = (int)Math.Ceiling((double)totalItems / pageSize);

            return View(events);
        }

        // GET: SportEvents/Details/5
        [AllowAnonymous]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sportEvent = await _context.SportEvents
                .Include(e => e.Images)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (sportEvent == null)
            {
                return NotFound();
            }

            return View(sportEvent);
        }


        // GET: SportEvents/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: SportEvents/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("Id,Name,SportType,Location,StartDate,EndDate,MaxParticipants,ParticipationFee,Description")] SportEvent sportEvent)
        {
            if (ModelState.IsValid)
            {
                _context.Add(sportEvent);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(sportEvent);
        }

        // GET: SportEvents/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sportEvent = await _context.SportEvents.FindAsync(id);
            if (sportEvent == null)
            {
                return NotFound();
            }
            return View(sportEvent);
        }

        // POST: SportEvents/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,SportType,Location,StartDate,EndDate,MaxParticipants,ParticipationFee,Description")] SportEvent sportEvent)
        {
            if (id != sportEvent.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(sportEvent);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SportEventExists(sportEvent.Id))
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
            return View(sportEvent);
        }

        // GET: SportEvents/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sportEvent = await _context.SportEvents
                .FirstOrDefaultAsync(m => m.Id == id);
            if (sportEvent == null)
            {
                return NotFound();
            }

            return View(sportEvent);
        }

        // POST: SportEvents/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var sportEvent = await _context.SportEvents.FindAsync(id);
            if (sportEvent != null)
            {
                _context.SportEvents.Remove(sportEvent);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SportEventExists(int id)
        {
            return _context.SportEvents.Any(e => e.Id == id);
        }
    }
}
