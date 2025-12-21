using Bindito.Core;
using Mods.BetterHealthcare.Scripts.VisitableSystem;
using Timberborn.EntityPanelSystem;
using Timberborn.TemplateInstantiation;

namespace Mods.BetterHealthcare.Scripts.VisitableSystemUI
{
    [Context("Game")]
    public class VisitableSystemUIConfigurator : Configurator
    {
        protected override void Configure()
        {
            Bind<VisitableDescriber>().AsTransient();
            Bind<VisitableFragment>().AsSingleton();
            Bind<VisitableLoadRateFragment>().AsSingleton();

            MultiBind<TemplateModule>().ToProvider(ProvideTemplateModule).AsSingleton();
            MultiBind<EntityPanelModule>().ToProvider<EntityPanelModuleProvider>().AsSingleton();
        }

        private static TemplateModule ProvideTemplateModule()
        {
            TemplateModule.Builder builder = new TemplateModule.Builder();
            builder.AddDecorator<Visitable, VisitableDescriber>();
            return builder.Build();
        }

        private class EntityPanelModuleProvider : IProvider<EntityPanelModule>
        {
            private readonly VisitableFragment _visitableFragment;
            private readonly VisitableLoadRateFragment _visitableLoadRateFragment;

            public EntityPanelModuleProvider(VisitableFragment visitableFragment,
                VisitableLoadRateFragment visitableLoadRateFragment)
            {
                _visitableFragment = visitableFragment;
                _visitableLoadRateFragment = visitableLoadRateFragment;
            }
            
            public EntityPanelModule Get()
            {
                EntityPanelModule.Builder builder = new EntityPanelModule.Builder();
                builder.AddTopFragment(_visitableLoadRateFragment, -100);
                builder.AddTopFragment(_visitableFragment, -100);
                return builder.Build();
            }
        }
    }
}