using SplitPackage.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace SplitPackage.Business.Logistics.Dto
{
    public class LogisticSearchFilter : PageSearchFilter<Logistic>
    {
        public string CorporationName { get; set; }
    }
}
