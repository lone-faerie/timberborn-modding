using Timberborn.BlueprintSystem;

namespace Mods.BetterHealthcare.Scripts.ScienceWorkshops
{
    public record ScienceRecipe : ComponentSpec
    {
        [Serialize]
        public string RecipeId { get; init; }
        
        [Serialize]
        public int ConsumedSciencePoints { get; init; }
    }
}