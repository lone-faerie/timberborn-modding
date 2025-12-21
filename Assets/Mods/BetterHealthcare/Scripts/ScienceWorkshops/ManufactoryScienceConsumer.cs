using System;
using System.Collections.Generic;
using System.Linq;
using Mods.BetterHealthcare.Scripts.VisitableSystem;
using Timberborn.BaseComponentSystem;
using Timberborn.ScienceSystem;
using Timberborn.Workshops;

namespace Mods.BetterHealthcare.Scripts.ScienceWorkshops
{
    public class ManufactoryScienceConsumer : BaseComponent, IAwakableComponent, IStartableComponent
    {
        private readonly ScienceService _scienceService;
        
        private Manufactory _manufactory;
        private Visitable _visitable;
        private IReadOnlyList<ScienceRecipe> _recipes;
        private VisitableToggle _visitableToggle;
        
        private ScienceRecipe _currentRecipe;
        private float _currentScience;

        public IReadOnlyList<ScienceRecipe> Recipes => _recipes;
        
        public event EventHandler<NotEnoughScienceStateChangedEventArgs> NotEnoughScienceChanged;
        
        public ManufactoryScienceConsumer(ScienceService scienceService) => _scienceService = scienceService;
        
        public bool RecipeConsumesScience => _currentRecipe is not null && _currentRecipe.ConsumedSciencePoints > 0;

        public int ConsumedSciencePoints => _currentRecipe is null ? 0 : _currentRecipe.ConsumedSciencePoints;
        
        public bool NotEnoughScience { get; private set; }

        public void Awake()
        {
            _manufactory = GetComponent<Manufactory>();
            _visitable = GetComponent<Visitable>();
            if (_visitable is not null)
                _visitableToggle = _visitable.GetVisitableToggle();
            _recipes = GetComponent<ManufactoryScienceConsumerSpec>().ScienceRecipes;
        }

        public void Start()
        {
            UpdateRecipe();
            _manufactory.RecipeChanged += (_1, _2) => UpdateRecipe();
        }

        private void UpdateRecipe()
        {
            _manufactory.ProductionProgressed -= OnProductionProgressed;
            _manufactory.ProductionFinished -= OnProductionFinished;
            RecipeSpec currentRecipe = _manufactory.CurrentRecipe;
            if (currentRecipe is null)
                return;
            _currentRecipe = _recipes.FirstOrDefault(r => r.RecipeId == currentRecipe.Id);
            if (!RecipeConsumesScience)
            {
                UpdateNotEnoughScience();
                return;
            }
            _manufactory.ProductionProgressed += OnProductionProgressed;
            _manufactory.ProductionFinished += OnProductionFinished;
        }

        private void OnProductionProgressed(object sender, ProductionProgressedEventArgs e)
        {
            if (_currentScience <= 0)
            {
                RefillScience();
                UpdateNotEnoughScience();
            }

            if (NotEnoughScience)
                return;
            UseScience(e.ProductionProgressChange);
        }

        private void OnProductionFinished(object sender, EventArgs e) => _currentScience = 0.0f;

        private void RefillScience()
        {
            if (_scienceService.SciencePoints < _currentRecipe.ConsumedSciencePoints)
                return;
            _scienceService.SubtractPoints(1);
            _currentScience = 1f;
        }

        private void UseScience(float productionProgressChange)
        {
            _currentScience -= _currentRecipe.ConsumedSciencePoints * productionProgressChange;
        }

        private void UpdateNotEnoughScience()
        {
            bool flag = RecipeConsumesScience && _currentScience <= 0;
            if (NotEnoughScience == flag)
                return;
            NotEnoughScience = flag;
            if (_visitableToggle is not null)
                _visitableToggle.ToggleVisitable(NotEnoughScience);
            EventHandler<NotEnoughScienceStateChangedEventArgs> notEnoughScienceChanged = NotEnoughScienceChanged;
            if (notEnoughScienceChanged == null)
                return;
            notEnoughScienceChanged(this, new NotEnoughScienceStateChangedEventArgs(flag));
        }
    }
}