using Abp.Authorization;
using Abp.Localization;
using Abp.MultiTenancy;

namespace SplitPackage.Authorization
{
    public class SplitPackageAuthorizationProvider : AuthorizationProvider
    {
        public override void SetPermissions(IPermissionDefinitionContext context)
        {
            context.CreatePermission(PermissionNames.Pages_Admin_Tenants, L("Tenants"), multiTenancySides: MultiTenancySides.Host);
            context.CreatePermission(PermissionNames.Pages_Admin_ProductClasses, L("ProductClasses"), multiTenancySides: MultiTenancySides.Host);

            context.CreatePermission(PermissionNames.Pages_Tenant_Users, L("Users"));
            context.CreatePermission(PermissionNames.Pages_Tenant_Roles, L("Roles"));
            context.CreatePermission(PermissionNames.Pages_Tenant_Products, L("Products"));
            context.CreatePermission(PermissionNames.Pages_Tenant_Logistics, L("Logistics"));
            context.CreatePermission(PermissionNames.Pages_Tenant_LogisticChannels, L("LogisticChannels"));
            context.CreatePermission(PermissionNames.Pages_Tenant_SplitRules, L("SplitRules"));
            context.CreatePermission(PermissionNames.Pages_Tenant_WeightFreights, L("WeightFreights"));
            context.CreatePermission(PermissionNames.Pages_Tenant_NumFreights, L("NumFreights"));
        }

        private static ILocalizableString L(string name)
        {
            return new LocalizableString(name, SplitPackageConsts.LocalizationSourceName);
        }
    }
}
