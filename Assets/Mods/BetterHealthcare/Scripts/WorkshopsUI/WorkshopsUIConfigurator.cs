using System;
using Bindito.Core;
using HarmonyLib;
using Mods.BetterHealthcare.Scripts.Common;
using Timberborn.EntityPanelSystem;
using Timberborn.TemplateInstantiation;
using Timberborn.Workshops;
using UnityEngine;

namespace Mods.BetterHealthcare.Scripts.WorkshopsUI
{
    [Context("Game")]
    public class WorkshopsUIConfigurator : Configurator
    {
        private static readonly string PatchId = nameof(WorkshopsUIConfigurator);

        private static readonly Type[] Patches = new[]
        {
            typeof(ManufactoryInventoryFragmentPatch),
            typeof(ManufactoryDescriberPatch),
            typeof(EntityPanelModulePatch),
        };
        
        protected override void Configure()
        {
            Bind<ManufactoryFragment>().AsSingleton();
            Bind<ManufactoryAdvancedDropdownProvider>().AsTransient();
            Bind<RecipeUnlockingDialogService>().AsSingleton();
            Bind<ManufactoryBatchControlRowItemFactory>().AsSingleton();
            MultiBind<TemplateModule>().ToProvider(ProvideTemplateModule).AsSingleton();
            MultiBind<EntityPanelModule>().ToProvider<EntityPanelModuleProvider>().AsSingleton();
            HarmonyPatcher.Patch(PatchId, Patches);
        }

        private static TemplateModule ProvideTemplateModule()
        {
            TemplateModule.Builder builder = new TemplateModule.Builder();
            builder.AddDecorator<Manufactory, ManufactoryAdvancedDropdownProvider>();
            return builder.Build();
        }

        private class EntityPanelModuleProvider : IProvider<EntityPanelModule>
        {
            private readonly ManufactoryFragment _manufactoryFragment;

            public EntityPanelModuleProvider(ManufactoryFragment manufactoryFragment)
            {
                _manufactoryFragment = manufactoryFragment;
            }
            
            public EntityPanelModule Get()
            {
                EntityPanelModule.Builder builder = new EntityPanelModule.Builder();
                builder.AddMiddleFragment(_manufactoryFragment);
                return builder.Build();
            }
        }
    }
}