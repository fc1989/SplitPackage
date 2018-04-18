using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using Abp.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace SplitPackage.Business.LogisticLines.Dto
{
    [AutoMap(typeof(LogisticLine))]
    public class CreateLogisticLineDto : EntityDto<long>
    {
        [Required]
        [StringLength(LogisticLine.MaxLineNameLength)]
        public string LineName { get; set; }

        [Required]
        [StringLength(LogisticLine.MaxLineCodeLength)]
        public string LineCode { get; set; }

        [Required]
        public long LogisticId { get; set; }
    }
}