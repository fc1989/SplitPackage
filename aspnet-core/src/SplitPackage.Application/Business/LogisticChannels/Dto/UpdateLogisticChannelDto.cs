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
    [AutoMap(typeof(LogisticChannel))]
    public class UpdateLogisticChannelDto : EntityDto<long>, IPassivable
    {
        [Required]
        [StringLength(LogisticChannel.MaxChannelNameLength)]
        public string ChannelName { get; set; }

        [StringLength(LogisticChannel.MaxAliasNameLength)]
        public string AliasName { get; set; }

        [Required]
        public ChannelType Type { get; set; }

        [Required]
        public ChargeWay Way { get; set; }

        public bool IsActive { get; set; }

        public IEnumerable<NumFreightDto> NumFreights { get; set; }

        public IEnumerable<WeightFreightDto> WeightFreights { get; set; }
    }
}
