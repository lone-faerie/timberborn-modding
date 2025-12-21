using System;
using Timberborn.BaseComponentSystem;
using Timberborn.EnterableSystem;
using Timberborn.Persistence;
using Timberborn.WorldPersistence;

namespace Mods.BetterHealthcare.Scripts.VisitableSystem
{
    public class Visitor :
            BaseComponent,
            IAwakableComponent,
            IStartableComponent,
            IPersistentEntity
    {
        private static readonly ComponentKey VisitorKey = new(nameof(Visitor));
        private static readonly PropertyKey<bool> FirstVisitKey = new(nameof(FirstVisit));
        private static readonly PropertyKey<Visitable> CurrentBuildingKey = new(nameof(CurrentBuilding));

        private readonly ReferenceSerializer _referenceSerializer;
        private Enterer _enterer;
        private Visitable _loadedCurrentBuilding;
        
        public bool FirstVisit { get; set; }
        
        public Visitable CurrentBuilding { get; private set; }

        public Visitor(ReferenceSerializer referenceSerializer)
        {
            _referenceSerializer = referenceSerializer;
        }

        public bool IsVisiting => CurrentBuilding != null;

        public void Awake() => _enterer = GetComponent<Enterer>();

        public void Start() => ResolveLoadedState();

        public void Save(IEntitySaver entitySaver)
        {
            IObjectSaver component = entitySaver.GetComponent(VisitorKey);
            component.Set(FirstVisitKey, FirstVisit);
            if (!IsVisiting)
                return;
            component.Set(CurrentBuildingKey, CurrentBuilding, _referenceSerializer.Of<Visitable>());
        }

        public void Load(IEntityLoader entityLoader)
        {
            if (!entityLoader.TryGetComponent(VisitorKey, out var component))
                return;
            if (component.Has(FirstVisitKey))
                FirstVisit = component.Get(FirstVisitKey);
            Visitable visitable;
            if (!component.Has(CurrentBuildingKey) || !component.GetObsoletable(CurrentBuildingKey, _referenceSerializer.Of<Visitable>(), out visitable))
                return;
            _loadedCurrentBuilding = visitable;
        }

        public void Reserve(Visitable visitable)
        {
            if (CurrentBuilding is not null)
                throw new InvalidOperationException($"{this} tried to visit {visitable} while already visiting {CurrentBuilding}");
            CurrentBuilding = visitable;
            visitable.ReserveSlot();
        }

        public void Unreserve()
        {
            if (CurrentBuilding is null)
                return;
            CurrentBuilding.UnreserveSlot();
            CurrentBuilding = null;
        }

        public void StartVisit(Visitable visitable)
        {
            if (CurrentBuilding is not null)
                throw new InvalidOperationException($"{this} tried to visit {visitable} while already visiting {CurrentBuilding}");
            CurrentBuilding = visitable;
            visitable.AddVisitor(_enterer);
        }

        public void StopVisit()
        {
            if (CurrentBuilding is null)
                return;
            CurrentBuilding.RemoveVisitor(_enterer);
            CurrentBuilding = null;
        }

        private void ResolveLoadedState()
        {
            if (_loadedCurrentBuilding is null)
                return;
            // StartVisit(_loadedCurrentBuilding);
        }
    }
}