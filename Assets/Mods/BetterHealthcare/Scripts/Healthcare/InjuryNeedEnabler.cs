using System;
using Timberborn.BaseComponentSystem;
using Timberborn.EntitySystem;
using Timberborn.NeedSpecs;
using Timberborn.NeedSystem;
using UnityEngine;

namespace Mods.BetterHealthcare.Scripts.Healthcare
{
    public class InjuryNeedEnabler : BaseComponent, IAwakableComponent, IInitializableEntity
    {
        private static readonly string SalveNeedId = "Salve";

        private NeedManager _needManager;
        private Injurable _injurable;

        public void Awake()
        {
            _needManager = GetComponent<NeedManager>();
            _injurable = GetComponent<Injurable>();
            _injurable.InjuryChanged += OnInjuryChanged;
        }

        public void InitializeEntity() => UpdateNeeds();

        private void OnInjuryChanged(object sender, EventArgs e) => UpdateNeeds();

        private void UpdateNeeds()
        {
            foreach (NeedSpec needSpec in _needManager.NeedSpecs)
            {
                if (ShouldBeEnabled(needSpec))
                {
                    _needManager.EnableNeed(needSpec.Id);
                    Debug.Log($"Enable {needSpec.Id}");
                } else
                {
                    _needManager.ResetNeed(needSpec.Id);
                    _needManager.DisableNeed(needSpec.Id);
                }
            }
        }

        private bool ShouldBeEnabled(NeedSpec needSpec)
        {
            if (IsEnabledOnlyWhenInjured(needSpec))
                return _injurable.IsInjured;
            return true;
        }

        private static bool IsEnabledOnlyWhenInjured(NeedSpec needSpec)
        {
            return needSpec.Id == SalveNeedId;
        }
    }
}