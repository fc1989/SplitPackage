using SplitPackage.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace SplitPackage.Business.PublicInformations.Dto
{
    public class LogisticChannelOptionDto
    {
        public IEnumerable<OptionDto<int>> ChannelType { get; set; }

        public IEnumerable<OptionDto<int>> ChargeWay { get; set; }
    }
}
