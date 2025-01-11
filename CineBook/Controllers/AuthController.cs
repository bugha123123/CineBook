using CineBook.DTO;
using CineBook.Interface;
using CineBook.Services;
using Microsoft.AspNetCore.Mvc;

namespace CineBook.Controllers
{
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;
        private readonly IAdminService _adminService;
        public AuthController(IAuthService authService, IAdminService adminService)
        {
            _authService = authService;
            _adminService = adminService;
        }

        public IActionResult signup()
        {
            return View();
        }
        public IActionResult signin()
        {
            return View();
        }

        public IActionResult resetpassword()
        {
            return View();
        }
        public IActionResult forgotpassword()
        {
            return View();
        }
        public async Task<IActionResult> SigninUser(LogInViewModel logInViewModel)
        {
            if (ModelState.IsValid)
            {
                await _authService.LogInUser(logInViewModel);
                return RedirectToAction("Index", "Home");
            }
            return View("signin", logInViewModel);
        }

        public async Task<IActionResult> SignupUser(RegisterViewModel RegisterViewModel)
        {
            if (ModelState.IsValid)
            {
                await _authService.RegisterUser(RegisterViewModel);
                return RedirectToAction("Index", "Home");
            }
            return View("signup", RegisterViewModel);
        }

        public async Task<IActionResult> SignoutUser()
        {

            await _authService.LogOutUser();
            return RedirectToAction("signin", "Auth");


        }

        public async Task<IActionResult> ResetPasswordAction(string Gmail, string token, string newPassword)
        {

            await _authService.ResetPassword(Gmail,token,newPassword);
            return RedirectToAction("signin", "Auth");


        }


        public async Task<IActionResult> User_SendResetPasswordGmail(string Gmail)
        {
            if (ModelState.IsValid)
            {
                await _adminService.SendResetPasswordEmail(Gmail);
                return RedirectToAction("signin", "Auth");
            }
            return View();
        }
    }
}
