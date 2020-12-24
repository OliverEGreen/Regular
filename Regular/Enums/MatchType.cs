using System.ComponentModel;

namespace Regular.Enums
{
    public enum MatchType
    {
        [Description("Exact Match")]
        ExactMatch,
        [Description("Match Anywhere")]
        MatchAnywhere,
        [Description("Match Beginning")]
        MatchAtBeginning,
        [Description("Match Ending")]
        MatchAtEnd
    }
}
