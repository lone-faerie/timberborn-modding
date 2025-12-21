using System.Collections.Immutable;
using Timberborn.BlueprintSystem;
using Timberborn.NeedSpecs;

namespace Mods.BetterHealthcare.Scripts.EffectWorkshops
{
    public record ManufactoryEffectProducerSpec : ComponentSpec
    {
        [Serialize]
        public ImmutableArray<ContinuousEffectSpec> IdleEffects { get; init; }
    }
}