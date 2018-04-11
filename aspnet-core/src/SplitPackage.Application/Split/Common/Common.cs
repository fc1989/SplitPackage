using System;
using System.Collections.Generic;

namespace SplitPackage.Split.Common
{
    public static class Common
    {

        public static List<ProductEntity> CloneProductEntityList(List<ProductEntity> source)
        {
            List<ProductEntity> result = new List<ProductEntity>();
            if (source == null)
            {
                throw new ArgumentNullException();
            }

            foreach (var pe in source)
            {
                ProductEntity pen = pe.Clone();
                result.Add(pen);
            }
            return result;
        }
    }
}
