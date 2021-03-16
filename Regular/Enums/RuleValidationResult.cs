using System.ComponentModel;

namespace Regular.Enums
{
    public enum RuleValidationResult
    {
        [Description("N/A")]
        NotApplicable,
        [Description("Valid")]
        Valid,
        [Description("Invalid")]
        Invalid,
    }
}
