using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.CharacterModelSystem;
using Timberborn.EnterableSystem;
using Timberborn.SlotSystem;

namespace Mods.BetterHealthcare.Scripts.VisitableSystem
{
    public class VisitableSlotManager : BaseComponent, IAwakableComponent, IFinishedStateListener
    {
        private Visitable _visitable;
        private VisitableSlotRetriever _visitableSlotRetriever;
        private SlotManager _slotManager;
        
        public void Awake()
        {
            _visitable = GetComponent<Visitable>();
            _visitableSlotRetriever = GetComponent<VisitableSlotRetriever>();
            _slotManager = GetComponent<SlotManager>();
            _slotManager.EntererAssignedToSlot += OnEntererAssignedToSlot;
            DisableComponent();
        }

        public void OnEnterFinishedState()
        {
            _visitable.VisitorAdded += OnVisitorAdded;
            _visitable.VisitorRemoved += OnVisitorRemoved;
            EnableComponent();
        }

        public void OnExitFinishedState()
        {
            _visitable.VisitorAdded -= OnVisitorAdded;
            _visitable.VisitorRemoved -= OnVisitorRemoved;
            DisableComponent();
        }

        private void OnEntererAssignedToSlot(object sender, Enterer e)
        {
            ShowEnterer(e);
        }

        private void OnVisitorAdded(object sender, VisitorAddedEventArgs e)
        {
            _visitableSlotRetriever.ToggleNextIsWorker(e.IsWorker);
            if (_slotManager.AddEnterer(e.Enterer))
                return;
            e.Enterer.GetComponent<CharacterModel>().Hide();
        }

        private void OnVisitorRemoved(object sender, VisitorRemovedEventArgs e)
        {
            _slotManager.RemoveEnterer(e.Enterer);
            ShowEnterer(e.Enterer);
        }
        
        private static void ShowEnterer(Enterer enterer) => enterer.GetComponent<CharacterModel>().Show();
    }
}