using System.ComponentModel.DataAnnotations;
using Abp.Authorization.Users;
using Abp.AutoMapper;
using Abp.MultiTenancy;

namespace SplitPackage.MultiTenancy.Dto
{
    [AutoMapTo(typeof(Tenant))]
    public class CreateTenantDto
    {
        [Required]
        [StringLength(AbpTenantBase.MaxTenancyNameLength)]
        public string TenancyName { get; set; }

        [Required]
        [StringLength(AbpTenantBase.MaxNameLength)]
        public string Name { get; set; }

        [Required]
        [StringLength(AbpUserBase.MaxEmailAddressLength)]
        public string AdminEmailAddress { get; set; }

        [StringLength(AbpTenantBase.MaxConnectionStringLength)]
        public string ConnectionString { get; set; }

        [StringLength(Tenant.MaxApiKeyLength)]
        public string ApiKey { get; set; }

        [StringLength(Tenant.MaxApiSecretLength)]
        public string ApiSecret { get; set; }

        public bool IsActive {get; set;}
    }
}
