using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using Autodesk.Revit.UI;
using Regular.Utilities;
using Regular.ViewModels;

namespace Regular.Commands.RuleEditor
{
    public class GenerateCompliantExampleCommand: ICommand
    {
        private readonly RuleEditorViewModel ruleEditorViewModel;

        public GenerateCompliantExampleCommand(RuleEditorViewModel ruleEditorViewModel)
        {
            this.ruleEditorViewModel = ruleEditorViewModel;
        }
        public bool CanExecute(object parameter)
        {
            return ruleEditorViewModel.StagingRule.RegexRuleParts.Count >= 0;
        }

        public void Execute(object parameter)
        {
            ruleEditorViewModel.CompliantExampleVisibility = Visibility.Visible;
            ruleEditorViewModel.CompliantExample = RegexAssemblyUtils.GenerateRandomExample(ruleEditorViewModel.StagingRule.RegexRuleParts);

            ruleEditorViewModel.UpdateRegexStringCommand.Execute(null);
            Regex regex = new Regex(ruleEditorViewModel.StagingRule.RegexString);
            if (regex.IsMatch(ruleEditorViewModel.CompliantExample) == false)
            {
                TaskDialog.Show("Regex Mismatch",
                    $"Compliant example {ruleEditorViewModel.CompliantExample} does not match regular expression {ruleEditorViewModel.StagingRule.RegexString}");
            }
        }

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
    }
}
