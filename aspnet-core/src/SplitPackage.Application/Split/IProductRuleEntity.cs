using SplitPackage.Split.SplitModels;
using System;
using System.Collections.Generic;

namespace SplitPackage.Split
{
    public interface IProductRuleEntity
    {
        ProductRuleType GetType();

        Tuple<SplitedOrder, List<ProductEntity>> Split(List<ProductEntity> productList, bool withTax);
    }

    public enum ProductRuleType
    {
        Single,
        Mixed,
    }
}
