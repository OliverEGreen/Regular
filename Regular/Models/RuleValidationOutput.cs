using System.ComponentModel;
using System.Runtime.CompilerServices;
using Autodesk.Revit.DB;
using Regular.Enums;
using Regular.Utilities;

namespace Regular.Models
{
    public class RuleValidationOutput : INotifyPropertyChanged
    {
        // Private Members & Defaults
        private string validationText = RuleValidationResult.Invalid.GetEnumDescription();
        private string compliantExample = "";
        private string trackingParameterValue = "";
        private RuleValidationResult ruleValidationResult = RuleValidationResult.Invalid;
        private string elementName = "";

        // Public Members & NotifyPropertyChanged
        public ElementId ElementId { get; } = ElementId.InvalidElementId;

        public string ElementName
        {
            get => elementName;
            set
            {
                elementName = value;
                NotifyPropertyChanged();
            }
        }
        public string ValidationText
        {
            get => validationText;
            set
            {
                validationText = value;
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
        public RuleValidationResult RuleValidationResult
        {
            get => ruleValidationResult;
            set
            {
                ruleValidationResult = value;
                validationText = ruleValidationResult.GetEnumDescription();
                NotifyPropertyChanged();
            }
        }


        public RuleValidationOutput(RuleValidationInfo ruleValidationInfo)
        {
            ElementId = ruleValidationInfo.Element.Id;
            elementName = ruleValidationInfo.Element.Name;

            RuleValidationResult = RuleExecutionUtils.ExecuteRegexRule
            (
                ruleValidationInfo.DocumentGuid,
                ruleValidationInfo.RegexRule.RuleGuid,
                ruleValidationInfo.Element
            );
            
            ValidationText = RuleValidationResult.GetEnumDescription();
            CompliantExample = ruleValidationResult == RuleValidationResult.Invalid ? RegexAssemblyUtils.GenerateRandomExample(ruleValidationInfo.RegexRule.RegexRuleParts) : "-";
            
            TrackingParameterValue = ParameterUtils.GetTrackingParameterValue
            (
                ruleValidationInfo.DocumentGuid,
                ruleValidationInfo.RegexRule.RuleGuid,
                ruleValidationInfo.Element
            );
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void NotifyPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
