using AutoMapper;
using SplitPackage.Domain.Logistic;
using System;
using System.Collections.Generic;
using System.Text;

namespace SplitPackage.Business.Logistics
{
    public class LogisticMapProfile : Profile
    {
        public LogisticMapProfile()
        {
            CreateMap<Logistic, ModifyLogisticEvent>();
        }
    }
}
