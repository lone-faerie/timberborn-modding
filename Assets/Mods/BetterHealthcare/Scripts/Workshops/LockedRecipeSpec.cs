using Timberborn.BlueprintSystem;

namespace Mods.BetterHealthcare.Scripts.Workshops
{
    public record LockedRecipeSpec : ComponentSpec
    {
        [Serialize]
        public int ScienceCost { get; init; }
    }
}