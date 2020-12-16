using System.ComponentModel;

namespace Regular.Enums
{
    public enum RuleType
    {
        [Description("Any Alphanumeric (A-Z, 0-9)")]
        AnyCharacter = 0,
        [Description("Any Digit (0-9)")]
        AnyDigit = 1,
        [Description("Any Letter (A-Z)")]
        AnyLetter = 2,
        [Description("Free Text")]
        FreeText = 3,
        [Description("Selection Set")]
        SelectionSet = 4
    }
}
