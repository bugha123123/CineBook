using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CineBook.Models
{
    public class User : IdentityUser
    {
        public List<Booking> booking { get; set; }

        public bool IsVerified { get; set; }
        public DateTime? VerifiedAt { get; set; }

        public string? VerificationToken { get; set; } = null;
    }
}
