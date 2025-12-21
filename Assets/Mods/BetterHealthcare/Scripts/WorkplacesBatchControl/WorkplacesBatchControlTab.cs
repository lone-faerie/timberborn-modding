using System.Collections.Generic;
using System.Linq;
using Timberborn.BaseComponentSystem;
using Timberborn.BatchControl;
using Timberborn.CoreUI;
using Timberborn.EntitySystem;
using Timberborn.SingletonSystem;
using Timberborn.WorkSystem;

namespace Mods.BetterHealthcare.Scripts.WorkplacesBatchControl
{
    public class WorkplacesBatchControlTab : BatchControlTab
    {
        private readonly WorkplacesBatchControlRowFactory _workplacesBatchControlRowFactory;
        private readonly BatchControlRowGroupFactory _batchControlRowGroupFactory;

        public WorkplacesBatchControlTab(
                VisualElementLoader visualElementLoader,
                BatchControlDistrict batchControlDistrict,
                WorkplacesBatchControlRowFactory workplacesBatchControlRowFactory,
                BatchControlRowGroupFactory batchControlRowGroupFactory,
                EventBus eventBus) : base(visualElementLoader, batchControlDistrict, eventBus)
        {
            _workplacesBatchControlRowFactory = workplacesBatchControlRowFactory;
            _batchControlRowGroupFactory = batchControlRowGroupFactory;
        }

        public override string TabNameLocKey => "BatchControl.Workplaces";
        public override string TabImage => "Workplaces";
        public override string BindingKey => "WorkplacesTab";

        public override IEnumerable<BatchControlRowGroup> GetRowGroups(IEnumerable<EntityComponent> entities)
        {
            foreach (IGrouping<string, EntityComponent> grouping in entities
                             .Where(entity => (bool) (BaseComponent) entity.GetComponent<Workplace>())
                             .Where(workplace => (bool) (BaseComponent) workplace)
                             .GroupBy(workplace => workplace.GetComponent<LabeledEntitySpec>().DisplayNameLocKey))
            {
                BatchControlRowGroup sortedWithTextHeader = _batchControlRowGroupFactory.CreateSortedWithTextHeader(grouping.Key);
                foreach (EntityComponent entity in grouping)
                    sortedWithTextHeader.AddRow(_workplacesBatchControlRowFactory.Create(entity));
                yield return sortedWithTextHeader;
            }
        }
    }
}