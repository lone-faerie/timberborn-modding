using Mods.BetterHealthcare.Scripts.WorkshopsUI;
using Timberborn.BatchControl;
using Timberborn.BuildingsUI;
using Timberborn.ConstructionSitesUI;
using Timberborn.CoreUI;
using Timberborn.EntitySystem;
using Timberborn.FieldsUI;
using Timberborn.ForestryUI;
using Timberborn.GatheringUI;
using Timberborn.HaulingUI;
using Timberborn.PlantingUI;
using Timberborn.StatusSystemUI;
using Timberborn.WorkshopsUI;
using Timberborn.WorkSystemUI;
using ManufactoryBatchControlRowItemFactory = Mods.BetterHealthcare.Scripts.WorkshopsUI.ManufactoryBatchControlRowItemFactory;

// ReSharper disable UnusedMember.Local
// ReSharper disable InconsistentNaming

namespace Mods.BetterHealthcare.Scripts.WorkplacesBatchControl
{
    public class WorkplacesBatchControlRowFactory
    {
        private readonly VisualElementLoader _visualElementLoader;
        private readonly BuildingBatchControlRowItemFactory _buildingBatchControlRowItemFactory;
        private readonly WorkplacePriorityBatchControlRowItemFactory _workplacePriorityBatchControlRowItemFactory;
        private readonly WorkplaceBatchControlRowItemFactory _workplaceBatchControlRowItemFactory;
        private readonly PlantablePrioritizerBatchControlRowItemFactory _plantablePrioritizerBatchControlRowItemFactory;
        private readonly FarmHouseBatchControlRowItemFactory _farmHouseBatchControlRowItemFactory;
        private readonly GatherablePrioritizerBatchControlRowItemFactory _gatherablePrioritizerBatchControlRowItemFactory;
        private readonly ManufactoryBatchControlRowItemFactory _manufactoryBatchControlRowItemFactory;
        private readonly HaulCandidateBatchControlRowItemFactory _haulCandidateBatchControlRowItemFactory;
        private readonly ConstructionSitePriorityBatchControlRowItemFactory _constructionSitePriorityBatchControlRowItemFactory;
        private readonly StatusBatchControlRowItemFactory _statusBatchControlRowItemFactory;
        private readonly WorkplaceWorkerTypeBatchControlRowItemFactory _workplaceWorkerTypeBatchControlRowItemFactory;
        private readonly ProductivityBatchControlRowItemFactory _productivityBatchControlRowItemFactory;
        private readonly ManufactoryTogglableRecipesBatchControlRowItemFactory _manufactoryTogglableRecipesBatchControlRowItemFactory;
        private readonly ForesterBatchControlRowItemFactory _foresterBatchControlRowItemFactory;

        public WorkplacesBatchControlRowFactory(
                VisualElementLoader visualElementLoader,
                BuildingBatchControlRowItemFactory buildingBatchControlRowItemFactory,
                WorkplacePriorityBatchControlRowItemFactory workplacePriorityBatchControlRowItemFactory,
                WorkplaceBatchControlRowItemFactory workplaceBatchControlRowItemFactory,
                PlantablePrioritizerBatchControlRowItemFactory plantablePrioritizerBatchControlRowItemFactory,
                FarmHouseBatchControlRowItemFactory farmHouseBatchControlRowItemFactory,
                GatherablePrioritizerBatchControlRowItemFactory gatherablePrioritizerBatchControlRowItemFactory,
                ManufactoryBatchControlRowItemFactory manufactoryBatchControlRowItemFactory,
                HaulCandidateBatchControlRowItemFactory haulCandidateBatchControlRowItemFactory,
                ConstructionSitePriorityBatchControlRowItemFactory constructionSitePriorityBatchControlRowItemFactory,
                StatusBatchControlRowItemFactory statusBatchControlRowItemFactory,
                WorkplaceWorkerTypeBatchControlRowItemFactory workplaceWorkerTypeBatchControlRowItemFactory,
                ProductivityBatchControlRowItemFactory productivityBatchControlRowItemFactory,
                ManufactoryTogglableRecipesBatchControlRowItemFactory manufactoryTogglableRecipesBatchControlRowItemFactory,
                ForesterBatchControlRowItemFactory foresterBatchControlRowItemFactory)
        {
            _visualElementLoader = visualElementLoader;
            _buildingBatchControlRowItemFactory = buildingBatchControlRowItemFactory;
            _workplacePriorityBatchControlRowItemFactory = workplacePriorityBatchControlRowItemFactory;
            _workplaceBatchControlRowItemFactory = workplaceBatchControlRowItemFactory;
            _plantablePrioritizerBatchControlRowItemFactory = plantablePrioritizerBatchControlRowItemFactory;
            _farmHouseBatchControlRowItemFactory = farmHouseBatchControlRowItemFactory;
            _gatherablePrioritizerBatchControlRowItemFactory = gatherablePrioritizerBatchControlRowItemFactory;
            _manufactoryBatchControlRowItemFactory = manufactoryBatchControlRowItemFactory;
            _haulCandidateBatchControlRowItemFactory = haulCandidateBatchControlRowItemFactory;
            _constructionSitePriorityBatchControlRowItemFactory = constructionSitePriorityBatchControlRowItemFactory;
            _statusBatchControlRowItemFactory = statusBatchControlRowItemFactory;
            _workplaceWorkerTypeBatchControlRowItemFactory = workplaceWorkerTypeBatchControlRowItemFactory;
            _productivityBatchControlRowItemFactory = productivityBatchControlRowItemFactory;
            _manufactoryTogglableRecipesBatchControlRowItemFactory = manufactoryTogglableRecipesBatchControlRowItemFactory;
            _foresterBatchControlRowItemFactory = foresterBatchControlRowItemFactory;
        }

        public BatchControlRow Create(EntityComponent entity)
        {
            return new (_visualElementLoader.LoadVisualElement("Game/BatchControl/MyBatchControlRow"),
                        entity,
                        _buildingBatchControlRowItemFactory.Create(entity),
                        _workplacePriorityBatchControlRowItemFactory.Create(entity),
                        _workplaceWorkerTypeBatchControlRowItemFactory.Create(entity),
                        _workplaceBatchControlRowItemFactory.Create(entity),
                        _haulCandidateBatchControlRowItemFactory.Create(entity),
                        _productivityBatchControlRowItemFactory.Create(entity),
                        _farmHouseBatchControlRowItemFactory.Create(entity),
                        _plantablePrioritizerBatchControlRowItemFactory.Create(entity),
                        _foresterBatchControlRowItemFactory.Create(entity),
                        _gatherablePrioritizerBatchControlRowItemFactory.Create(entity),
                        _manufactoryBatchControlRowItemFactory.Create(entity),
                        _manufactoryTogglableRecipesBatchControlRowItemFactory.Create(entity),
                        _constructionSitePriorityBatchControlRowItemFactory.Create(entity),
                        _statusBatchControlRowItemFactory.Create(entity));
        }
    }
}