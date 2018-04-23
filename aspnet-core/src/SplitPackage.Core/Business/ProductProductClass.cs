using Abp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace SplitPackage.Business
{
    [Table("Product_ProductClass")]
    public class ProductProductClass : Entity<long>
    {
        public long ProductId { get; set; }

        public Product ProductBy { get; set; }

        public long ProductClassId { get; set; }

        public ProductClass ProductClassBy { get; set; }
    }
}
