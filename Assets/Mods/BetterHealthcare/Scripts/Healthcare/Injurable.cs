using System;
using Timberborn.BaseComponentSystem;
using Timberborn.NeedSystem;
using Timberborn.SingletonSystem;

namespace Mods.BetterHealthcare.Scripts.Healthcare
{
    public class Injurable : BaseComponent, IAwakableComponent
    {
        private static readonly string InjuryNeedId = "Injury";
        private readonly EventBus _eventBus;
        private NeedManager _needManager;

        public event EventHandler InjuryChanged;

        public Injurable(EventBus eventBus) => _eventBus = eventBus;
        
        public bool IsInjured => _needManager.NeedIsActive(InjuryNeedId);
        
        public void Awake()
        {
            _needManager = GetComponent<NeedManager>();
            _needManager.NeedChangedActiveState += OnNeedChangedActiveState;
        }

        private void OnNeedChangedActiveState(object sender, NeedChangedActiveStateEventArgs e)
        {
            if (e.NeedSpec.Id != InjuryNeedId)
                return;
            EventHandler injuryChanged = InjuryChanged;
            if (injuryChanged != null)
                injuryChanged(this, EventArgs.Empty);
            _eventBus.Post(new InjurableInjuryChangedEvent(this));
        }
    }
}