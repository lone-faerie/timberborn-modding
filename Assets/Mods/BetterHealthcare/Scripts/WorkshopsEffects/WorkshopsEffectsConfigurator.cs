using Bindito.Core;
using HarmonyLib;
using Mods.BetterHealthcare.Scripts.Common;
using System;
using Timberborn.TemplateInstantiation;
using UnityEngine;

namespace Mods.BetterHealthcare.Scripts.WorkshopsEffects
{
    [Context("Game")]
    public class WorkshopsEffectsConfigurator : Configurator
    {
        private static readonly string PatchId = nameof(WorkshopsEffectsConfigurator);
        private static readonly Type[] Patches = new[] {
                typeof(WorkshopWorkerHiderPatch)
        };
        
        protected override void Configure()
        {
            Bind<ManufactoryInventoryVisualizer>().AsTransient();
            MultiBind<TemplateModule>().ToProvider(ProvideTemplateModule).AsSingleton();
            
            HarmonyPatcher.Patch(PatchId, Patches);
        }

        private static TemplateModule ProvideTemplateModule()
        {
            TemplateModule.Builder builder = new TemplateModule.Builder();
            builder.AddDecorator<ManufactoryInventoryVisualizerSpec, ManufactoryInventoryVisualizer>();
            return builder.Build();
        }
    }
}