using System.ComponentModel;
using System.Runtime.CompilerServices;
using Autodesk.Revit.DB;
using Regular.Annotations;
using Regular.Utilities;

namespace Regular.Models
{
    public class RuleValidationOutput : INotifyPropertyChanged
    {
        // Private Members & Defaults
        private bool validity = false;
        private string compliantExample = "";
        private string trackingParameterValue = "";


        // Public Members & NotifyPropertyChanged
        public ElementId ElementId { get; } = ElementId.InvalidElementId;
        public string ElementName { get; } = "";
        public bool Validity
        {
            get => validity;
            set
            {
                validity = value;
                NotifyPropertyChanged();
            }
        }
        public string CompliantExample
        {
            get => compliantExample;
            set
            {
                compliantExample = value;
                NotifyPropertyChanged();
            }
        }
        public string TrackingParameterValue
        {
            get => trackingParameterValue;
            set
            {
                trackingParameterValue = value;
                NotifyPropertyChanged();
            }
        }


        public RuleValidationOutput(RuleValidationInfo ruleValidationInfo)
        {
            ElementId = ruleValidationInfo.Element.Id;
            ElementName = ruleValidationInfo.Element.Name;

            Validity = RuleExecutionUtils.TestRuleValidity
            (
                ruleValidationInfo.DocumentGuid,
                ruleValidationInfo.RegexRule.RuleGuid,
                ruleValidationInfo.Element
            );
            
            CompliantExample = RegexAssemblyUtils.GenerateRandomExample(ruleValidationInfo.RegexRule.RegexRuleParts);
            
            TrackingParameterValue = ParameterUtils.GetTrackingParameterValue
            (
                ruleValidationInfo.DocumentGuid,
                ruleValidationInfo.RegexRule.RuleGuid,
                ruleValidationInfo.Element
            );
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void NotifyPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
