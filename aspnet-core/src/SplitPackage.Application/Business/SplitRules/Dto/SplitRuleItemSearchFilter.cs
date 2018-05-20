using Abp.Application.Services.Dto;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace SplitPackage.Business.SplitRules.Dto
{
    public class SplitRuleItemFilter : PagedResultRequestDto
    {
        public long SplitRuleId { get; set; }
    }
}
