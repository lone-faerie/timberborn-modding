using System;
using Bindito.Core;
using HarmonyLib;
using Mods.BetterHealthcare.Scripts.Common;
using Timberborn.Beavers;
using Timberborn.EnterableSystem;
using Timberborn.SlotSystem;
using Timberborn.TemplateInstantiation;
using UnityEngine;

namespace Mods.BetterHealthcare.Scripts.VisitableSystem
{
    [Context("Game")]
    public class VisitableSystemConfigurator : Configurator
    {
        private static readonly string PatchId = typeof(VisitableSystemConfigurator).FullName;

        private static readonly Type[] Patches = new[]
        {
            typeof(EnterableNeedSuspendingBuildingPatch)
        };
        
        protected override void Configure()
        {
            // Bind<Visitable3>().AsTransient();
            Bind<Visitable>().AsTransient();
            Bind<Visitor>().AsTransient();
            Bind<VisitExecutor>().AsTransient();
            Bind<VisitableLoadRate>().AsTransient();
            Bind<VisitorManufactoryLimiter>().AsTransient();
            Bind<VisitableSlotManager>().AsTransient();
            Bind<VisitableSlotRetriever>().AsTransient();
            MultiBind<TemplateModule>().ToProvider(ProvideTemplateModule).AsSingleton();

            HarmonyPatcher.Patch(PatchId, Patches);
        }

        private static TemplateModule ProvideTemplateModule()
        {
            TemplateModule.Builder builder = new TemplateModule.Builder();
            builder.AddDecorator<Enterer, Visitor>();
            builder.AddDecorator<VisitableSpec, Visitable>();
            builder.AddDecorator<Visitable, VisitableLoadRate>();
            builder.AddDecorator<VisitableSlotManagerSpec, VisitableSlotManager>();
            builder.AddDecorator<VisitableSlotManager, SlotManager>();
            builder.AddDecorator<VisitableSlotRetrieverSpec, VisitableSlotRetriever>();
            builder.AddDecorator<AdultSpec, VisitExecutor>();
            return builder.Build();
        }
    }
}