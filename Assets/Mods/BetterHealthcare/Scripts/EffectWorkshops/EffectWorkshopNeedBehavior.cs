using Mods.BetterHealthcare.Scripts.VisitableSystem;
using System;
using System.Collections.Generic;
using Timberborn.BaseComponentSystem;
using Timberborn.BehaviorSystem;
using Timberborn.BlockSystem;
using Timberborn.Buildings;
using Timberborn.EnterableSystem;
using Timberborn.GameDistricts;
using Timberborn.NeedBehaviorSystem;
using Timberborn.NeedSpecs;
using Timberborn.NeedSystem;
using Timberborn.TimeSystem;
using Timberborn.WalkingSystem;
using UnityEngine;

namespace Mods.BetterHealthcare.Scripts.EffectWorkshops
{
    public class EffectWorkshopNeedBehavior : NeedBehavior, IAwakableComponent, IFinishedStateListener
    {
        private readonly IDayNightCycle _dayNightCycle;
        
        private BuildingAccessible _buildingAccessible;
        private Enterable _enterable;
        private Visitable _visitable;
        private ManufactoryEffectProducer _manufactoryEffectProducer;
        private DistrictBuilding _districtBuilding;
        private DistrictNeedBehaviorService _districtNeedBehaviorService;
        private readonly List<ContinuousEffectSpec> _effects = new();

        public EffectWorkshopNeedBehavior(IDayNightCycle dayNightCycle)
        {
            _dayNightCycle = dayNightCycle;
        }
        
        public void Awake()
        {
            _manufactoryEffectProducer = GetComponent<ManufactoryEffectProducer>();
            _visitable = GetComponent<Visitable>();
            _enterable = GetComponent<Enterable>();
            _buildingAccessible = GetComponent<BuildingAccessible>();
            _districtBuilding = GetComponent<DistrictBuilding>();
            DisableComponent();
        }

        public override Vector3? ActionPosition(NeedManager needManager)
        {
            Enterer enterer = needManager.GetComponent<Enterer>();
            if (_visitable.CanVisitorEnter || _visitable.IsVisiting(enterer))
            {
                var unblockedSingleAccess =
                        _buildingAccessible.Accessible.UnblockedSingleAccess;
                if (unblockedSingleAccess.HasValue)
                    return new Vector3?(unblockedSingleAccess.GetValueOrDefault());
            }
            return new Vector3?();
        }

        public void OnEnterFinishedState()
        {
            _districtBuilding.ReassignedDistrict += OnReassignedDistrict;
            _manufactoryEffectProducer.EffectChanged += OnEffectChanged;
            UpdateEffect();
            EnableComponent();
        }

        public void OnExitFinishedState()
        {
            _districtBuilding.ReassignedDistrict -= OnReassignedDistrict;
            _manufactoryEffectProducer.EffectChanged -= OnEffectChanged;
            RemoveDistrictNeedBehaviorService();
            DisableComponent();
        }

        public override Decision Decide(BehaviorAgent agent)
        {
            if (!_manufactoryEffectProducer.RecipeIsEffect || !_manufactoryEffectProducer.IsReadyToProduce || !Enabled)
            {
                return Decision.ReleaseNow();
            }
            Enterer enterer = agent.GetComponent<Enterer>();
            if (_visitable.IsVisiting(enterer))
                return DecideVisiting(agent);
            if (!_visitable.HasEnoughWorkers())
            {
                return Decision.ReleaseNow();
            }
            WalkInsideExecutor executor1 = agent.GetComponent<WalkInsideExecutor>();
            switch (executor1.Launch(_enterable))
            {
                case ExecutorStatus.Success:
                    return Visit(agent);
                case ExecutorStatus.Failure:
                    return Decision.ReleaseNextTick();
                case ExecutorStatus.Running:
                    return Decision.ReturnWhenFinished(executor1);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private Decision DecideVisiting(BehaviorAgent agent)
        {
            if (!_visitable.HasEnoughWorkers())
            {
                if (_manufactoryEffectProducer.HasIdleEffects)
                    return ApplyEffect(agent);
                return Decision.ReleaseNow();
            }
            return Visit(agent);
        }

        private Decision Visit(BehaviorAgent agent)
        {
            VisitExecutor executor = agent.GetComponent<VisitExecutor>();
            executor.Launch(_visitable, 0.05f);
            return Decision.ReleaseWhenFinished(executor);
        }
        
        private Decision ApplyEffect(BehaviorAgent agent)
        {
            ApplyEffectExecutor executor = agent.GetComponent<ApplyEffectExecutor>();
            float timestamp = _dayNightCycle.DayNumberHoursFromNow(0.05f);
            executor.LaunchToTimestamp(_manufactoryEffectProducer.IdleEffects, timestamp);
            return Decision.ReleaseWhenFinished(executor);
        }

        private void OnEffectChanged(object sender, EventArgs e) => UpdateEffect();

        private void UpdateEffect()
        {
            RemoveDistrictNeedBehaviorService();
            _effects.Clear();
            if (!_manufactoryEffectProducer.RecipeIsEffect)
                return;
            ContinuousEffectSpec effect = new ContinuousEffectSpec {
                    NeedId = _manufactoryEffectProducer.Effect.NeedId,
                    PointsPerHour = _manufactoryEffectProducer.Effect.PointsPerHour,
                    SatisfyToMaxValue = false
            };
            _effects.Add(effect);
            if (_districtNeedBehaviorService is null && !GetDistrictNeedBehaviorService())
                return;
            _districtNeedBehaviorService!.AddNeedBehavior(_effects, this);
        }

        public void OnReassignedDistrict(object sender, EventArgs e)
        {
            RemoveDistrictNeedBehaviorService();
            if (!GetDistrictNeedBehaviorService())
                return;
            _districtNeedBehaviorService.AddNeedBehavior(_effects, this);
        }

        private bool GetDistrictNeedBehaviorService()
        {
            DistrictCenter district = _districtBuilding.District;
            if (district is null)
                return false;
            _districtNeedBehaviorService = district.GetComponent<DistrictNeedBehaviorService>();
            return true;
        }

        private void RemoveDistrictNeedBehaviorService()
        {
            if (_districtNeedBehaviorService is null)
                return;
            _districtNeedBehaviorService.RemoveNeedBehavior(_effects, this);
            _districtNeedBehaviorService = null;
        }
    }
}