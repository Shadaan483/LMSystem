using LMSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LMSystem.Controllers
{
    // Public landing page - stays visible even though every other controller
    // now requires a logged-in user (see the global AuthorizeFilter in Program.cs).
    [AllowAnonymous]
    public class HomeController : Controller
    {
        private readonly LibraryContext _context;

        public HomeController(LibraryContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // Show the most recently added real books instead of the old
            // hardcoded placeholder titles - this also fixes the bug where
            // "View Details" pointed at IDs that didn't match the cards shown.
            var featuredBooks = await _context.Books13
                .AsNoTracking()
                .OrderByDescending(b => b.BookId)
                .Take(3)
                .ToListAsync();

            return View(featuredBooks);
        }
    }
}
