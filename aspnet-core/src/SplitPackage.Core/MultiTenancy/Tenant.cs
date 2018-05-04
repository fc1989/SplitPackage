using Abp.MultiTenancy;
using SplitPackage.Authorization.Users;

namespace SplitPackage.MultiTenancy
{
    public class Tenant : AbpTenant<User>
    {
        public const int MaxApiKeyLength = 50;

        public const int MaxApiSecretLength = 100;

        public string ApiKey { get; set; }

        public string ApiSecret { get; set; }

        public Tenant()
        {            
        }

        public Tenant(string tenancyName, string name)
            : base(tenancyName, name)
        {
        }
    }
}
