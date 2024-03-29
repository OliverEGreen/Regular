﻿using System.ComponentModel;

namespace Regular.Enums
{
    public enum RuleType
    {
        [Description("Any Alphanumeric (A-Z, 0-9)")]
        AnyAlphanumeric = 0,
        [Description("Any Digit (0-9)")]
        AnyDigit = 1,
        [Description("Any Letter (A-Z)")]
        AnyLetter = 2,
        [Description("Custom Text")]
        CustomText = 3,
        [Description("Option Set")]
        OptionSet = 4,
        [Description("Full Stop")]
        FullStop = 5,
        [Description("Hyphen")]
        Hyphen = 6,
        [Description("Underscore")]
        Underscore = 7,
        [Description("Open Parenthesis '('")]
        OpenParenthesis = 8,
        [Description("Close Parenthesis ')'")]
        CloseParenthesis = 9,
    }
}
