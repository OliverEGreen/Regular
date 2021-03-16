using System.Collections.ObjectModel;
using Autodesk.Revit.UI;
using Document = Autodesk.Revit.DB.Document;

namespace Regular.UI.SelectElements.Model
{
    public class SelectElementsInfo
    {
        public Document Document { get; set; }
        public UIDocument UIDocument { get; set; }
        public string UserMessage { get; set; }
        public ObservableCollection<ObservableObject> ObservableObjects { get; set; }
    }
}
