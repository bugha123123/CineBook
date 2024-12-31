using CineBook.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CineBook.ApplicationDbContext
{
    public class AppDbContextion : IdentityDbContext<User>
    {

        public AppDbContextion(DbContextOptions<AppDbContextion> options) : base(options) { }

    }
}
