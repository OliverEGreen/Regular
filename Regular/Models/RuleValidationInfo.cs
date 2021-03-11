using Autodesk.Revit.DB;

namespace Regular.Models
{
    public class RuleValidationInfo
    {
        public string DocumentGuid { get; set; }
        public Element Element { get; set; }
        public RegexRule RegexRule { get; set; }
    }
}
