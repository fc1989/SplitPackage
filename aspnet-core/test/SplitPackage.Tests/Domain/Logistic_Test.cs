using Abp.Events.Bus;
using SplitPackage.Domain.Logistic;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using Xunit.Abstractions;

namespace SplitPackage.Tests.Domain
{
    [Collection("AstraeaAssistant collection")]
    public class Logistic_Test
    {
        private readonly SplitPackageSettingBase _context;
        private readonly IEventBus _eventBus;

        public Logistic_Test(ITestOutputHelper output, AstraeaAssistantSetting context)
        {
            this._context = context;
            this._eventBus = context.ResolveService<IEventBus>();
        }

        [Fact]
        public void LogisticStartUse_Test()
        {
            this._eventBus.TriggerAsync(new StartUseLogisticEvent() {
                LogisticId = 1
            }).Wait();
        }
    }
}
