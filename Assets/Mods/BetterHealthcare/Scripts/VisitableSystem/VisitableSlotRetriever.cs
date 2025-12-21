using System.Collections.Generic;
using System.Linq;
using Timberborn.BaseComponentSystem;
using Timberborn.SlotSystem;

namespace Mods.BetterHealthcare.Scripts.VisitableSystem
{
    public class VisitableSlotRetriever : BaseComponent, IAwakableComponent, ICustomSlotRetriever
    {
        private VisitableSlotRetrieverSpec _visitableSlotRetrieverSpec;
        private SlotManager _slotManager;
        private bool _nextIsWorker;
        
        public void Awake()
        {
            _visitableSlotRetrieverSpec = GetComponent<VisitableSlotRetrieverSpec>();
            _slotManager = GetComponent<SlotManager>();
        }

        public bool TryGetUnassignedSlot(IEnumerable<ISlot> slots, out ISlot slot)
        {
            return _slotManager._randomNumberGenerator.TryGetEnumerableElement(
                slots.Where(_nextIsWorker ? IsAvailableWorkerSlot : IsAvailableVisitorSlot),
                out slot
            );
        }

        public void ToggleNextIsWorker(bool newValue) => _nextIsWorker = newValue;

        private bool IsAvailableVisitorSlot(ISlot slot)
        {
            return slot.IsAvailable && slot.AssignedEnterer is null &&
                   !_visitableSlotRetrieverSpec.WorkerSlotNames.Contains(slot.Name);
        }
        
        private bool IsAvailableWorkerSlot(ISlot slot)
        {
            return slot.IsAvailable && slot.AssignedEnterer is null &&
                   _visitableSlotRetrieverSpec.WorkerSlotNames.Contains(slot.Name);
        }
    }
}