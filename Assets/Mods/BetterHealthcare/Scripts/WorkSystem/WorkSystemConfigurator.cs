using Bindito.Core;
using HarmonyLib;
using Mods.BetterHealthcare.Scripts.Common;
using System;
using Timberborn.TemplateInstantiation;

namespace Mods.BetterHealthcare.Scripts.WorkSystem
{
    [Context("Game")]
    public class WorkSystemConfigurator : Configurator
    {
        private static readonly string PatchId = typeof(WorkSystemConfigurator).FullName;
        private static readonly Type[] Patches = new[] {
                typeof(DecoratorDefinitionPatch)
        };
        
        protected override void Configure()
        {
            Bind<WorkRefuser>().AsTransient();
            HarmonyPatcher.Patch(PatchId, Patches);
        }
    }
}