using System.Collections.Generic;
using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.Common;
using Timberborn.InventorySystem;
using Timberborn.Workshops;
using UnityEngine;

namespace Mods.BetterHealthcare.Scripts.WorkshopsEffects
{
    public class ManufactoryInventoryVisualizer : BaseComponent, IAwakableComponent, IFinishedStateListener
    {
        private Manufactory _manufactory;
        private ManufactoryInventoryVisualizerSpec _manufactoryInventoryVisualizerSpec;
        private readonly Dictionary<IReadOnlyList<string>, GameObject> _goodsModels = new();
        private bool _isShowing = true;
        
        public void Awake()
        {
            _manufactory = GetComponent<Manufactory>();
            _manufactoryInventoryVisualizerSpec = GetComponent<ManufactoryInventoryVisualizerSpec>();
            _manufactory.Inventory.InventoryStockChanged += OnInventoryStockChanged;
            DisableComponent();
            InitializeGoodsModels();
            UpdateVisualization();
        }

        public void OnEnterFinishedState()
        {
            EnableComponent();
            UpdateVisualization();
        }

        public void OnExitFinishedState()
        {
            DisableComponent();
            UpdateVisualization();
        }

        private void OnInventoryStockChanged(object sender, InventoryAmountChangedEventArgs e) => UpdateVisualization();

        private void InitializeGoodsModels()
        {
            foreach (var goodsModel in _manufactoryInventoryVisualizerSpec.GoodsModels)
                _goodsModels.Add(goodsModel.GoodIds, GameObject.FindChild(goodsModel.ModelName));
        }

        private void UpdateVisualization()
        {
            if (_isShowing && (!Enabled || _manufactory.Inventory.IsEmpty))
            {
                HideAll();
                return;
            }

            foreach (var goodsModel in _goodsModels)
            {
                goodsModel.Deconstruct(out var goodIds, out var gameObject);
                bool flag = goodIds.FastAll(ShouldShow);
                gameObject.SetActive(flag);
                _isShowing = _isShowing || flag;
            }
        }

        private bool ShouldShow(string goodId)
        {
            return _manufactory.Inventory.LimitedAmount(goodId) > 0 | _manufactory.Inventory.AmountInStock(goodId) > 0;
        }

        private void HideAll()
        {
            _isShowing = false;
            foreach (var gameObject in _goodsModels.Values)
                gameObject.SetActive(false);
        }
    }
}