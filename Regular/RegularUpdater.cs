using System;
using System.Linq;
using Autodesk.Revit.DB;
using Regular.Utilities;

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
            RuleExecutionUtils.ExecuteRegexRule(
                DocumentGuidUtils.GetDocumentGuidFromExtensibleStorage(data.GetDocument()),
                updaterId,
                data.GetModifiedElementIds().ToList());
        }
        public string GetAdditionalInformation() { return "Regular: Reads any Regex Rules in the open document"; }
        public ChangePriority GetChangePriority() { return ChangePriority.Annotations; }
        public UpdaterId GetUpdaterId() { return updaterId; }
        public string GetUpdaterName() { return "Regular Updater"; }
    }
}
