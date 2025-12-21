using System.Collections.Frozen;
using System.Collections.Generic;
using System.Linq;
using Timberborn.BlueprintSystem;
using Timberborn.SingletonSystem;
using Timberborn.Workshops;

namespace Mods.BetterHealthcare.Scripts.Workshops
{
    public class LockedRecipeSpecService : ILoadableSingleton
    {
        private readonly ISpecService _specService;
        private FrozenDictionary<string, LockedRecipeSpec> _lockedRecipeSpecs;

        public LockedRecipeSpecService(ISpecService specService) => _specService = specService;
        
        public void Load()
        {
            _lockedRecipeSpecs = _specService.GetSpecs<RecipeSpec>()
                    .Where(spec => spec.HasSpec<LockedRecipeSpec>())
                    .ToFrozenDictionary(
                            spec => spec.Id,
                            spec => spec.GetSpec<LockedRecipeSpec>()
                    );
        }

        public bool IsLockedRecipe(string recipeId) => _lockedRecipeSpecs.ContainsKey(recipeId);

        public LockedRecipeSpec GetLockedRecipe(string recipeId)
        {
            return IsLockedRecipe(recipeId) ? _lockedRecipeSpecs[recipeId] : null;
        }

        public IEnumerable<LockedRecipeSpec> GetLockedRecipes() => _lockedRecipeSpecs.Values;
    }
}