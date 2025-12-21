using System.Collections.Immutable;
using Timberborn.BlueprintSystem;

namespace Mods.BetterHealthcare.Scripts.WorkshopsEffects
{
    public record GoodsModel
    {
        [Serialize]
        public ImmutableArray<string> GoodIds { get; init; }
        
        [Serialize]
        public string ModelName { get; init; }
    }
}