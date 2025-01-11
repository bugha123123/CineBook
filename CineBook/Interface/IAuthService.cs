using CineBook.DTO;
using CineBook.Models;

namespace CineBook.Interface
{
    public interface IAuthService
    {
        Task<User?> GetLoggedInUserAsync();
        Task RegisterUser(RegisterViewModel registerViewModel);
        Task LogInUser(LogInViewModel logInViewModel);
        Task LogOutUser();

        Task SendWelcomeEmailToUser(string email);

        Task ResetPassword(string Gmail, string token, string newPassword);
    }
}
