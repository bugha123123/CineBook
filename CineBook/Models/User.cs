using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CineBook.Models
{
    public class User : IdentityUser
    {
        public List<Booking> booking { get; set; }

        
    }
}
