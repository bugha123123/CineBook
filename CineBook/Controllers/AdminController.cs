using CineBook.Interface;
using Microsoft.AspNetCore.Mvc;

namespace CineBook.Controllers
{
    public class AdminController : Controller
    {
        private readonly IAdminService _adminService;

        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        public IActionResult MainAdminPage()
        {
            return View();
        }

        public async Task<IActionResult> AddNewAdmin(string UserName, string Gmail, string Role)
        {
            await _adminService.AddNewAdmin(UserName, Gmail, Role);
            return RedirectToAction("MainAdminPage", "Admin");
        }
    }
}
