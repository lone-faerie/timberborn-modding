using System.Collections.Generic;
using System.Text;
using Mods.BetterHealthcare.Scripts.EffectWorkshops;
using Timberborn.BaseComponentSystem;
using Timberborn.Common;
using Timberborn.Effects;
using Timberborn.EntityPanelSystem;

namespace Mods.BetterHealthcare.Scripts.EffectWorkshopsUI
{
    public class EffectWorkshopDescriber : BaseComponent, IAwakableComponent, IEntityDescriber
    {
        private readonly EffectDescriber _effectDescriber;
        private ManufactoryEffectProducer _manufactoryEffectProducer;
        private readonly StringBuilder _description = new();

        public EffectWorkshopDescriber(EffectDescriber effectDescriber) => _effectDescriber = effectDescriber;
        
        public void Awake()
        {
            _manufactoryEffectProducer = GetComponent<ManufactoryEffectProducer>();
        }

        public IEnumerable<EntityDescription> DescribeEntity()
        {
            if (_manufactoryEffectProducer.Effects.Count > 0)
            {
                _effectDescriber.DescribeEffects(_manufactoryEffectProducer.Effects, _description);
                yield return EntityDescription.CreateTextSection(_description.ToStringWithoutNewLineEndAndClean(), 1010);
            }
        }
    }
}