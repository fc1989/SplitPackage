using Abp.Authorization;
using SplitPackage.Authorization.Roles;
using SplitPackage.Authorization.Users;

namespace SplitPackage.Authorization
{
    public class PermissionChecker : PermissionChecker<Role, User>
    {
        public PermissionChecker(UserManager userManager)
            : base(userManager)
        {
        }
    }
}
