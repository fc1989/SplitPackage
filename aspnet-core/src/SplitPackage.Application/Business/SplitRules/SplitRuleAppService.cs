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
            cfg.CreateMap<SplitRuleProductClass, RuleItemDto>().ForMember(dest => dest.ProductClassName,
                opt => opt.MapFrom(src => string.Format("{0}[{1}]", src.ProductClassBy.ClassName, src.ProductClassBy.PTId)))
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id));
            cfg.CreateMap<SplitRule, SplitRuleDto>()
                .ForMember(dest => dest.RuleItems,
                    opt => opt.MapFrom(src => src.ProductClasses.Select(oi => new RuleItemDto()
                    {
                        ProductClassId = oi.ProductClassId,
                        MaxNum = oi.MaxNum,
                        MinNum = oi.MinNum
                    })))
                 .ForMember(dest => dest.LogisticLineName, opt => opt.MapFrom(src => src.LogisticLineBy.LineName))
                 .ForMember(dest => dest.RuleItems, opt => opt.MapFrom(src => src.ProductClasses.Select(o=> SRMapper.Map<RuleItemDto>(o))));
            cfg.CreateMap<UpdateSplitRuleDto, SplitRule>();
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

            var entities = await AsyncQueryableExecuter.ToListAsync(query.Include(p=>p.LogisticLineBy).Include(p=>p.ProductClasses).ThenInclude((SplitRuleProductClass srp) => srp.ProductClassBy));

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

            // delete
            var dcl = entity.ProductClasses.Where(o => !input.RuleItems.Select(oi => oi.ProductClassId).Contains(o.ProductClassId)).ToList();
            if (dcl.Count > 0)
            {
                dcl.ForEach(o => {
                    entity.ProductClasses.Remove(o);
                });
                await this._srpRepository.DeleteAsync(o => dcl.Select(oi => oi.Id).Contains(o.Id));
            }

            // update
            foreach (var item in entity.ProductClasses)
            {
                var update = input.RuleItems.FirstOrDefault(o => o.ProductClassId == item.ProductClassId);
                item.MaxNum = update.MaxNum;
                item.MinNum = update.MinNum;
                await this._srpRepository.UpdateAsync(item);
            }

            // insert
            var icl = input.RuleItems.Where(o => !entity.ProductClasses.Any(oi => oi.ProductClassId == o.ProductClassId)).ToList();
            foreach (var item in icl)
            {
                var srp = new SplitRuleProductClass()
                {
                    ProductClassId = item.ProductClassId,
                    SplitRuleId = input.Id,
                    MinNum = item.MinNum,
                    MaxNum = item.MaxNum
                };
                await this._srpRepository.InsertAsync(srp);
                entity.ProductClasses.Add(srp);
            }

            await CurrentUnitOfWork.SaveChangesAsync();

            return SRMapper.Map<SplitRuleDto>(entity);
        }
    }
}
