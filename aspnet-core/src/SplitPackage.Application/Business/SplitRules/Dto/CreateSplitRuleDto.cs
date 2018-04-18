using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using Abp.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace SplitPackage.Business.SplitRules.Dto
{
    [AutoMap(typeof(SplitRule))]
    public class CreateSplitRuleDto : EntityDto<long>
    {
        [Required]
        public long LogisticLineId { get; set; }

        public int MinPackage { get; set; }

        public int MaxPackage { get; set; }

        public double MaxWeight { get; set; }

        public double MaxTax { get; set; }

        public double MaxPrice { get; set; }
    }
}