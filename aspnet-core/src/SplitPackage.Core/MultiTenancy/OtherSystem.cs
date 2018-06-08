using Abp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace SplitPackage.MultiTenancy
{
    [Table("OtherSystems")]
    public class OtherSystem : Entity<long>, IPassivable
    {
        public OtherSystem()
        {
            IsActive = true;
        }

        public const int MaxSystemNameLength = 50;

        public const int MaxCertificateLength = 100;

        public string SystemName { get; set; }

        public string Certificate { get; set; }

        public bool IsActive { get; set; }
    }
}
