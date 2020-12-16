using System.Collections.Generic;

namespace Regular.Enums
{
    public class RuleTypesDict
    {
        public Dictionary<string, RuleType> RulesTypeDict { get; }

        public RuleTypesDict()
        {
            RulesTypeDict = new Dictionary<string, RuleType>
            {
                {"Any Alphanumeric (A-Z, 0-9)", RuleType.AnyCharacter},
                {"Any Digit (0-9)", RuleType.AnyDigit},
                {"Any Letter (A-Z)", RuleType.AnyLetter},
                {"Free Text", RuleType.FreeText},
                {"Selection Set", RuleType.SelectionSet}
            };
        }
    }
}
