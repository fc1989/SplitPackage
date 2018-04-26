using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SplitPackage.Authorization;
using SplitPackage.Business.SplitRules.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SplitPackage.Business.SplitRules
{
    [AbpAuthorize(PermissionNames.Pages_Tenant_SplitRules)]
    public class SplitRuleAppService : AsyncCrudAppService<SplitRule, SplitRuleDto, long, PagedResultRequestDto, CreateSplitRuleDto, UpdateSplitRuleDto>, ISplitRuleAppService
    {
        private readonly IRepository<SplitRuleProductClass, long> _srpRepository;

        protected static IMapper SRMapper = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<SplitRule, SplitRuleDto>()
                .ForMember(dest => dest.RuleItems,
                    opt => opt.MapFrom(src => src.ProductClasses.Select(oi => new RuleItemDto()
                    {
                        ProductClassId = oi.ProductClassId,
                        MaxNum = oi.MaxNum,
                        MinNum = oi.MinNum
                    })))
                 .ForMember(dest => dest.LogisticLineName, opt => opt.MapFrom(src => src.LogisticLineBy.LineName));
            cfg.CreateMap<UpdateSplitRuleDto, SplitRule>();
            cfg.CreateMap<SplitRuleProductClass, RuleItemDto>().ForMember(dest => dest.ProductClassName,
                opt => opt.MapFrom(src => src.ProductClassBy.ClassName));
        }).CreateMapper();

        public SplitRuleAppService(IRepository<SplitRule, long> repository, IRepository<SplitRuleProductClass, long> srpRepository) : base(repository)
        {
            this._srpRepository = srpRepository;
        }

        public override async Task<PagedResultDto<SplitRuleDto>> GetAll(PagedResultRequestDto input)
        {
            CheckGetAllPermission();

            var query = CreateFilteredQuery(input);

            var totalCount = await AsyncQueryableExecuter.CountAsync(query);

            query = ApplySorting(query, input);
            query = ApplyPaging(query, input);

            var entities = await AsyncQueryableExecuter.ToListAsync(query);

            return new PagedResultDto<SplitRuleDto>(
                totalCount,
                entities.Select(o => SRMapper.Map<SplitRuleDto>(o)).ToList()
            );
        }

        public override async Task<SplitRuleDto> Create(CreateSplitRuleDto input)
        {
            CheckCreatePermission();

            var entity = MapToEntity(input);
            entity.TenantId = AbpSession.TenantId;
            var ruleId = await this.Repository.InsertAndGetIdAsync(entity);
            List<SplitRuleProductClass> ruleItems = new List<SplitRuleProductClass>();
            foreach (var item in input.RuleItems)
            {
                var ruleItem = new SplitRuleProductClass()
                {
                    ProductClassId = item.ProductClassId,
                    SplitRuleId = ruleId,
                    MinNum = item.MinNum,
                    MaxNum = item.MaxNum
                };
                ruleItems.Add(ruleItem);
                await this._srpRepository.InsertAsync(ruleItem);
            }
            entity.ProductClasses = ruleItems;
            return SRMapper.Map<SplitRuleDto>(entity);
        }

        public override async Task<SplitRuleDto> Update(UpdateSplitRuleDto input)
        {
            CheckUpdatePermission();

            var entity = this.Repository.GetAll().Include(p => p.ProductClasses).FirstOrDefault(o => o.Id == input.Id);
            SRMapper.Map(input, entity);

            //insert
            var icl = input.RuleItems.Where(o => !entity.ProductClasses.Any(oi => oi.ProductClassId == o.ProductClassId)).ToList();
            foreach (var item in icl)
            {
                await this._srpRepository.InsertAsync(new SplitRuleProductClass()
                {
                    ProductClassId = item.ProductClassId,
                    SplitRuleId = input.Id,
                    MinNum = item.MinNum,
                    MaxNum = item.MaxNum
                });
            }

            //delete
            var dcl = entity.ProductClasses.Where(o => !input.RuleItems.Select(oi => oi.ProductClassId).Contains(o.ProductClassId)).Select(o => o.Id).ToList();
            if (dcl.Count > 0)
            {
                await this._srpRepository.DeleteAsync(o => dcl.Contains(o.Id));
            }

            await CurrentUnitOfWork.SaveChangesAsync();

            return SRMapper.Map<SplitRuleDto>(entity);
        }
    }
}
