using System.Collections.Generic;
using Mods.BetterHealthcare.Scripts.VisitableSystem;
using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.CharactersUI;
using Timberborn.CoreUI;
using Timberborn.EnterableSystem;
using Timberborn.EntityPanelSystem;
using Timberborn.SelectionSystem;
using Timberborn.SingletonSystem;
using UnityEngine.UIElements;

namespace Mods.BetterHealthcare.Scripts.VisitableSystemUI
{
    public class VisitableFragment : IEntityPanelFragment
    {
        private readonly VisualElementLoader _visualElementLoader;
        private readonly EntitySelectionService _entitySelectionService;
        private readonly CharacterButtonFactory _characterButtonFactory;
        private readonly EventBus _eventBus;

        private VisualElement _root;
        private VisualElement _buildingUserButtons;
        private Visitable _visitable;

        private readonly List<CharacterButton> _userButtons = new();

        public VisitableFragment(
            VisualElementLoader visualElementLoader,
            EntitySelectionService entitySelectionService,
            CharacterButtonFactory characterButtonFactory,
            EventBus eventBus)
        {
            _visualElementLoader = visualElementLoader;
            _entitySelectionService = entitySelectionService;
            _characterButtonFactory = characterButtonFactory;
            _eventBus = eventBus;
        }

        public VisualElement InitializeFragment()
        {
            _root = _visualElementLoader.LoadVisualElement("Game/EntityPanel/AttractionFragment");
            _buildingUserButtons = _root.Q("BeaverUserButtons");
            _root.ToggleDisplayStyle(false);
            _eventBus.Register(this);
            return _root;
        }

        [OnEvent]
        public void OnEnterFinishedState(EnteredFinishedStateEvent enteredFinishedStateEvent)
        {
            if (_visitable is null || enteredFinishedStateEvent.BlockObject != _visitable.GetComponent<BlockObject>())
                return;
            InitializeButtons();
        }

        public void ShowFragment(BaseComponent entity)
        {
            _visitable = entity.GetComponent<Visitable>();
            if (_visitable is null)
                return;
            _visitable.VisitorAdded += OnVisitorAdded;
            _visitable.VisitorRemoved += OnVisitorRemoved;
            InitializeButtons();
            if (!_visitable.Enabled)
                return;
            UpdateButtons();
        }

        public void ClearFragment()
        {
            if (_visitable is not null)
            {
                _visitable.VisitorAdded -= OnVisitorAdded;
                _visitable.VisitorRemoved -= OnVisitorRemoved;
            }

            _visitable = null;
            _root.ToggleDisplayStyle(false);
        }

        public void UpdateFragment()
        {
            _root.ToggleDisplayStyle(_visitable is not null && _visitable.Enabled);
        }

        private void OnVisitorAdded(object sender, VisitorAddedEventArgs e) {
            if (!e.IsWorker)
                UpdateButtons();
        }

        private void OnVisitorRemoved(object sender, VisitorRemovedEventArgs e) {
            if (!e.IsWorker)
                UpdateButtons();
        }

        private void InitializeButtons()
        {
            RemoveAllButtons();
            for (int index = 0; index < _visitable.VisitorCapacity; index++)
                AddEmptyButton();
        }

        private void RemoveAllButtons()
        {
            foreach (var characterButton in _userButtons)
                _buildingUserButtons.Remove(characterButton.Root);
            _userButtons.Clear();
        }

        private void AddEmptyButton()
        {
            CharacterButton characterButton = _characterButtonFactory.Create();
            characterButton.ShowEmpty();
            _userButtons.Add(characterButton);
            _buildingUserButtons.Add(characterButton.Root);
        }
        
        private void UpdateButtons()
        {
            int index = 0;
            foreach (Enterer enterer1 in _visitable.VisitorsInside)
            {
                Enterer enterer = enterer1;
                _userButtons[index++].ShowFilled(enterer, () => _entitySelectionService.SelectAndFollow(enterer));
            }
            for (; index < _userButtons.Count; ++index)
                _userButtons[index].ShowEmpty();
        }
    }
}