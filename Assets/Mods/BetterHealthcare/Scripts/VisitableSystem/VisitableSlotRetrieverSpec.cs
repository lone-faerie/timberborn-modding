using System.Collections.Immutable;
using Timberborn.BlueprintSystem;

namespace Mods.BetterHealthcare.Scripts.VisitableSystem
{
    public record VisitableSlotRetrieverSpec : ComponentSpec
    {
        [Serialize]
        public ImmutableArray<string> WorkerSlotNames { get; init; }
    }
}