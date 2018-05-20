using Abp.Application.Services.Dto;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace SplitPackage.Business.SplitRules.Dto
{
    public class SplitRuleSearchFilter : PagedResultRequestDto
    {
        public string LogisticName { get; set; }

        public string ChannelName { get; set; }

        [JsonProperty("ptid")]
        public string PTId { get; set; }
    }
}
