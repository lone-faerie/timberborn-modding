using Bindito.Core;
using Mods.BetterHealthcare.Scripts.Common;
using System;
using Timberborn.Debugging;

namespace Mods.BetterHealthcare.Scripts.Workshops
{
    [Context("Game")]
    public class WorkshopsConfigurator : Configurator
    {
        private static readonly string PatchId = nameof(WorkshopsConfigurator);
        private static readonly Type[] Patches = new[] {
            typeof(ManufactoryPatch)
        };
        
        protected override void Configure()
        {
            Bind<LockedRecipeSpecService>().AsSingleton();
            Bind<RecipeUnlockingService>().AsSingleton();
            MultiBind<IDevModule>().To<LockRecipesDevModule>().AsSingleton();
            
            HarmonyPatcher.Patch(PatchId, Patches);
        }
    }
}