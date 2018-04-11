using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SplitPackage.Split.Common
{
    public class ListComparer<T> : IEqualityComparer<List<T>>
    {
        public bool Equals(List<T> x, List<T> y)
        {
            if (Object.ReferenceEquals(x, y))
            {
                return true;
            }
            if (Object.ReferenceEquals(x, null) || Object.ReferenceEquals(y, null))
            {
                return false;
            }

            return x.SequenceEqual(y);
        }

        // If Equals() returns true for a pair of objects 
        // then GetHashCode() must return the same value for these objects.
        public int GetHashCode(List<T> ruleEntities)
        {
            if (Object.ReferenceEquals(ruleEntities, null)) return 0;

            int hashCode = 0;
            ruleEntities.ForEach(r => hashCode ^= r.GetHashCode());

            return hashCode;
        }
    }
}
