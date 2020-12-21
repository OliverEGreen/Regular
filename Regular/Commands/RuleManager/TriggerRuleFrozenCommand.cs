using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Regular.ViewModels;

namespace Regular.Commands.RuleManager
{
    public class TriggerRuleFrozenCommand
    {
        private readonly RuleManagerViewModel ruleManagerViewModel;

        public TriggerRuleFrozenCommand(RuleManagerViewModel ruleManagerViewModel)
        {
            this.ruleManagerViewModel = ruleManagerViewModel;
        }

        public bool CanExecute()
        {
            // If there's no selected item, we return
            return ruleManagerViewModel.SelectedRegexRule != null;
        }

        public void Execute()
        {
            ruleManagerViewModel.SelectedRegexRule.IsFrozen = !(ruleManagerViewModel.SelectedRegexRule.IsFrozen);
        }

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
    }
}
