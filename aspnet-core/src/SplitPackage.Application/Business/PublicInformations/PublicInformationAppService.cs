using Abp.Application.Services;
using SplitPackage.Business.PublicInformations.Dto;
using SplitPackage.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace SplitPackage.Business.PublicInformations
{
    public class PublicInformationAppService : ApplicationService
    {
        public PublicInformationAppService()
        {
            this.LocalizationSourceName = SplitPackageConsts.LocalizationSourceName;
        }

        public LogisticChannelOptionDto GetLogisticChannelsOptions()
        {
            var chargeWays = new List<OptionDto<int>>();
            var channelTypes = new List<OptionDto<int>>();
            foreach (var item in Enum.GetValues(typeof(ChargeWay)))
            {
                ChargeWay cw = (ChargeWay)item;
                chargeWays.Add(new OptionDto<int>() {
                    Value = (int)cw,
                    Label = L(cw.ToString())
                });
            }
            foreach (var item in Enum.GetValues(typeof(ChannelType)))
            {
                ChannelType ct = (ChannelType)item;
                channelTypes.Add(new OptionDto<int>()
                {
                    Value = (int)ct,
                    Label = ct.ToString()
                });
            }
            return new LogisticChannelOptionDto()
            {
                ChargeWay = chargeWays,
                ChannelType = channelTypes
            };
        }
    }
}
