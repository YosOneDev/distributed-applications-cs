using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SportsEvents.Web.Models;

namespace SportsEvents.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AdminController(UserManager<IdentityUser> userManager,
                               RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // LIST USERS + ROLES
        public IActionResult Index(string searchEmail)
        {
            var usersQuery = _userManager.Users.AsQueryable();

            if (!string.IsNullOrEmpty(searchEmail))
            {
                usersQuery = usersQuery.Where(u => u.Email.Contains(searchEmail));
            }

            var users = usersQuery.ToList();

            var model = users.Select(u => new UserRolesViewModel
            {
                Id = u.Id,
                Email = u.Email,
                Roles = _userManager.GetRolesAsync(u).Result
            }).ToList();

            ViewData["CurrentFilter"] = searchEmail;

            return View(model);
        }

        // MAKE ADMIN
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MakeAdmin(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                if (!await _userManager.IsInRoleAsync(user, "Admin"))
                {
                    await _userManager.AddToRoleAsync(user, "Admin");
                }
            }

            return RedirectToAction(nameof(Index));
        }

        // MAKE USER
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MakeUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                if (!await _userManager.IsInRoleAsync(user, "User"))
                {
                    await _userManager.AddToRoleAsync(user, "User");
                }
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveAdmin(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                var currentUser = await _userManager.GetUserAsync(User);

                if (currentUser != null && currentUser.Id == user.Id)
                {
                    // TempData["Error"] = "You cannot remove your own Admin role.";
                }
                else if (user.Email == "admin@sportsevents.com")
                {
                    // TempData["Error"] = "You cannot remove Admin role from the primary admin.";
                }
                else if (await _userManager.IsInRoleAsync(user, "Admin"))
                {
                    await _userManager.RemoveFromRoleAsync(user, "Admin");
                }
            }

            return RedirectToAction(nameof(Index));
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user != null && await _userManager.IsInRoleAsync(user, "User"))
            {
                await _userManager.RemoveFromRoleAsync(user, "User");
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                var currentUser = await _userManager.GetUserAsync(User);

                if (currentUser != null && currentUser.Id == user.Id)
                {
                    //TempData["Error"] = "You cannot delete your own account.";
                }
                else if (user.Email == "admin@sportsevents.com")
                {
                    // TempData["Error"] = "You cannot delete the primary admin account.";
                }
                else
                {
                    await _userManager.DeleteAsync(user);
                }
            }

            return RedirectToAction(nameof(Index));
        }

    }

}

