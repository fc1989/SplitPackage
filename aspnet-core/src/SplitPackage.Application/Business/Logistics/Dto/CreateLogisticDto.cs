using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using Abp.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace SplitPackage.Business.Logistics.Dto
{
    [AutoMap(typeof(Logistic))]
    public class CreateLogisticDto : EntityDto<long>
    {
        [Required]
        [StringLength(Logistic.MaxCorporationNameLength)]
        public string CorporationName { get; set; }

        [StringLength(Logistic.MaxCorporationUrlLength)]
        public string CorporationUrl { get; set; }

        [Required]
        [StringLength(Logistic.MaxLogisticCodeLength)]
        [RegularExpression("^[A-Za-z0-9]+$")]
        public string LogisticCode { get; set; }
    }
}