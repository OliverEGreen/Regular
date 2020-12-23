using System;
using System.Windows.Input;
using Regular.Enums;
using Regular.Models;
using Regular.Utilities;

namespace Regular.Commands.RuleEditor
{
    public class EditRulePartCommand: ICommand
    {
        public bool CanExecute(object parameter)
        {
            if (!(parameter is IRegexRulePart regexRulePart)) return false;
            return regexRulePart.IsButtonControlEnabled;
        }

        public void Execute(object parameter)
        {
            if (!(parameter is IRegexRulePart regexRulePart)) return;
            switch (regexRulePart.RuleType)
            {
                case RuleType.AnyAlphanumeric:
                    switch (regexRulePart.CaseSensitivityMode)
                    {
                        case CaseSensitivity.AnyCase:
                            regexRulePart.CaseSensitivityMode = CaseSensitivity.UpperCase;
                            regexRulePart.CaseSensitiveDisplayString =
                                EnumUtils.GetEnumDescription(regexRulePart.CaseSensitivityMode);
                            regexRulePart.ButtonControlDisplayText = "AB3";
                            break;
                        case CaseSensitivity.UpperCase:
                            regexRulePart.CaseSensitivityMode = CaseSensitivity.LowerCase;
                            regexRulePart.CaseSensitiveDisplayString =
                                EnumUtils.GetEnumDescription(regexRulePart.CaseSensitivityMode);
                            regexRulePart.ButtonControlDisplayText = "ab3";
                            break;
                        case CaseSensitivity.LowerCase:
                            regexRulePart.CaseSensitivityMode = CaseSensitivity.AnyCase;
                            regexRulePart.CaseSensitiveDisplayString =
                                EnumUtils.GetEnumDescription(regexRulePart.CaseSensitivityMode);
                            regexRulePart.ButtonControlDisplayText= "Ab3";
                            break;
                        case CaseSensitivity.None:
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                    break;
                case RuleType.AnyDigit:
                    break;
                case RuleType.AnyLetter:
                    switch (regexRulePart.CaseSensitivityMode)
                    {
                        case CaseSensitivity.AnyCase:
                            regexRulePart.CaseSensitivityMode = CaseSensitivity.UpperCase;
                            regexRulePart.CaseSensitiveDisplayString =
                                EnumUtils.GetEnumDescription(regexRulePart.CaseSensitivityMode);
                            regexRulePart.ButtonControlDisplayText = "A-Z";
                            break;
                        case CaseSensitivity.UpperCase:
                            regexRulePart.CaseSensitivityMode = CaseSensitivity.LowerCase;
                            regexRulePart.CaseSensitiveDisplayString =
                                EnumUtils.GetEnumDescription(regexRulePart.CaseSensitivityMode);
                            regexRulePart.ButtonControlDisplayText = "a-z";
                            break;
                        case CaseSensitivity.LowerCase:
                            regexRulePart.CaseSensitivityMode = CaseSensitivity.AnyCase;
                            regexRulePart.CaseSensitiveDisplayString =
                                EnumUtils.GetEnumDescription(regexRulePart.CaseSensitivityMode);
                            regexRulePart.ButtonControlDisplayText = "A-z";
                            break;
                        case CaseSensitivity.None:
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                    break;
                case RuleType.FreeText:
                    break;
                case RuleType.SelectionSet:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
    }
}