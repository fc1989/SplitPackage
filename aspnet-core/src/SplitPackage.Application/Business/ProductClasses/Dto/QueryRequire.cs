using System;
using System.Collections.Generic;
using System.Text;

namespace SplitPackage.Business.ProductClasses.Dto
{
    public class QueryRequire
    {
        public string Flag { get; set; }

        public List<long> Ids { get; set; }
    }
}
