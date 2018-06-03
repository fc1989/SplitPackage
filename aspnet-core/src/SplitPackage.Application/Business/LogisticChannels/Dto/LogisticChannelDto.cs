using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using Abp.Domain.Entities;
using SplitPackage.Business.NumFreights.Dto;
using SplitPackage.Business.WeightFreights.Dto;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SplitPackage.Business.LogisticChannels.Dto
{
    [AutoMapFrom(typeof(LogisticChannel))]
    public class LogisticChannelDto : EntityDto<long>, IPassivable
    {
        [Required]
        [StringLength(LogisticChannel.MaxChannelNameLength)]
        public string ChannelName { get; set; }

        [Required]
        public long LogisticId { get; set; }

        public string LogisticName { get; set; }

        public string AliasName { get; set; }

        public ChannelType Type { get; set; }

        public ChargeWay Way { get; set; }

        public bool IsActive { get; set; }

        public int? TenantId { get; set; }

        public IEnumerable<NumFreightDto> NumFreights { get;set;}

        public IEnumerable<WeightFreightDto> WeightFreights { get; set; }
    }
}
