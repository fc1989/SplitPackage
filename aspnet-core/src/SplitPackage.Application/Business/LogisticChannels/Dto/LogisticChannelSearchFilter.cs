using Abp.Application.Services.Dto;
using SplitPackage.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace SplitPackage.Business.LogisticChannels.Dto
{
    public class LogisticChannelSearchFilter : PageSearchFilter<LogisticChannel>
    {
        public long LogisticId { get; set; }
    }
}
