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
using Visibility = System.Windows.Visibility;

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
        
        // The document we're editing rules within
        public string DocumentGuid { get; set; }
        
        // The original rule object, used if updating the original rule using staging rule values
        public RegexRule InputRule { get; set; }
        // Our staging rule, either a new rule or a copy of an existing rule
        public RegexRule StagingRule { get; set; }
        // Saving the original rule's GUID if editing an existing rule
        public bool EditingExistingRule { get; set; }

        // ICommands
        public AddRulePartCommand AddRulePartCommand { get; }
        public DeleteRulePartCommand DeleteRulePartCommand { get; }
        public EditRulePartCommand EditRulePartCommand { get; }
        public SubmitRuleCommand SubmitRuleCommand { get; }
        public MoveRulePartUpCommand MoveRulePartUpCommand { get; }
        public MoveRulePartDownCommand MoveRulePartDownCommand { get; }
        public GenerateCompliantExampleCommand GenerateCompliantExampleCommand { get; }
        public TriggerCategoryPanelCommand TriggerCategoryPanelCommand { get; }
        public SelectAllCategoriesCommand SelectAllCategoriesCommand { get; }
        public SelectNoneCategoriesCommand SelectNoneCategoriesCommand { get; }
        public TriggerCategoryCheckedCommand TriggerCategoryCheckedCommand { get; }

        // Control-based properties
        private RuleType selectedRuleType;
        public RuleType SelectedRuleType
        {
            get => selectedRuleType;
            set
            {
                selectedRuleType = value;
                NotifyPropertyChanged("SelectedRuleType");
            }
        }

        private MatchType selectedMatchType;
        public MatchType SelectedMatchType
        {
            get => selectedMatchType;
            set
            {
                selectedMatchType = value;
                StagingRule.MatchType = value;
                NotifyPropertyChanged("SelectedMatchType");
            }
        }

        private IRegexRulePart selectedRegexRulePart;
        public IRegexRulePart SelectedRegexRulePart
        {
            get => selectedRegexRulePart;
            set
            {
                selectedRegexRulePart = value;
                NotifyPropertyChanged("SelectedRegexRulePart");
            }
        }
        public Dictionary<string, RuleType> RulesTypeDict { get; }
        public Dictionary<string, MatchType> MatchTypesDict { get; }

        private ObservableCollection<ParameterObject> possibleTrackingParameterObjects;
        public ObservableCollection<ParameterObject> PossibleTrackingParameterObjects
        {
            get => possibleTrackingParameterObjects;
            set
            {
                possibleTrackingParameterObjects = value;
                NotifyPropertyChanged("PossibleTrackingParameterObjects");
            }
        }

        private ParameterObject selectedTrackingParameterObject;

        public ParameterObject SelectedTrackingParameterObject
        {
            get => selectedTrackingParameterObject;
            set
            {
                selectedTrackingParameterObject = value;
                NotifyPropertyChanged("SelectedTrackingParameterObject");
            }
        }
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

        private Visibility userFeedbackTextVisibility;

        public Visibility UserFeedbackTextVisibility
        {
            get => userFeedbackTextVisibility;
            set
            {
                userFeedbackTextVisibility = value;
                NotifyPropertyChanged("UserFeedbackTextVisibility");
            }
        }

        private string userFeedbackText;
        public string UserFeedbackText
        {
            get => userFeedbackText;
            set
            {
                userFeedbackText = value;
                NotifyPropertyChanged("UserFeedbackText");
            }
        }
        private int numberCategoriesSelected;
        public int NumberCategoriesSelected
        {
            get => numberCategoriesSelected;
            set
            {
                numberCategoriesSelected = value;
                NotifyPropertyChanged("NumberCategoriesSelected");
            }
        }

        private string comboBoxTrackingParameterText;
        public string ComboBoxTrackingParameterText
        {
            get => comboBoxTrackingParameterText;
            set
            {
                comboBoxTrackingParameterText = value;
                NotifyPropertyChanged("ComboBoxTrackingParameterText");
            }
        }
        public bool OutputParameterNameInputEnabled { get; set; } = true;
        public bool CategoriesPanelExpanded { get; set; } = false;
        private bool ruleNameInputDirty;

        public bool RuleNameInputDirty
        {
            get => ruleNameInputDirty;
            set
            {
                ruleNameInputDirty = value;
                NotifyPropertyChanged("RuleNameInputDirty");
            }
        }
        private bool outputParameterNameInputDirty;
        public bool OutputParameterNameInputDirty
        {
            get => outputParameterNameInputDirty;
            set
            {
                outputParameterNameInputDirty = value;
                NotifyPropertyChanged("OutputParameterNameInputDirty");
            }
        }
        private string categoriesPanelButtonText;
        public string CategoriesPanelButtonText
        {
            get => categoriesPanelButtonText;
            set
            {
                categoriesPanelButtonText = value;
                NotifyPropertyChanged("CategoriesPanelButtonText");
            }
        }
        private GridLength columnCategoriesPanelWidth;
        public GridLength ColumnCategoriesPanelWidth
        {
            get => columnCategoriesPanelWidth;
            set
            {
                columnCategoriesPanelWidth = value;
                NotifyPropertyChanged("ColumnCategoriesPanelWidth");
            }
        }
        private GridLength columnMarginWidth;
        public GridLength ColumnMarginWidth
        {
            get => columnMarginWidth;
            set
            {
                columnMarginWidth = value;
                NotifyPropertyChanged("ColumnMarginWidth");
            }
        }

        private int windowMinWidth;
        public int WindowMinWidth
        {
            get => windowMinWidth;
            set
            {
                windowMinWidth = value;
                NotifyPropertyChanged("WindowMinWidth");
            }
        }

        private int windowMaxWidth;
        public int WindowMaxWidth
        {
            get => windowMaxWidth;
            set
            {
                windowMaxWidth = value;
                NotifyPropertyChanged("WindowMaxWidth");
            }
        }

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
            EditRulePartCommand = new EditRulePartCommand();
            SubmitRuleCommand = new SubmitRuleCommand(this);
            MoveRulePartUpCommand = new MoveRulePartUpCommand(this);
            MoveRulePartDownCommand = new MoveRulePartDownCommand(this);
            GenerateCompliantExampleCommand = new GenerateCompliantExampleCommand(this);
            TriggerCategoryPanelCommand = new TriggerCategoryPanelCommand(this);
            SelectAllCategoriesCommand = new SelectAllCategoriesCommand(this);
            SelectNoneCategoriesCommand = new SelectNoneCategoriesCommand(this);
            TriggerCategoryCheckedCommand = new TriggerCategoryCheckedCommand(this);

            CategoriesPanelButtonText = "Show Categories";
            ComboBoxTrackingParameterText = "Select Categories";
            UserFeedbackTextVisibility = Visibility.Hidden;

            // Retrieving the list of parameters which might possibly be tracked, given the selected categories
            PossibleTrackingParameterObjects = ParameterServices.GetParametersOfCategories(DocumentGuid, StagingRule.TargetCategoryObjects);

            WindowMinWidth = 436;
            WindowMaxWidth = 436;
            ColumnCategoriesPanelWidth = new GridLength(0);
            ColumnMarginWidth = new GridLength(0);

            RuleNameInputDirty = false;
            OutputParameterNameInputDirty = false;

            RulesTypeDict = new Dictionary<string, RuleType>
            {
                {"Any Alphanumeric", RuleType.AnyAlphanumeric},
                {"Any Digit", RuleType.AnyDigit},
                {"Any Letter", RuleType.AnyLetter},
                {"Free Text", RuleType.FreeText},
                {"Selection Set", RuleType.SelectionSet}
            };

            MatchTypesDict = new Dictionary<string, MatchType>
            {
                {"Exact Match", MatchType.ExactMatch},
                {"Match At Beginning", MatchType.MatchAtBeginning},
                {"Match At End", MatchType.MatchAtEnd},
                {"Partial Match", MatchType.PartialMatch}
            };
            
            if (InputRule == null) return;
            void LoadExistingRule()
            {
                EditingExistingRule = true;
                Title = EditingExistingRule ? $"Editing Rule: {StagingRule.RuleName}" : "Creating New Rule";
                OutputParameterNameInputEnabled = !EditingExistingRule;
                
                // Selecting the previously-saved tracking parameter
                TrackingParameter = PossibleTrackingParameterObjects.FirstOrDefault(x => x.ParameterObjectId == StagingRule.TrackingParameterObject.ParameterObjectId);
            }
            
            // If we're editing an existing rule, the UI-bound properties can load the rule to display saved information
            LoadExistingRule();
        }
    }
}
