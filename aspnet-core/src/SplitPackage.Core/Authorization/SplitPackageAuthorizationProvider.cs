using Abp.Authorization;
using Abp.Localization;
using Abp.MultiTenancy;

namespace SplitPackage.Authorization
{
    public class SplitPackageAuthorizationProvider : AuthorizationProvider
    {
        public override void SetPermissions(IPermissionDefinitionContext context)
        {
            context.CreatePermission(PermissionNames.Pages_Users, L("Users"));
            context.CreatePermission(PermissionNames.Pages_Roles, L("Roles"));
            context.CreatePermission(PermissionNames.Pages_Tenants, L("Tenants"), multiTenancySides: MultiTenancySides.Host);
            context.CreatePermission(PermissionNames.Pages_Products, L("Products"));
            context.CreatePermission(PermissionNames.Pages_ProductClasses, L("ProductClasses"));
            context.CreatePermission(PermissionNames.Pages_Logistics, L("Logistics"));
            context.CreatePermission(PermissionNames.Pages_LogisticLines, L("LogisticLines"));
            context.CreatePermission(PermissionNames.Pages_SplitRules, L("SplitRules"));
            context.CreatePermission(PermissionNames.Pages_WeightFreights, L("WeightFreights"));
            context.CreatePermission(PermissionNames.Pages_NumFreights, L("NumFreights"));
        }

        private static ILocalizableString L(string name)
        {
            return new LocalizableString(name, SplitPackageConsts.LocalizationSourceName);
        }
    }
}
