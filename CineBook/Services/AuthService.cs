using CineBook.DTO;
using CineBook.Interface;
using CineBook.Models;
using Microsoft.AspNetCore.Identity;

namespace CineBook.Services
{
    public class AuthService :IAuthService
    {

        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public AuthService(IHttpContextAccessor httpContextAccessor, UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<User?> GetLoggedInUserAsync()
        {
            // Check if the user is authenticated and has a name
            if (_httpContextAccessor.HttpContext.User.Identity?.IsAuthenticated != true)
            {
                // User is not authenticated, return null
                return null;
            }

            // Get the user name
            string? userName = _httpContextAccessor.HttpContext.User.Identity?.Name;

            // Check if the user name is null
            if (userName == null)
            {

                return null;
            }

            // Find the user by username
            var user = await _userManager.FindByNameAsync(userName);

            return user;
        }




        public async Task RegisterUser(RegisterViewModel registerViewModel)
        {
            var user = new User { UserName = registerViewModel.Email, Email = registerViewModel.Email };

            var result = await _userManager.CreateAsync(user, registerViewModel.Password);
            if (result.Succeeded)
            {
                await _signInManager.SignInAsync(user, isPersistent: false);

            }
        }

        public async Task LogInUser(LogInViewModel logInViewModel)
        {
            if (logInViewModel == null)
            {
                throw new ArgumentNullException(nameof(logInViewModel), "LogInViewModel cannot be null");
            }

            // Attempt to sign in the user using their email and password
            var result = await _signInManager.PasswordSignInAsync(logInViewModel.Email, logInViewModel.Password, false, lockoutOnFailure: false);
        }

        public async Task LogOutUser()
        {
        }
    }
}
