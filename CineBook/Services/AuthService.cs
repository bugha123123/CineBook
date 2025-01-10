using CineBook.DTO;
using CineBook.Interface;
using CineBook.Models;
using Microsoft.AspNetCore.Identity;

namespace CineBook.Services
{
    public class AuthService : IAuthService
    {

        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IEmailService emailService;

        public AuthService(IHttpContextAccessor httpContextAccessor, UserManager<User> userManager, SignInManager<User> signInManager, IEmailService emailService)
        {
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
            _signInManager = signInManager;
            this.emailService = emailService;
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
                // you can change the role here to add admin User to DB.
                await _userManager.AddToRoleAsync(user, "USER");
                await _signInManager.SignInAsync(user, isPersistent: false);
                await SendWelcomeEmailToUser(registerViewModel.Email);

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
            await _signInManager.SignOutAsync();
        }

        public async Task SendWelcomeEmailToUser(string email)
        {
            var htmlBody = $@"
<html>
<head>
    <style>
        .email-container {{
            font-family: Arial, sans-serif;
            background-color: #f4f4f4;
            color: #333;
            padding: 20px;
            border-radius: 10px;
            text-align: center;
            max-width: 600px;
            margin: 0 auto;
        }}
        .header {{
            background-color: #6b46c1;
            color: #fff;
            padding: 15px;
            border-radius: 10px;
            margin-bottom: 20px;
        }}
        .button {{
            display: inline-block;
            background-color: #6b46c1;
            color: black;
            padding: 10px 20px;
            border-radius: 5px;
            text-decoration: none;
            font-weight: bold;
            margin-top: 20px;
            transition: background-color 0.3s;
        }}
        .button:hover {{
            background-color: #805ad5;
        }}
        .footer {{
            font-size: 0.9em;
            color: #b3b3b3;
            margin-top: 30px;
        }}
    </style>
</head>
<body>
    <div class='email-container'>
        <div class='header'>
            <h2>Welcome to Spooky Bookstore!</h2>
        </div>
        <p>Hi there!</p>
        <p>We're excited to have you as a new member of the CineBook  community! Get ready to dive into the world of movies</p>
        <p>To get started, click the button below and explore our latest arrivals and special offers:</p>
        <a href='https://localhost:7194/Home/Index' class='button'>Start Booking</a>
        <p class='footer'>If you have any questions or need help, feel free to reach out to our support team.</p>
    </div>
</body>
</html>
";

             await emailService.SendEmailAsync(email, "Welcome to CineBook!", htmlBody);
        }

    }
}
