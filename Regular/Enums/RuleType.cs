using System.ComponentModel;

namespace Regular.Enums
{
    public enum RuleType
    {
        [Description("Any Character (A-Z, 0-9)")]
        AnyCharacter,
        [Description("Any Digit (0-9)")]
        AnyDigit,
        [Description("Any Letter (A-Z)")]
        AnyLetter,
        [Description("Free Text")]
        FreeText,
        [Description("Selection Set")]
        SelectionSet
    }
}
