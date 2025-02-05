using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WorkShopManager.Models;

namespace WorkShopManager.Data
{
    public class WorkshopContext : IdentityDbContext<ApplicationUser>
    {
        public WorkshopContext(DbContextOptions<WorkshopContext> options) : base(options) { }

        public DbSet<Appointment> Appointments { get; set; } // Tabela dla wizyt

        public DbSet<Event> Events { get; set; }

        public List<ApplicationUser> GetUsersByRole(string roleId)
        {
            return Users
                .Where(u => UserRoles.Any(ur => ur.UserId == u.Id && ur.RoleId == roleId))
                .ToList();
        }

    }
}
