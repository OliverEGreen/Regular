using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using Regular.Commands;
using Regular.Enums;
using Regular.Models;
using Regular.Services;

namespace Regular.ViewModels
{
    public class RuleEditorViewModel : INotifyPropertyChanged
    {
        // We need the ability to validate what a user is doing before making any changes back to the model.
        // Ideally, we would not have anything confirmed until they pressed OK, whereupon all validation occurs.
        // So for that to work, we would need to have a 'sketch copy' of the RegexRule to work on.
        // We can use ICommands to validate whether edits proposed to the 'sketch copy' are valid.
        // Upon pressing OK, we can update the existing rule (or create it from scratch if we're making a new one).
        // We need an unsaved 'sketch copy' to work with if editing an existing rule - our "Staging Rule".

        // The original rule object, used if updating the original rule using staging rule values
        public RegexRule InputRule { get; set; }
        // Our staging rule, either a new rule or a copy of an existing rule
        public RegexRule StagingRule { get; set; }
        public string DocumentGuid { get; set; }

        // Saving the original rule's GUID if editing an existing rule
        public bool EditingExistingRule { get; set; } = false;

        // ICommands
        public AddRulePartCommand AddRulePartCommand { get; }
        public DeleteRulePartCommand DeleteRulePartCommand { get; }
        public SubmitRuleCommand SubmitRuleCommand { get; }
        public MoveRulePartUpCommand MoveRulePartUpCommand { get; }
        public MoveRulePartDownCommand MoveRulePartDownCommand { get; }
        public GenerateCompliantExampleCommand GenerateCompliantExampleCommand { get; }
        public TriggerCategoryPanelCommand TriggerCategoryPanelCommand { get; }
        public SelectAllCategoriesCommand SelectAllCategoriesCommand { get; }
        public SelectNoneCategoriesCommand SelectNoneCategoriesCommand { get; }

        // Control-based properties
        private RuleType selectedRegexRulePartType;
        public RuleType SelectedRegexRulePartType
        {
            get => selectedRegexRulePartType;
            set
            {
                selectedRegexRulePartType = value;
                MessageBox.Show("Changed!");
                NotifyPropertyChanged("SelectedRegexRulePartType");
            }
        }
        public Dictionary<string, RuleType> RulesTypeDict { get; }
        public IEnumerable<RuleType> RuleTypes { get; set; } = Enum.GetValues(typeof(RuleType)).Cast<RuleType>();
        public IEnumerable<MatchType> MatchTypes { get; set; } = Enum.GetValues(typeof(MatchType)).Cast<MatchType>();
        public ObservableCollection<ParameterObject> PossibleTrackingParameters { get; set; } = new ObservableCollection<ParameterObject>();
        public ParameterObject TrackingParameter { get; set; }

        // View-based properties 
        public string Title { get; set; }

        private string compliantExample;
        public string CompliantExample
        {
            get => compliantExample;
            set
            {
                compliantExample = value;
                NotifyPropertyChanged("CompliantExample");
            }
        }

        private Visibility compliantExampleVisibility;
        public Visibility CompliantExampleVisibility
        {
            get => compliantExampleVisibility;
            set
            {
                compliantExampleVisibility = value;
                NotifyPropertyChanged("CompliantExampleVisibility");
            }
        }
        public string UserFeedbackText { get; set; } = "";
        public int NumberCategoriesSelected { get; set; } = 0;
        public bool OutputParameterNameInputEnabled { get; set; } = true;
        public bool CategoriesPanelExpanded { get; set; } = false;
        public string CategoriesPanelButtonText { get; set; } = "Show Categories";
        public GridLength ColumnCategoriesPanelWidth { get; set; } = new GridLength(0);
        public GridLength ColumnMarginWidth { get; set; } = new GridLength(0);
        public int WindowMinWidth { get; set; } = 436;
        public int WindowMaxWidth { get; set; } = 436;

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        // The two-argument constructor is for editing an existing rule
        // We need a reference to the original rule ID, and to create a 
        // staging rule for the user to play around with until submission
        public RuleEditorViewModel(string documentGuid, RegexRule inputRule = null)
        {
            DocumentGuid = documentGuid;
            if (inputRule != null) InputRule = inputRule;

            // If there's no rule provided, we start from scratch
            // Otherwise we create a staging rule to work with until submission / validation
            StagingRule = inputRule == null ? RegexRule.Create(documentGuid) : RegexRule.Duplicate(DocumentGuid, InputRule);

            AddRulePartCommand = new AddRulePartCommand(this);
            DeleteRulePartCommand = new DeleteRulePartCommand(this);
            SubmitRuleCommand = new SubmitRuleCommand(this);
            MoveRulePartUpCommand = new MoveRulePartUpCommand(this);
            MoveRulePartDownCommand = new MoveRulePartDownCommand(this);
            GenerateCompliantExampleCommand = new GenerateCompliantExampleCommand(this);
            TriggerCategoryPanelCommand = new TriggerCategoryPanelCommand(this);
            SelectAllCategoriesCommand = new SelectAllCategoriesCommand(this);
            SelectNoneCategoriesCommand = new SelectNoneCategoriesCommand(this);
            CategoriesPanelButtonText = "Show Categories";

            RulesTypeDict = new Dictionary<string, RuleType>
            {
                {"Any Alphanumeric", RuleType.AnyCharacter},
                {"Any Digit", RuleType.AnyDigit},
                {"Any Letter", RuleType.AnyLetter},
                {"Free Text", RuleType.FreeText},
                {"Selection Set", RuleType.SelectionSet}
            };

            if (InputRule == null) return;
            void LoadExistingRule()
            {
                EditingExistingRule = true;
                Title = EditingExistingRule ? $"Editing Rule: {StagingRule.RuleName}" : "Creating New Rule";
                OutputParameterNameInputEnabled = !EditingExistingRule;

                // Retrieving the list of parameters which might possibly be tracked, given the selected categories
                PossibleTrackingParameters = ParameterServices.GetParametersOfCategories(DocumentGuid, StagingRule.TargetCategoryObjects);

                // Selecting the previously-saved tracking parameter
                TrackingParameter = PossibleTrackingParameters.FirstOrDefault(x => x.ParameterObjectId == StagingRule.TrackingParameterObject.ParameterObjectId);
            }
            
            // If we're editing an existing rule, the UI-bound properties can load the rule to display saved information
            LoadExistingRule();
        }
    }
}
