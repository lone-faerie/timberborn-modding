using Mods.BetterHealthcare.Scripts.Common;
using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Linq;
using Mods.BetterHealthcare.Scripts.VisitableSystem;
using Timberborn.BaseComponentSystem;
using Timberborn.Effects;
using Timberborn.GameFactionSystem;
using Timberborn.NeedSpecs;
using Timberborn.NeedSystem;
using Timberborn.Workshops;

namespace Mods.BetterHealthcare.Scripts.EffectWorkshops
{

    public class ManufactoryEffectProducer : BaseComponent, IAwakableComponent, IStartableComponent
    {
        private readonly FactionNeedService _factionNeedService;
        
        private Manufactory _manufactory;
        private Visitable _visitable;

        private ContinuousEffect _effect;
        private RecipeSpec _currentRecipe;
        private bool _recipeIsEffect;
        private readonly HashSet<NeedManager> _needManagers = new();
        private FrozenDictionary<string, ContinuousEffectSpec> _effects;

        public event EventHandler EffectChanged;
        
        public ContinuousEffect Effect => _effect;
        
        public IReadOnlyList<ContinuousEffect> IdleEffects { get; private set; }

        public IReadOnlyList<ContinuousEffectSpec> Effects => _effects.Values;

        public ManufactoryEffectProducer(FactionNeedService factionNeedService) =>
            _factionNeedService = factionNeedService;

        public bool RecipeIsEffect => !string.IsNullOrEmpty(_effect.NeedId);

        public bool HasIdleEffects => IdleEffects.Count > 0;

        public bool IsReadyToProduce {
            get
            {
                if (!RecipeIsEffect)
                    return false;
                return _manufactory.HasAllIngredients && _manufactory.HasFuel && _manufactory.HasPower;
            }
        }

        public void Awake()
        {
            _manufactory = GetComponent<Manufactory>();
            _visitable = GetComponent<Visitable>();
            InitializeEffects();
        }

        public void Start()
        {
            InitializeVisitors();
            UpdateRecipe();
            _manufactory.RecipeChanged += (_1, _2) => UpdateRecipe();
            _visitable.VisitorAdded += OnVisitorAdded;
            _visitable.VisitorRemoved += OnVisitorRemoved;
        }

        private void UpdateRecipe()
        {
            _manufactory.ProductionProgressed -= OnProductionProgressed;
            RecipeSpec currentRecipe = _manufactory.CurrentRecipe;
            if (currentRecipe is null || !_effects.ContainsKey(currentRecipe.Id))
            {
                _effect = new("", 0.0f);
                _currentRecipe = null;
            }
            else
            {
                _effect = ContinuousEffect.FromSpec(_effects[currentRecipe.Id]);
                _currentRecipe = currentRecipe;
                EventHandler effectChanged = EffectChanged;
                if (effectChanged != null)
                    effectChanged(this, EventArgs.Empty);
                _manufactory.ProductionProgressed += OnProductionProgressed;
            }
        }

        private void OnProductionProgressed(object sender, ProductionProgressedEventArgs e)
        {
            if (_manufactory.CurrentRecipe != _currentRecipe || _currentRecipe is null)
                return;
            float num = e.ProductionProgressChange * _manufactory.CurrentRecipe.CycleDurationInHours;
            foreach (var needManager in _needManagers)
                needManager.ApplyEffect(_effect, num);
        }

        private void OnVisitorAdded(object sender, VisitorAddedEventArgs e)
        {
            if (e.IsWorker)
                return;
            if (e.Enterer.HasComponent<NeedManager>())
                _needManagers.Add(e.Enterer.GetComponent<NeedManager>());
        }

        private void OnVisitorRemoved(object sender, VisitorRemovedEventArgs e)
        {
            if (e.IsWorker)
                return;
            if (e.Enterer.HasComponent<NeedManager>())
                _needManagers.Remove(e.Enterer.GetComponent<NeedManager>());
            if (_visitable.NumberOfVisitorsInside != 0)
                return;
            _manufactory.ProductionProgressed -= OnProductionProgressed;
            _manufactory.ResetProductionProgress();
            _manufactory.ProductionProgressed += OnProductionProgressed;
        }

        private void InitializeVisitors()
        {
            foreach (var visitor in _visitable.VisitorsInside)
            {
                if (visitor.HasComponent<NeedManager>())
                    _needManagers.Add(visitor.GetComponent<NeedManager>());
            }
        }

        private void InitializeEffects()
        {
            var idleEffects = GetComponent<ManufactoryEffectProducerSpec>().IdleEffects;
            if (!idleEffects.IsDefaultOrEmpty)
                IdleEffects = idleEffects.Where(spec => _factionNeedService.IsCurrentFactionNeed(spec.NeedId))
                        .Select(ContinuousEffect.FromSpec)
                        .ToList();
            else
                IdleEffects = new List<ContinuousEffect>();
            _effects = _manufactory.ProductionRecipes
                                   .Where(recipe => recipe.IsEffect())
                                   .ToFrozenDictionary(
                                       recipe => recipe.Id,
                                       recipe => new ContinuousEffectSpec
                                       {
                                           NeedId = NeedIdFromRecipeId(recipe.Id),
                                           PointsPerHour = 1f / recipe.CycleDurationInHours,
                                           SatisfyToMaxValue = false
                                       });
        }

        private static string NeedIdFromRecipeId(string recipeId)
        {
            int index = recipeId.IndexOf('.', 7);
            if (index == -1)
                index = recipeId.Length;
            return recipeId[7..index];
        }
    }

}