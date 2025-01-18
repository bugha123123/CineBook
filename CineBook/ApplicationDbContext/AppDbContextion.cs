using CineBook.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CineBook.ApplicationDbContext
{
    public class AppDbContextion : IdentityDbContext<User>
    {

        public AppDbContextion(DbContextOptions<AppDbContextion> options) : base(options) { }


        public DbSet<Movie> Movies { get; set; }


        public DbSet<Seat> Seats { get; set; }

        public DbSet<Booking> Bookings { get; set; }

        public DbSet<Chat> Chats { get; set; }

        public DbSet<Message> Messages { get; set; }

        public DbSet<SupportTicket> SupportTickets { get; set; }



        public static async Task SeedRolesAsync(IServiceProvider serviceProvider, RoleManager<IdentityRole> roleManager)
        {
            string[] roleNames = { "Admin", "User" };

            foreach (var roleName in roleNames)
            {
                var roleExist = await roleManager.RoleExistsAsync(roleName);
                if (!roleExist)
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }
        }

    }
}
