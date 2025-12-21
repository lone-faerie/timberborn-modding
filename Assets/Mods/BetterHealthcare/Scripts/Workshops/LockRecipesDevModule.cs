using Timberborn.Debugging;
using Timberborn.EntitySystem;
using Timberborn.Workshops;

namespace Mods.BetterHealthcare.Scripts.Workshops
{
    public class LockRecipesDevModule : IDevModule
    {
        private readonly LockedRecipeSpecService _lockedRecipeSpecService;
        private readonly RecipeUnlockingService _recipeUnlockingService;
        private readonly EntityComponentRegistry _entityComponentRegistry;
        
        public LockRecipesDevModule(
                LockedRecipeSpecService lockedRecipeSpecService,
                RecipeUnlockingService recipeUnlockingService,
                EntityComponentRegistry entityComponentRegistry)
        {
            _lockedRecipeSpecService = lockedRecipeSpecService;
            _recipeUnlockingService = recipeUnlockingService;
            _entityComponentRegistry = entityComponentRegistry;
        }
        
        public DevModuleDefinition GetDefinition()
        {
            return new DevModuleDefinition.Builder().AddMethod(
                    DevMethod.Create("Reset locked recipes", LockRecipes)).Build();
        }

        private void LockRecipes()
        {
            _recipeUnlockingService.ClearUnlockedRecipes();
            foreach (Manufactory manufactory in _entityComponentRegistry.GetEnabled<Manufactory>())
            {
                if (!manufactory.HasCurrentRecipe)
                    continue;
                if (_lockedRecipeSpecService.IsLockedRecipe(manufactory.CurrentRecipe.Id))
                    manufactory.SetRecipe(null);
            }
        }
    }
}