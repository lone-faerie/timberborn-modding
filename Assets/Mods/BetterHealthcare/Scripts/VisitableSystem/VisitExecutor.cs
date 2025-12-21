using Timberborn.BaseComponentSystem;
using Timberborn.BehaviorSystem;
using Timberborn.BlockingSystem;
using Timberborn.Persistence;
using Timberborn.TimeSystem;
using Timberborn.WorldPersistence;

namespace Mods.BetterHealthcare.Scripts.VisitableSystem
{
    public class VisitExecutor : BaseComponent, IAwakableComponent, IExecutor
    {
        private static readonly ComponentKey VisitExecutorKey = new(nameof(VisitExecutor));
        private static readonly PropertyKey<float> FinishTimestampKey = new("FinishTimestamp");
        private static readonly PropertyKey<bool> IgnoreMinWorkersKey = new("IgnoreMinWorkers");
        private static readonly PropertyKey<Visitable> VisitableKey = new("Visitable");

        private readonly IDayNightCycle _dayNightCycle;
        private readonly ReferenceSerializer _referenceSerializer;

        private Visitor _visitor;
        private Visitable _visitable;
        private BlockableObject _blockableObject;

        private float _finishTimestamp;
        private bool _ignoreMinWorkers;

        public VisitExecutor(IDayNightCycle dayNightCycle, ReferenceSerializer referenceSerializer)
        {
            _dayNightCycle = dayNightCycle;
            _referenceSerializer = referenceSerializer;
        }
        
        public void Awake()
        {
            _visitor = GetComponent<Visitor>();
        }

        public bool Launch(Visitable visitable, float maxVisitTimeInHours)
        {
            return Launch(visitable, maxVisitTimeInHours, false);
        }

        public bool LaunchIgnoringMinWorkers(Visitable visitable, float maxVisitTimeInHours)
        {
            return Launch(visitable, maxVisitTimeInHours, true);
        }

        public ExecutorStatus Tick(float deltaTimeInHours)
        {
            if (_visitable is null)
                return ExecutorStatus.Failure;
            if (!_blockableObject.IsUnblocked)
            {
                _visitor.Unreserve();
                return ExecutorStatus.Failure;
            }
            if (_dayNightCycle.PartialDayNumber > _finishTimestamp
                || (!_ignoreMinWorkers && _visitable.NumberOfWorkersInside < _visitable.MinWorkers))
            {
                _visitor.Unreserve();
                _ignoreMinWorkers = false;
                return ExecutorStatus.Success;
            }
            return ExecutorStatus.Running;
        }

        public void Save(IEntitySaver entitySaver)
        {
            IObjectSaver component = entitySaver.GetComponent(VisitExecutorKey);
            if (_visitable is not null)
                component.Set(VisitableKey, _visitable, _referenceSerializer.Of<Visitable>());
            component.Set(FinishTimestampKey, _finishTimestamp);
            component.Set(IgnoreMinWorkersKey, _ignoreMinWorkers);
        }

        public void Load(IEntityLoader entityLoader)
        {
            if (!entityLoader.TryGetComponent(VisitExecutorKey, out var component))
                return;
            _finishTimestamp = component.Get(FinishTimestampKey);
            _ignoreMinWorkers = component.Get(IgnoreMinWorkersKey);
            Visitable visitable = component.Has(VisitableKey)
                    ? component.Get(VisitableKey, _referenceSerializer.Of<Visitable>())
                    : _visitor.CurrentBuilding;
            Initialize(visitable);
            VerifyLoadedVisitable();
        }

        private bool Launch(Visitable visitable, float maxVisitTimeInHours, bool ignoreMinWorkers)
        {
            Initialize(visitable);
            if (_visitable is null)
                return false;
            if (!ignoreMinWorkers && _visitable.NumberOfWorkersInside < _visitable.MinWorkers)
                return false;
            _finishTimestamp = _dayNightCycle.DayNumberHoursFromNow(maxVisitTimeInHours);
            _ignoreMinWorkers = ignoreMinWorkers;
            _visitor.Reserve(_visitable);
            return true;
        }
        
        private void Initialize(Visitable visitable)
        {
            if (visitable is null)
                return;
            _visitable = visitable;
            _blockableObject = _visitable.GetComponent<BlockableObject>();
        }

        private void VerifyLoadedVisitable()
        {
            if (_visitable is not null)
                return;
            _blockableObject = null;
        }
    }
}