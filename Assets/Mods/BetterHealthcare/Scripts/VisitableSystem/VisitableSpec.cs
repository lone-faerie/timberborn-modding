using Timberborn.BlueprintSystem;

namespace Mods.BetterHealthcare.Scripts.VisitableSystem
{
    public record VisitableSpec : ComponentSpec
    {
        [Serialize]
        public int MinWorkers { get; init; }
    }
}