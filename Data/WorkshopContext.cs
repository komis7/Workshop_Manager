using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WorkShopManager.Models;

namespace WorkShopManager.Data
{
    public class WorkshopContext : IdentityDbContext<ApplicationUser>
    {
        public WorkshopContext(DbContextOptions<WorkshopContext> options) : base(options) { }
        public DbSet<CarMake> CarMakes { get; set; }
        public DbSet<CarModel> CarModels { get; set; }
        public DbSet<Appointment> Appointments { get; set; }

        public DbSet<Event> Events { get; set; }

        public List<ApplicationUser> GetUsersByRole(string roleId)
        {
            return Users
                .Where(u => UserRoles.Any(ur => ur.UserId == u.Id && ur.RoleId == roleId))
                .ToList();
        }

    }
}
