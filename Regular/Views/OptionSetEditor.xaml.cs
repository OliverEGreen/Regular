using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using Regular.Models;
using Regular.ViewModels;

namespace Regular.Views
{
    public partial class OptionSetEditor : Window, INotifyPropertyChanged
    {
        public IRegexRulePart RegexRulePart { get; set; }
        private ObservableCollection<string> options;
        public ObservableCollection<string> Options
        {
            get => options;
            set
            {
                options = value;
                NotifyPropertyChanged("Options");
            }
        }

        public OptionSetEditor(IRegexRulePart regexRulePart)
        {
            InitializeComponent();
            DataContext = this;
            RegexRulePart = regexRulePart;
            Options = regexRulePart.Options;

            Options.Add("Blah");
            Options.Add("bluh");
            Options.Add("bleh");
        }

        private void ButtonOk_Click(object sender, RoutedEventArgs e)
        {
            //TODO: Validate that something's been added, feedback if not
            RegexRulePart.RawUserInputValue = String.Join(", ", RegexRulePart.Options);
            RegexRulePart.RawUserInputTextBoxVisibility = Visibility.Visible;
            RegexRulePart.RuleTypeNameVisibility = Visibility.Collapsed;
        }

        private void ButtonCancel_Click(object sender, RoutedEventArgs e) => Close();

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
