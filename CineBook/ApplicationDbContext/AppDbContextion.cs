using CineBook.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CineBook.ApplicationDbContext
{
    public class AppDbContextion : IdentityDbContext<User>
    {

        public AppDbContextion(DbContextOptions<AppDbContextion> options) : base(options) { }


        public DbSet<Movie> Movies { get; set; }


        public DbSet<Seat> Seats { get; set; }
    }
}
