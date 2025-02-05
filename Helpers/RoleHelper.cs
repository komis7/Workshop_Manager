using WorkShopManager.Data;

namespace WorkShopManager.Helpers
{
    public class RoleHelper
    {
        public static bool UserHasRole(WorkshopContext context, string userId, string roleName)
        {
            var roleId = context.Roles.FirstOrDefault(r => r.Name == roleName)?.Id;
            if (roleId == null) return false;

            return context.UserRoles.Any(ur => ur.UserId == userId && ur.RoleId == roleId);
        }
    }
}
