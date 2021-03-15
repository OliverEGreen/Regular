using System.ComponentModel;

namespace Regular.Enums
{
    public enum ValidationResult
    {
        [Description("N/A")]
        NotApplicable,
        [Description("Valid")]
        Valid,
        [Description("Invalid")]
        Invalid,
    }
}
