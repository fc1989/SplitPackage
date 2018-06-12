using System;
using System.ComponentModel.DataAnnotations;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using Abp.MultiTenancy;

namespace SplitPackage.MultiTenancy.Dto
{
    [AutoMapFrom(typeof(Tenant))]
    public class TenantDto : EntityDto
    {
        [Required]
        [StringLength(AbpTenantBase.MaxTenancyNameLength)]
        public string TenancyName { get; set; }

        [Required]
        [StringLength(AbpTenantBase.MaxNameLength)]
        public string Name { get; set; }

        [StringLength(Tenant.MaxApiKeyLength)]
        public string ApiKey { get; set; }

        [StringLength(Tenant.MaxApiSecretLength)]
        public string ApiSecret { get; set; }

        public DateTime CreationTime { get; set; }

        public bool IsActive {get; set;}
    }
}
