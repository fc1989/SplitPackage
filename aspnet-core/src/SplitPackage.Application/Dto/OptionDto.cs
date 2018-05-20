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

    public class Option
    {
        public string Value { get; set; }

        public string label { get; set; }

        public IList<Option> Children { get; set; }
    }
}
