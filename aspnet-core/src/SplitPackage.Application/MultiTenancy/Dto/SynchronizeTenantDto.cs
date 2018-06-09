using System;
using System.Collections.Generic;
using System.Text;

namespace SplitPackage.MultiTenancy.Dto
{
    public class SynchronizeTenantDto
    {
        public string TenantName { get; set; }

        public List<SynchronizeChannelDto> Channels { get; set; }
    }

    public class SynchronizeChannelDto
    {
        public string LogisticCode { get; set; }

        public string LogisticChannelCode { get; set; }

        public double? StartingWeight { get; set; }

        public double? StartingPrice { get; set; }

        public double? StepWeight { get; set; }

        public double? Price { get; set; }
    }
}
