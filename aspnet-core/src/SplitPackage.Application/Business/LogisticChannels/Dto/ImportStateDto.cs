using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;

namespace SplitPackage.Business.LogisticChannels.Dto
{
    public class ImportStateDto
    {
        public IEnumerable<LogisticChannelDto> SystemLogisticChannel { get; set; }

        public IEnumerable<long> ImportLogisticChannel { get; set; }
    }
}
