using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.Persistence;
using Timberborn.TickSystem;
using Timberborn.TimeSystem;
using Timberborn.WorldPersistence;

namespace Mods.BetterHealthcare.Scripts.VisitableSystem
{
    public class VisitableLoadRate :
        TickableComponent,
        IAwakableComponent,
        IFinishedStateListener,
        IPersistentEntity
    {
        private static readonly ComponentKey VisitableLoadRateKey = new(nameof(VisitableLoadRate));
        private static readonly ListKey<int> MaxLoadKey = new("MaxLoad");
        private static readonly ListKey<int> ActualLoadKey = new("ActualLoad");

        private readonly IDayNightCycle _dayNightCycle;
        private int[] _maxLoad = new int[24];
        private int[] _actualLoad = new int[24];
        private int _currentHour;
        private Visitable _visitable;

        public VisitableLoadRate(IDayNightCycle dayNightCycle) => _dayNightCycle = dayNightCycle;
        
        public void Awake()
        {
            _visitable = GetComponent<Visitable>();
            DisableComponent();
        }

        public override void Tick()
        {
            ResetValuesOnHourChange();
            CollectSample();
        }

        public void OnEnterFinishedState() => EnableComponent();

        public void OnExitFinishedState() => DisableComponent();

        public float GetLoadRate(int hour)
        {
            return (float)_actualLoad[hour] / _maxLoad[hour];
        }

        public void Save(IEntitySaver entitySaver)
        {
            IObjectSaver component = entitySaver.GetComponent(VisitableLoadRateKey);
            component.Set(MaxLoadKey, _maxLoad);
            component.Set(ActualLoadKey, _actualLoad);
        }

        public void Load(IEntityLoader entityLoader)
        {
            if (!entityLoader.TryGetComponent(VisitableLoadRateKey, out var component))
                return;
            _maxLoad = component.Get(MaxLoadKey).ToArray();
            _actualLoad = component.Get(ActualLoadKey).ToArray();
        }

        private void ResetValuesOnHourChange()
        {
            int hoursPassedToday = (int) _dayNightCycle.HoursPassedToday;
            if (hoursPassedToday == _currentHour)
                return;
            _currentHour = hoursPassedToday;
            _maxLoad[_currentHour] = 0;
            _actualLoad[_currentHour] = 0;
        }

        private void CollectSample()
        {
            _maxLoad[_currentHour] += _visitable.VisitorCapacity;
            _actualLoad[_currentHour] += _visitable.NumberOfVisitorsInside;
        }
    }
}