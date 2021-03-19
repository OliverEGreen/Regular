using Regular.Models;

namespace Regular.UI.ImportRule.Model
{
    public class ImportRuleInfo
    {
        public string DocumentGuid { get; set; }
        public RegexRule ExistingRegexRule { get; set; }
        public RegexRule NewRegexRule { get; set; }
    }
}
