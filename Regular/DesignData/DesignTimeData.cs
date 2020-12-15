using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Regular.Enums;
using Regular.Models;

namespace Regular.DesignData
{
    public class DesignTimeData
    {
        public ObservableCollection<RegexRule> RegexRules;
        public RegexRule RegexRule { get; set; }

        public DesignTimeData()
        {
            RegexRules = new ObservableCollection<RegexRule>();
            
            RegexRule regexRule1 = RegexRule.Create("12345");
            
            
            regexRule1.TargetCategoryObjects = new ObservableCollection<CategoryObject>
            {
                new CategoryObject {IsChecked = true, CategoryObjectId = 12345, CategoryObjectName = "TestCategory"},
                new CategoryObject {IsChecked = false, CategoryObjectId = 54321, CategoryObjectName = "AnotherCategory"}
            };
            
            regexRule1.RuleName = "Test";
            regexRule1.IsFrozen = false;
            regexRule1.MatchType = MatchType.ExactMatch;
            
            regexRule1.RegexRuleParts = new ObservableCollection<RegexRulePart>
            {
                new RegexRulePart(RuleType.AnyCharacter, true, true, true),
                new RegexRulePart(RuleType.AnyLetter, true, true, true),
                new RegexRulePart(RuleType.AnyDigit, true, true, true),
                new RegexRulePart(RuleType.FreeText, true, true, true)
            };
            
            regexRule1.OutputParameterObject = new ParameterObject
            {
                ParameterObjectId = 12345,
                ParameterObjectName = "ThisParameter"
            };
            regexRule1.TrackingParameterObject = new ParameterObject
            {
                ParameterObjectId = 32789,
                ParameterObjectName = "Tracking Parameter"
            };

            RegexRules.Add(regexRule1);
            
            RegexRule = regexRule1;
        }
    }
}
