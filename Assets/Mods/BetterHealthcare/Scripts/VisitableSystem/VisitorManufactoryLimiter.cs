using Timberborn.BaseComponentSystem;
using Timberborn.Workshops;

namespace Mods.BetterHealthcare.Scripts.VisitableSystem
{
    public class VisitorManufactoryLimiter : BaseComponent, IAwakableComponent, IManufactoryLimiter
    {
        private Visitable _visitable;
        
        public void Awake()
        {
            _visitable = GetComponent<Visitable>();
        }

        public float ProductionEfficiency()
        {
            if (_visitable.NumberOfVisitorsInside == 0 ||
                _visitable.AreVisitorsPaused ||
                !_visitable.HasEnoughWorkers())
                return 0.0f;
            return 1f / _visitable.NumberOfVisitorsInside;
        }

        public float MaxProductionProgressChange() => ProductionEfficiency();
    }
}