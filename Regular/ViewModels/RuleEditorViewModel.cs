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
        public RuleEditorInfo RuleEditorInfo { get; }
        
        // The original rule object, used if updating the original rule using the Staging Rule's values
        public RegexRule InputRule { get; }
        
        // Our staging rule, either a new rule or a copy of an existing rule
        public RegexRule StagingRule { get; }
   

        // ICommands
        public AddRulePartCommand AddRulePartCommand { get; }
        public DeleteRulePartCommand DeleteRulePartCommand { get; }
        public EditRulePartCommand EditRulePartCommand { get; }
        public SubmitRuleCommand SubmitRuleCommand { get; }
        public MoveRulePartUpCommand MoveRulePartUpCommand { get; }
        public MoveRulePartDownCommand MoveRulePartDownCommand { get; }
        public GenerateCompliantExampleCommand GenerateCompliantExampleCommand { get; }
        public TriggerCategoryPanelCommand TriggerCategoryPanelCommand { get; }
        public TriggerSelectCategoryCommand TriggerCategoryCheckedCommand { get; }
        public TriggerSelectAllCategoriesCommand SelectAllCategoriesCommand { get; }
        public UpdateRegexStringCommand UpdateRegexStringCommand { get; }


        // Private Members & Settings Default Values
        private string titlePrefix = "";
        private string title = "";
        private string userFeedbackText = "";
        private Visibility userFeedbackTextVisibility = Visibility.Hidden;
        private bool ruleNameInputDirty = false;
        private int numberCategoriesSelected = 0;
        private ObservableCollection<ParameterObject> possibleTrackingParameterObjects = new ObservableCollection<ParameterObject>();
        private string comboBoxTrackingParameterText = "Select Categories";
        private bool outputParameterNameInputDirty = false;
        private string categoriesPanelButtonText = "Show Categories";
        private GridLength columnCategoriesPanelWidth = new GridLength(0);
        private GridLength columnMarginWidth = new GridLength(0);
        private IRegexRulePart selectedRegexRulePart = null;
        private MatchType selectedMatchType = MatchType.ExactMatch;
        private RuleType selectedRuleType = RuleType.AnyAlphanumeric;
        private string compliantExample = "";
        private Visibility compliantExampleVisibility = Visibility.Collapsed;
        private int buttonsColumnNumber = 1;
        private int windowMinWidth = 436;
        private int windowMaxWidth = 436;
        

        // Public Properties & Setters
        public string TitlePrefix
        {
            get => titlePrefix;
            set
            {
                titlePrefix = value;
                NotifyPropertyChanged();
            }
        }
        public string Title
        {
            get => title;
            set
            {
                title = value;
                NotifyPropertyChanged();
            }
        }
        public string UserFeedbackText
        {
            get => userFeedbackText;
            set
            {
                userFeedbackText = value;
                NotifyPropertyChanged();
            }
        }
        public Visibility UserFeedbackTextVisibility
        {
            get => userFeedbackTextVisibility;
            set
            {
                userFeedbackTextVisibility = value;
                NotifyPropertyChanged();
            }
        }
        public int NumberCategoriesSelected
        {
            get => numberCategoriesSelected;
            set
            {
                numberCategoriesSelected = value;
                NotifyPropertyChanged();
            }
        }
        public ObservableCollection<ParameterObject> PossibleTrackingParameterObjects
        {
            get => possibleTrackingParameterObjects;
            set
            {
                possibleTrackingParameterObjects = value;
                NotifyPropertyChanged();
            }
        }
        public string ComboBoxTrackingParameterText
        {
            get => comboBoxTrackingParameterText;
            set
            {
                comboBoxTrackingParameterText = value;
                NotifyPropertyChanged();
            }
        }
        public bool OutputParameterNameInputDirty
        {
            get => outputParameterNameInputDirty;
            set
            {
                outputParameterNameInputDirty = value;
                NotifyPropertyChanged();
            }
        }
        public string CategoriesPanelButtonText
        {
            get => categoriesPanelButtonText;
            set
            {
                categoriesPanelButtonText = value;
                NotifyPropertyChanged();
            }
        }
        public bool RuleNameInputDirty
        {
            get => ruleNameInputDirty;
            set
            {
                ruleNameInputDirty = value;
                NotifyPropertyChanged();
            }
        }
        public GridLength ColumnCategoriesPanelWidth
        {
            get => columnCategoriesPanelWidth;
            set
            {
                columnCategoriesPanelWidth = value;
                NotifyPropertyChanged();
            }
        }
        public GridLength ColumnMarginWidth
        {
            get => columnMarginWidth;
            set
            {
                columnMarginWidth = value;
                NotifyPropertyChanged();
            }
        }
        public IRegexRulePart SelectedRegexRulePart
        {
            get => selectedRegexRulePart;
            set
            {
                selectedRegexRulePart = value;
                NotifyPropertyChanged();
            }
        }
        public MatchType SelectedMatchType
        {
            get => selectedMatchType;
            set
            {
                selectedMatchType = value;
                StagingRule.MatchType = value;
                UpdateRegexStringCommand.Execute(null);
                NotifyPropertyChanged();
            }
        }
        public RuleType SelectedRuleType
        {
            get => selectedRuleType;
            set
            {
                selectedRuleType = value;
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
        public Visibility CompliantExampleVisibility
        {
            get => compliantExampleVisibility;
            set
            {
                compliantExampleVisibility = value;
                NotifyPropertyChanged();
            }
        }
        public int ButtonsColumnNumber
        {
            get => buttonsColumnNumber;
            set
            {
                buttonsColumnNumber = value;
                NotifyPropertyChanged();
            }
        }
        public int WindowMinWidth
        {
            get => windowMinWidth;
            set
            {
                windowMinWidth = value;
                NotifyPropertyChanged();
            }
        }
        public int WindowMaxWidth
        {
            get => windowMaxWidth;
            set
            {
                windowMaxWidth = value;
                NotifyPropertyChanged();
            }
        }


        // Auto Properties
        public bool OutputParameterNameInputEnabled { get; set; } = true;
        public bool CategoriesPanelExpanded { get; set; } = false;
        
        public Dictionary<string, RuleType> RulesTypeDict { get; } = EnumDicts.RulesTypeDict;
        public Dictionary<string, MatchType> MatchTypesDict { get; } = EnumDicts.MatchTypesDict;

        public void UpdateCheckedCategoriesCount() => NumberCategoriesSelected = StagingRule.TargetCategoryObjects.Count(x => x.IsChecked);
        

        public RuleEditorViewModel(RuleEditorInfo ruleEditorInfo)
        {
            RuleEditorInfo = ruleEditorInfo;
            
            if (RuleEditorInfo.RegexRule != null) InputRule = RuleEditorInfo.RegexRule;

            AddRulePartCommand = new AddRulePartCommand(this);
            DeleteRulePartCommand = new DeleteRulePartCommand(this);
            EditRulePartCommand = new EditRulePartCommand(this);
            SubmitRuleCommand = new SubmitRuleCommand(this);
            MoveRulePartUpCommand = new MoveRulePartUpCommand(this);
            MoveRulePartDownCommand = new MoveRulePartDownCommand(this);
            GenerateCompliantExampleCommand = new GenerateCompliantExampleCommand(this);
            TriggerCategoryPanelCommand = new TriggerCategoryPanelCommand(this);
            SelectAllCategoriesCommand = new TriggerSelectAllCategoriesCommand(this);
            TriggerCategoryCheckedCommand = new TriggerSelectCategoryCommand(this);
            UpdateRegexStringCommand = new UpdateRegexStringCommand(this);
            
            switch (RuleEditorInfo.RuleEditorType)
            {
                case RuleEditorType.CreateNewRule:
                    TitlePrefix = "New Rule";
                    StagingRule = RegexRule.Create(RuleEditorInfo.DocumentGuid);
                    StagingRule.TargetCategoryObjects = CategoryUtils.GetInitialCategories(RuleEditorInfo.DocumentGuid);
                    break;
                case RuleEditorType.EditingExistingRule:
                    TitlePrefix = "Editing Rule";
                    StagingRule = RegexRule.Duplicate(RuleEditorInfo.DocumentGuid, InputRule, true);
                    OutputParameterNameInputEnabled = false;
                    SelectedMatchType = InputRule.MatchType;
                    break;
                case RuleEditorType.DuplicateExistingRule:
                    TitlePrefix = "New Rule";
                    StagingRule = RegexRule.Duplicate(RuleEditorInfo.DocumentGuid, InputRule, true);
                    OutputParameterNameInputEnabled = true;
                    SelectedMatchType = InputRule.MatchType;
                    break;
            }
            
            UpdateCheckedCategoriesCount();
            
            Title = $"{TitlePrefix}: {StagingRule.RuleName}";
            // Retrieving the list of parameters which might possibly be tracked, given the selected categories
            PossibleTrackingParameterObjects = new ObservableCollection<ParameterObject>
            (
                ParameterUtils.GetParametersOfCategories(RuleEditorInfo.DocumentGuid, StagingRule.TargetCategoryObjects)
            );
            // The compliant example should always update
            GenerateCompliantExampleCommand.Execute(null);
        }
    }
}
