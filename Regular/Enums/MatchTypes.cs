using System.ComponentModel;

namespace Regular.Enums
{
    public enum MatchTypes
    {
        [Description("Exact Match")]
        ExactMatch,
        [Description("Match Anywhere")]
        PartialMatch,
        [Description("Match Beginning")]
        MatchAtBeginning
    }
}
