using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using Abp.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace SplitPackage.Business.NumFreights.Dto
{
    [AutoMap(typeof(NumFreight))]
    public class CreateNumFreightDto : EntityDto<long>
    {
        [Required]
        public long LogisticLineId { get; set; }

        public int ProductNum { get; set; }

        public double PackagePrice { get; set; }
    }
}