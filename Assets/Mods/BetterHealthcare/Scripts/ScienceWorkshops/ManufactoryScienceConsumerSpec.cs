using System.Collections.Immutable;
using Timberborn.BlueprintSystem;

namespace Mods.BetterHealthcare.Scripts.ScienceWorkshops
{
    public record ManufactoryScienceConsumerSpec : ComponentSpec
    {
        [Serialize]
        public ImmutableArray<ScienceRecipe> ScienceRecipes { get; init; }
    }
}