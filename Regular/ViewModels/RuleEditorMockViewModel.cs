using System.Collections.ObjectModel;
using System.Windows;
using Regular.Commands;
using Regular.DesignData;
using Regular.Enums;
using Regular.Models;

namespace Regular.ViewModels
{
    public class RuleEditorMockViewModel
    {
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

        public string CategoriesPanelButtonText { get; set; }
        public string RuleName { get; set; }
        public GridLength ColumnCategoriesPanelWidth { get; set; }
        public GridLength ColumnMarginWidth { get; set; }
        public int WindowMinWidth { get; set; } = 436;
        public int WindowMaxWidth { get; set; }= 436;
        public int NumberCategoriesSelected { get; set; }
        public string CompliantExample { get; set; }
        public Visibility CompliantExampleVisibility { get; set; }
        public ObservableCollection<ParameterObject> TrackingParameterObjects { get; set; }
        public ParameterObject OutputParameterObject { get; set; }
        public ObservableCollection<RegexRulePart> RegexRuleParts { get; set; }
        public ObservableCollection<CategoryObject> TargetCategoryObjects { get; set; }

        public RuleEditorMockViewModel()
        {
            RuleName = "ISO 19650 Rule";
            CategoriesPanelButtonText = "Show Categories";
            ColumnCategoriesPanelWidth = new GridLength(0);
            ColumnMarginWidth = new GridLength(0);
            NumberCategoriesSelected = 12;
            CompliantExample = "AHMM-XX-A-XX-00001";
            CompliantExampleVisibility = Visibility.Visible;
            TrackingParameterObjects = new ObservableCollection<ParameterObject>()
            {
                new ParameterObject {ParameterObjectId = 12345, ParameterObjectName = "Tracking Parameter"},
                new ParameterObject {ParameterObjectId = 12345, ParameterObjectName = "Other Parameter"}
            };
            
            OutputParameterObject = new ParameterObject()
            {
                ParameterObjectId = 12345, ParameterObjectName = "Some Parameter Object"
            };
            RegexRuleParts = new ObservableCollection<RegexRulePart>()
            {
                new RegexRulePart(RuleType.AnyCharacter, true, true, true)
                {
                    CaseSensitiveDisplayString = "doijd",
                    DisplayText = "eworijwr",
                    EditButtonDisplayText = "Edit",
                    IsCaseSensitive = true,
                    IsCaseSensitiveCheckboxVisible = true,
                    IsEditable = true,
                    IsOptional = true,
                    RawUserInputValue = "dsefs"
                },
                new RegexRulePart(RuleType.AnyCharacter, true, true, true)
                {
                    CaseSensitiveDisplayString = "doijd",
                    DisplayText = "eworijwr",
                    EditButtonDisplayText = "Edit",
                    IsCaseSensitive = true,
                    IsCaseSensitiveCheckboxVisible = true,
                    IsEditable = true,
                    IsOptional = true,
                    RawUserInputValue = "dsefs"
                }
            };
            
            TargetCategoryObjects = new ObservableCollection<CategoryObject>()
            {
                new CategoryObject {IsChecked = true, CategoryObjectId = 12345, CategoryObjectName = "TestCategory"},
                new CategoryObject {IsChecked = false, CategoryObjectId = 54321, CategoryObjectName = "AnotherCategory"}
            };
        }
    }
}
