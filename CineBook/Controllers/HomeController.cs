using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CineBook.Controllers
{
    public class HomeController : Controller
    {


 
        public IActionResult Index()
        {
            return View();
        }

    

 
    }
}
