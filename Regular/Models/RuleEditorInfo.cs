using Regular.Enums;

namespace Regular.Models
{
    public class RuleEditorInfo
    {
        public string DocumentGuid { get; set; }
        public RuleEditorType RuleEditorType { get; set; }
        public RegexRule RegexRule { get; set; }
    }
}
