using System;
using System.Collections.Generic;
using System.Text;

namespace SplitPackage.Dto
{
    public class OptionDto<T>
    {
        public T Value { get; set; }

        public string Label { get; set; }
    }
}
