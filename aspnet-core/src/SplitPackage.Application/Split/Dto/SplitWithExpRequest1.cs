using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SplitPackage.Split.Dto
{
    public class SplitWithExpRequest1 : BaseRequest
    {
        /// <summary>
        /// 物流企业唯一ID
        /// </summary>
        public List<string> logistics { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}