using System.Diagnostics;
using CineBook.ApplicationDbContext;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CineBook.Controllers
{
    public class HomeController : Controller
    {

        private readonly AppDbContextion _context;

        public HomeController(AppDbContextion context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }


        public async Task<IActionResult> Search(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return Json(new { movies = new object[] { } });
            }

            var movies = await _context.Movies
                .Where(m => m.Title.Contains(searchTerm))
                .Select(m => new { m.Id, m.Title, m.MovieImage }) 
                .ToListAsync();

            return Json(new { movies });
        }

    }
}
