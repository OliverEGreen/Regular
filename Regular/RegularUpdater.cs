using System;
using Autodesk.Revit.DB;

namespace Regular
{
    public class RegularUpdater : IUpdater
    {
        private readonly UpdaterId updaterId;

        public RegularUpdater(AddInId id)
        {
            updaterId = new UpdaterId(id, Guid.NewGuid());
        }

        public void Execute(UpdaterData data)
        {
            //Document document = data.GetDocument();
            //string documentGuid = DocumentGuidUtils.GetDocumentGuidFromExtensibleStorage(document);
            //TaskDialog.Show("Test", "DMU Executing");
            //List<ElementId> modifiedElementIds = data.GetModifiedElementIds().ToList();
        }
        public string GetAdditionalInformation() { return "Regular: Reads any Regex Rules in the open document"; }
        public ChangePriority GetChangePriority() { return ChangePriority.Annotations; }
        public UpdaterId GetUpdaterId() { return updaterId; }
        public string GetUpdaterName() { return "Regular Updater"; }
    }
}
