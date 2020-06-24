using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Regular.Services;

namespace Regular.ViewModel
{
    public class RegularUpdater : IUpdater
    {
        private readonly UpdaterId updaterId = null;

        public RegularUpdater(AddInId id)
        {
            updaterId = new UpdaterId(id, Guid.NewGuid());
        }

        public void Execute(UpdaterData data)
        {
            Document document = data.GetDocument();
            string documentGuid = DocumentServices.GetRevitDocumentGuid(document);
            TaskDialog.Show("Test", "DMU Executing");
            List<ElementId> modifiedElementIds = data.GetModifiedElementIds().ToList();
        }
        public string GetAdditionalInformation() { return "Regular: Reads any Regex Rules in the open document"; }
        public ChangePriority GetChangePriority() { return ChangePriority.Annotations; }
        public UpdaterId GetUpdaterId() { return updaterId; }
        public string GetUpdaterName() { return "Regular Updater"; }
    }
}
