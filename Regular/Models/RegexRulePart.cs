namespace Regular.Models
{
    public class RegexRulePart
    {
        public RuleTypes RuleType { get; }
        public string RuleTypeDisplayString { get; set; } = "Rule";
        public bool IsOptional { get; set; } = false;
        public bool IsCaseSensitive { get; set; } = false;
        public bool RequiresUserInput { get; set; } = false;
        public string RawUserInputValue { get; set; } = "";

        // Our default constructor for newly-created RegexRuleParts
        public RegexRulePart(RuleTypes ruleType)
        {
            RuleType = ruleType;
            RequiresUserInput = RuleType == RuleTypes.FreeText || RuleType == RuleTypes.SelectionSet ? true : false;
        }
        // Our detailed constructor for recreating stored RegexRuleParts that were loaded from ExtensibleStorage
        public RegexRulePart(RuleTypes ruleType, bool isOptional, bool isCaseSensitive, bool requiresUserInput, string rawUserInputValue)
        {
            RuleType = ruleType;
            IsOptional = isOptional;
            IsCaseSensitive = isCaseSensitive;
            RequiresUserInput = requiresUserInput;
            RawUserInputValue = rawUserInputValue;
        }
    }
}
