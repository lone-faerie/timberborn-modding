using Bindito.Core;
using Mods.BetterHealthcare.Scripts.Common;
using System;
using Timberborn.BatchControl;

namespace Mods.BetterHealthcare.Scripts.WorkplacesBatchControl
{
    [Context("Game")]
    public class WorkplacesBatchControlConfigurator : Configurator
    {
        private static readonly string PatchId = typeof(WorkplacesBatchControlConfigurator).FullName;
        private static readonly Type[] Patches = new[] {
                typeof(BatchControlModulePatch)
        };
        
        protected override void Configure()
        {
            Bind<WorkplacesBatchControlTab>().AsSingleton();
            Bind<WorkplacesBatchControlRowFactory>().AsSingleton();
            MultiBind<BatchControlModule>().ToProvider<BatchControlModuleProvider>().AsSingleton();
            
            HarmonyPatcher.Patch(PatchId, Patches);
        }

        private class BatchControlModuleProvider : IProvider<BatchControlModule>
        {
            private readonly WorkplacesBatchControlTab _workplacesBatchControlTab;

            public BatchControlModuleProvider(WorkplacesBatchControlTab workplacesBatchControlTab)
            {
                _workplacesBatchControlTab = workplacesBatchControlTab;
            }

            public BatchControlModule Get()
            {
                BatchControlModule.Builder builder = new BatchControlModule.Builder();
                builder.AddTab(_workplacesBatchControlTab, 3);
                return builder.Build();
            }
        }
    }
}