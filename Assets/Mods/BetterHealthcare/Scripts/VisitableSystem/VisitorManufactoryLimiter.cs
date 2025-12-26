using Mods.BetterHealthcare.Scripts.Common;
using Timberborn.BaseComponentSystem;
using Timberborn.Workshops;

namespace Mods.BetterHealthcare.Scripts.VisitableSystem
{
    public class VisitorManufactoryLimiter : BaseComponent, IAwakableComponent, IManufactoryLimiter
    {
        private Visitable _visitable;
        private Manufactory _manufactory;
        
        public void Awake()
        {
            _visitable = GetComponent<Visitable>();
            _manufactory = GetComponent<Manufactory>();
        }

        public float ProductionEfficiency()
        {
            if (_manufactory.HasCurrentRecipe && !_manufactory.CurrentRecipe.RequiresVisitor())
                return 1f;
            if (_visitable.NumberOfVisitorsInside == 0 ||
                _visitable.AreVisitorsPaused ||
                !_visitable.HasEnoughWorkers())
                return 0.0f;
            return 1f / _visitable.NumberOfVisitorsInside;
        }

        public float MaxProductionProgressChange() => ProductionEfficiency();
    }
}