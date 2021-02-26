using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using Regular.Commands.RuleEditor;
using Regular.Enums;
using Regular.Models;
using Regular.Utilities;
using Regular.Views;
using Visibility = System.Windows.Visibility;

namespace Regular.ViewModels
{
    public class RuleEditorViewModel : NotifyPropertyChangedBase
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
        public UpdateRegexStringCommand UpdateRegexStringCommand { get; }

        // Control-based properties
        private RuleType selectedRuleType;
        public RuleType SelectedRuleType
        {
            get => selectedRuleType;
            set
            {
                selectedRuleType = value;
                NotifyPropertyChanged();
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
                NotifyPropertyChanged();
            }
        }

        private IRegexRulePart selectedRegexRulePart;
        public IRegexRulePart SelectedRegexRulePart
        {
            get => selectedRegexRulePart;
            set
            {
                selectedRegexRulePart = value;
                NotifyPropertyChanged();
            }
        }
        
        public Dictionary<string, RuleType> RulesTypeDict { get; } = EnumDicts.RulesTypeDict;
        public Dictionary<string, MatchType> MatchTypesDict { get; } = EnumDicts.MatchTypesDict;

        private ObservableCollection<ParameterObject> possibleTrackingParameterObjects;
        public ObservableCollection<ParameterObject> PossibleTrackingParameterObjects
        {
            get => possibleTrackingParameterObjects;
            set
            {
                possibleTrackingParameterObjects = value;
                NotifyPropertyChanged();
            }
        }
        
        // View-based properties 
        private string title;
        public string Title
        {
            get => title;
            set
            {
                title = value;
                NotifyPropertyChanged();
            }
        }

        private string compliantExample;
        public string CompliantExample
        {
            get => compliantExample;
            set
            {
                compliantExample = value;
                NotifyPropertyChanged();
            }
        }

        private Visibility compliantExampleVisibility;
        public Visibility CompliantExampleVisibility
        {
            get => compliantExampleVisibility;
            set
            {
                compliantExampleVisibility = value;
                NotifyPropertyChanged();
            }
        }

        private Visibility userFeedbackTextVisibility = Visibility.Hidden;
        public Visibility UserFeedbackTextVisibility
        {
            get => userFeedbackTextVisibility;
            set
            {
                userFeedbackTextVisibility = value;
                NotifyPropertyChanged();
            }
        }

        private string userFeedbackText;
        public string UserFeedbackText
        {
            get => userFeedbackText;
            set
            {
                userFeedbackText = value;
                NotifyPropertyChanged();
            }
        }

        private int numberCategoriesSelected;
        public int NumberCategoriesSelected
        {
            get => numberCategoriesSelected;
            set
            {
                numberCategoriesSelected = value;
                NotifyPropertyChanged();
            }
        }

        private string comboBoxTrackingParameterText = "Select Categories";
        public string ComboBoxTrackingParameterText
        {
            get => comboBoxTrackingParameterText;
            set
            {
                comboBoxTrackingParameterText = value;
                NotifyPropertyChanged();
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
                NotifyPropertyChanged();
            }
        }
        
        private bool outputParameterNameInputDirty;
        public bool OutputParameterNameInputDirty
        {
            get => outputParameterNameInputDirty;
            set
            {
                outputParameterNameInputDirty = value;
                NotifyPropertyChanged();
            }
        }
        
        private string categoriesPanelButtonText = "Show Categories";
        public string CategoriesPanelButtonText
        {
            get => categoriesPanelButtonText;
            set
            {
                categoriesPanelButtonText = value;
                NotifyPropertyChanged();
            }
        }
        
        private GridLength columnCategoriesPanelWidth = new GridLength(0);
        public GridLength ColumnCategoriesPanelWidth
        {
            get => columnCategoriesPanelWidth;
            set
            {
                columnCategoriesPanelWidth = value;
                NotifyPropertyChanged();
            }
        }
        
        private GridLength columnMarginWidth = new GridLength(0);
        public GridLength ColumnMarginWidth
        {
            get => columnMarginWidth;
            set
            {
                columnMarginWidth = value;
                NotifyPropertyChanged();
            }
        }

        private int windowMinWidth = 436;
        public int WindowMinWidth
        {
            get => windowMinWidth;
            set
            {
                windowMinWidth = value;
                NotifyPropertyChanged();
            }
        }

        private int windowMaxWidth = 436;
        public int WindowMaxWidth
        {
            get => windowMaxWidth;
            set
            {
                windowMaxWidth = value;
                NotifyPropertyChanged();
            }
        }
        

        // The two-argument constructor is for editing an existing rule
        // We need a reference to the original rule ID, and to create a 
        // staging rule for the user to play around with until submission
        public RuleEditorViewModel(string documentGuid, RegexRule inputRule = null)
        {
            DocumentGuid = documentGuid;

            AddRulePartCommand = new AddRulePartCommand(this);
            DeleteRulePartCommand = new DeleteRulePartCommand(this);
            EditRulePartCommand = new EditRulePartCommand(this);
            SubmitRuleCommand = new SubmitRuleCommand(this);
            MoveRulePartUpCommand = new MoveRulePartUpCommand(this);
            MoveRulePartDownCommand = new MoveRulePartDownCommand(this);
            GenerateCompliantExampleCommand = new GenerateCompliantExampleCommand(this);
            TriggerCategoryPanelCommand = new TriggerCategoryPanelCommand(this);
            SelectAllCategoriesCommand = new SelectAllCategoriesCommand(this);
            SelectNoneCategoriesCommand = new SelectNoneCategoriesCommand(this);
            TriggerCategoryCheckedCommand = new TriggerCategoryCheckedCommand(this);
            UpdateRegexStringCommand = new UpdateRegexStringCommand(this);
            
            // If we're editing an existing rule
            if (inputRule != null)
            {
                EditingExistingRule = true;
                InputRule = inputRule;
                StagingRule = RegexRule.Duplicate(DocumentGuid, InputRule);
                Title = $"Editing Rule: {StagingRule.RuleName}";
                OutputParameterNameInputEnabled = false;
                NumberCategoriesSelected = StagingRule.TargetCategoryObjects.Count(x => x.IsChecked);
                GenerateCompliantExampleCommand.Execute(null);
            }
            // Otherwise we're creating a new rule from scratch
            else
            {
                StagingRule = RegexRule.Create(documentGuid);
                Title = "Creating New Rule";
            }
            
            // Retrieving the list of parameters which might possibly be tracked, given the selected categories
            PossibleTrackingParameterObjects = ParameterUtils.GetParametersOfCategories(DocumentGuid, StagingRule.TargetCategoryObjects);
        }
    }
}
