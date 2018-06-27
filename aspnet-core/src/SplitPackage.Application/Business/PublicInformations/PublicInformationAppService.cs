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

        public EnumOptionDto GetEnumOptions()
        {
            var chargeWays = new List<OptionDto<int>>();
            var channelTypes = new List<OptionDto<int>>();
            var ruleItemStintType = new List<OptionDto<int>>();
            foreach (ChargeWay item in Enum.GetValues(typeof(ChargeWay)))
            {
                chargeWays.Add(new OptionDto<int>()
                {
                    Value = (int)item,
                    Label = L(item.ToString())
                });
            }
            foreach (ChannelType item in Enum.GetValues(typeof(ChannelType)))
            {
                channelTypes.Add(new OptionDto<int>()
                {
                    Value = (int)item,
                    Label = item.ToString()
                });
            }
            foreach (RuleItemStintType item in Enum.GetValues(typeof(RuleItemStintType)))
            {
                ruleItemStintType.Add(new OptionDto<int>()
                {
                    Value = (int)item,
                    Label = item.ToString()
                });
            }
            return new EnumOptionDto()
            {
                ChargeWay = chargeWays,
                ChannelType = channelTypes,
                RuleItemStintType = ruleItemStintType
            };
        }
    }
}
