using System.Collections.Immutable;
using Timberborn.BlueprintSystem;

namespace Mods.BetterHealthcare.Scripts.WorkshopsEffects
{
    public record ManufactoryInventoryVisualizerSpec : ComponentSpec
    {
        [Serialize]
        public ImmutableArray<GoodsModel> GoodsModels { get; init; }
    }
}