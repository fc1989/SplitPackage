using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SplitPackage.Business.Products
{
    public interface IProductAppService
    {
        Task<bool> Verify(string sku);
    }
}
