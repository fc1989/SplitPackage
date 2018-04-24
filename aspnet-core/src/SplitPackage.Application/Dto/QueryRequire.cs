using System;
using System.Collections.Generic;
using System.Text;

namespace SplitPackage.Business.ProductClasses.Dto
{
    public class QueryRequire<T> where T :struct
    {
        public string Flag { get; set; }

        public List<T> Ids { get; set; }
    }
}
