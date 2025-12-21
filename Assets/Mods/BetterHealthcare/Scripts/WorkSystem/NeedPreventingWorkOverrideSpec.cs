using System.Collections.Immutable;
using Timberborn.BlueprintSystem;

namespace Mods.BetterHealthcare.Scripts.WorkSystem
{
    public record NeedPreventingWorkOverrideSpec : ComponentSpec
    {
        [Serialize]
        public ImmutableArray<string> OverrideNeedIds { get; init; }
    }
}