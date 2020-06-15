using System.ComponentModel;

namespace Regular.Enums
{
    public enum LetterMatchType
    {
        [Description("Upper Case (A-Z)")]
        UpperCase,
        [Description("Lower Case (a-z)")]
        LowerCase,
        [Description("Any Case (A-Z a-z)")]
        AnyCase
    }
}