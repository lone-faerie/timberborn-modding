using Timberborn.Workshops;

namespace Mods.BetterHealthcare.Scripts.Workshops
{
    public class RecipeUnlockedEvent
    {
        public RecipeSpec RecipeSpec { get; }

        public RecipeUnlockedEvent(RecipeSpec recipeSpec) => RecipeSpec = recipeSpec;
    }
}