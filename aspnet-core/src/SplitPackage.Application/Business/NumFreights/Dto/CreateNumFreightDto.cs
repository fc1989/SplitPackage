using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using Abp.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace SplitPackage.Business.NumFreights.Dto
{
    [AutoMap(typeof(NumFreight))]
    public class CreateNumFreightDto
    {
        public string Currency { get; set; }

        public string Unit { get; set; }

        public int SplitNum { get; set; }

        public double FirstPrice { get; set; }

        public double CarryOnPrice { get; set; }
    }
}