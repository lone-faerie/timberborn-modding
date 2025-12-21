using System;
using System.Collections.Generic;
using System.Linq;
using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.Common;
using Timberborn.EnterableSystem;
using Timberborn.EntitySystem;
using Timberborn.TickSystem;
using Timberborn.WorkSystem;

namespace Mods.BetterHealthcare.Scripts.VisitableSystem
{
    public class Visitable : 
        TickableComponent,
        IAwakableComponent,
        IInitializableEntity,
        IFinishedStateListener,
        IRegisteredComponent,
        ILateTickable
    {
        private BlockObject _blockObject;
        private Enterable _enterable;
        private Workplace _workplace;
        private readonly HashSet<Enterer> _visitorsInside = new();
        private readonly HashSet<Worker> _workersInside = new();
        private readonly List<VisitableToggle> _toggles = new();
        private int _numberOfIncomingVisitors;
        private int _numberOfIncomingWorkers;

        private bool _numberOfWorkersChanged;

        public event EventHandler<VisitorAddedEventArgs> VisitorAdded;
        public event EventHandler<VisitorRemovedEventArgs> VisitorRemoved;
        
        public VisitableSpec VisitableSpec { get; private set; }

        public int Capacity => _enterable.Capacity;
        
        public int WorkerCapacity => !_blockObject.IsFinished ? 0 : _workplace.DesiredWorkers;

        public int VisitorCapacity => !_blockObject.IsFinished ? 0 : _enterable.Capacity - WorkerCapacity;
        
        public int VisitorCapacityFinished => _enterable.EnterableSpec.CapacityFinished - _workplace._workplaceSpec.DefaultWorkers;

        public int NumberOfVisitorsInside => _visitorsInside.Count;

        public int NumberOfWorkersInside { get; private set; }

        public int MinWorkers => VisitableSpec.MinWorkers;
        
        public bool CanReserveSlot
        {
            get
            {
                if (!Enabled)
                    return false;
                return NumberOfReservedSlots < VisitorCapacity;
            }
        }

        public bool CanVisitorEnter
        {
            get
            {
                if (!Enabled)
                    return false;
                return _visitorsInside.Count < VisitorCapacity;
            }
        }

        public bool CanWorkerEnter
        {
            get
            {
                if (!Enabled)
                    return false;
                return _workersInside.Count < WorkerCapacity;
            }
        }
        
        public bool AreVisitorsPaused { get; private set; }
        
        public IEnumerable<Enterer> VisitorsInside => _visitorsInside.AsReadOnlyEnumerable();

        public IEnumerable<Worker> WorkersInside => _workersInside.AsReadOnlyEnumerable();

        public void Awake()
        {
            VisitableSpec = GetComponent<VisitableSpec>();
            _blockObject = GetComponent<BlockObject>();
            _enterable = GetComponent<Enterable>();
            _workplace = GetComponent<Workplace>();
            DisableComponent();
        }

        public override void Tick()
        {
            if (!_numberOfWorkersChanged)
                return;
            NumberOfWorkersInside = _workplace.AssignedWorkers.FastCount(w => w.JobRunning);
            _numberOfWorkersChanged = false;
        }

        public void InitializeEntity()
        {
            if (ShouldOperate)
                EnableComponent();
            else
                DisableComponent();
        }

        public void OnEnterFinishedState()
        {
            _enterable.EntererAdded += OnEntererAdded;
            _enterable.EntererRemoved += OnEntererRemoved;
            EnableComponent();
        }

        public void OnExitFinishedState()
        {
            _enterable.EntererAdded -= OnEntererAdded;
            _enterable.EntererRemoved -= OnEntererRemoved;
            ForceRemoveEveryone();
            DisableComponent();
        }

        private void OnEntererAdded(object sender, EntererAddedEventArgs e)
        {
            Worker worker = e.Enterer.GetComponent<Worker>();
            if (worker is null || !AddWorker(worker))
                AddVisitor(e.Enterer);
        }

        private void OnEntererRemoved(object sender, EntererRemovedEventArgs e)
        {
            Worker worker = e.Enterer.GetComponent<Worker>();
            if (worker is null || !RemoveWorker(worker))
                RemoveVisitor(e.Enterer);
        }

        public VisitableToggle GetVisitableToggle()
        {
            VisitableToggle toggle = new VisitableToggle();
            _toggles.Add(toggle);
            toggle.StateChanged += UpdateVisitableState;
            return toggle;
        }

        private void UpdateVisitableState(object sender, EventArgs e)
        {
            AreVisitorsPaused = _toggles.FastAny(t => t is not null && t.Paused);
        }

        public bool IsVisiting(Enterer enterer) => _visitorsInside.Contains(enterer);

        public bool IsVisiting(Visitor visitor) => IsVisiting(visitor.GetComponent<Enterer>());

        public bool HasEnoughWorkers()
        {
            return NumberOfWorkersInside >= MinWorkers;
        }

        public bool AddWorker(Worker worker)
        {
            if (worker.Workplace != _workplace || !CanWorkerEnter)
                return false;
            _workersInside.Add(worker);
            _numberOfWorkersChanged = true;
            EventHandler<VisitorAddedEventArgs> visitorAdded = VisitorAdded;
            if (visitorAdded != null)
                visitorAdded(this, new(worker.GetComponent<Enterer>(), true));
            return true;
        }
        
        public void AddVisitor(Enterer enterer)
        {
            if (!CanVisitorEnter)
                return;
            _visitorsInside.Add(enterer);
            UnreserveSlot();
            EventHandler<VisitorAddedEventArgs> visitorAdded = VisitorAdded;
            if (visitorAdded == null)
                return;
            visitorAdded(this, new(enterer, false));
        }

        public bool RemoveWorker(Worker worker)
        {
            if (!_workersInside.Contains(worker) || !_workersInside.Remove(worker))
                return false;
            _numberOfWorkersChanged = true;
            EventHandler<VisitorRemovedEventArgs> visitorRemoved = VisitorRemoved;
            if (visitorRemoved != null)
                visitorRemoved(this, new(worker.GetComponent<Enterer>(), true));
            return true;
        }

        public void RemoveVisitor(Enterer enterer)
        {
            if (!_visitorsInside.Contains(enterer))
                return;
            _visitorsInside.Remove(enterer);
            EventHandler<VisitorRemovedEventArgs> visitorRemoved = VisitorRemoved;
            if (visitorRemoved == null)
                return;
            visitorRemoved(this, new(enterer, false));
        }

        public void ReserveSlot()
        {
            ++_numberOfIncomingVisitors;
            ValidateReservedSlots();
        }

        public void UnreserveSlot()
        {
            --_numberOfIncomingVisitors;
            ValidateReservedSlots();
        }

        public bool ShouldOperate => _blockObject.IsFinished;

        public int NumberOfReservedSlots => _numberOfIncomingVisitors + _visitorsInside.Count;

        public void ForceRemoveEveryone()
        {
            foreach (Enterer enterer in _visitorsInside.ToArray())
            {
                enterer.Abandon();
                RemoveVisitor(enterer);
            }            
        }

        private void ValidateReservedSlots()
        {
            if (NumberOfReservedSlots >= 0 && NumberOfReservedSlots <= VisitorCapacity)
                return;
            if (NumberOfReservedSlots < 0)
                _numberOfIncomingVisitors = 0;
            else if (NumberOfReservedSlots > VisitorCapacity)
                _numberOfIncomingVisitors = VisitorCapacity;
        }
    }
}