using System.Collections.Generic;
using Mods.BetterHealthcare.Scripts.VisitableSystem;
using Timberborn.BaseComponentSystem;
using Timberborn.CoreUI;
using Timberborn.EntityPanelSystem;
using Timberborn.Localization;
using UnityEngine;

namespace Mods.BetterHealthcare.Scripts.VisitableSystemUI
{
    public class VisitableDescriber : BaseComponent, IAwakableComponent, IEntityDescriber
    {
        private static readonly string VisitorsLimitLocKey = "Attractions.VisitorsLimit";
        
        private readonly ILoc _loc;
        private Visitable _visitable;

        public VisitableDescriber(ILoc loc)
        {
            _loc = loc;
        }
        
        public void Awake()
        {
            _visitable = GetComponent<Visitable>();
        }

        public IEnumerable<EntityDescription> DescribeEntity()
        {
            if (!_visitable.Enabled)
            {
                string str = _loc.T(VisitorsLimitLocKey, _visitable.VisitorCapacityFinished);
                Debug.Log(SpecialStrings.RowStarter + str);
                yield return EntityDescription.CreateTextSection(SpecialStrings.RowStarter + str, 10);
            }
        }
    }
}