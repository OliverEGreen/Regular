﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Regular.Models;
using Regular.ViewModels;

namespace Regular.Views
{
    public partial class OptionSetEditor
    {
        public OptionSetEditorViewModel OptionSetEditorViewModel { get; set; }
        public IRegexRulePart RegexRulePart { get; set; }
        
        public OptionSetEditor(IRegexRulePart regexRulePart)
        {
            InitializeComponent();
            OptionSetEditorViewModel = new OptionSetEditorViewModel(regexRulePart);
            RegexRulePart = regexRulePart;
            DataContext = OptionSetEditorViewModel;
            PreviewKeyDown += (s, e) => { if (e.Key == Key.Escape) Close(); };
        }

        private void ButtonOk_Click(object sender, RoutedEventArgs e)
        {
            //TODO: Validate that something's been added, feedback if not
            List<string> options = OptionSetEditorViewModel.Options.Select(x => x.OptionValue).ToList();
            RegexRulePart.RawUserInputValue = String.Join(", ", options);
            RegexRulePart.RawUserInputTextBoxVisibility = Visibility.Visible;
            RegexRulePart.RuleTypeNameVisibility = Visibility.Collapsed;
            Close();
        }

        private void ButtonCancel_Click(object sender, RoutedEventArgs e) => Close();

        
    }
}