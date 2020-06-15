using System.ComponentModel;

namespace Regular.Enums
{
    public enum MatchType
    {
        [Description("Exact Match")]
        ExactMatch,
        [Description("Match Anywhere")]
        PartialMatch,
        [Description("Match Beginning")]
        MatchAtBeginning
    }
}
