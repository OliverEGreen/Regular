using System;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Regular.ViewModel
{
    public class RegexRule : INotifyPropertyChanged
    {
        private string name;
        private ObservableCollection<ObservableObject> targetCategoryIds;
        private ObservableCollection<RegexRulePart> regexRuleParts;
        private string toolTipString;

        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                NotifyPropertyChanged("Name");
            }
        }
        public string Guid { get; set; }
        public ObservableCollection<ObservableObject> TargetCategoryIds
        {
            get { return targetCategoryIds; }
            set
            {
                targetCategoryIds = value;
                NotifyPropertyChanged("TargetCategoryIds");
            }
        }
        public string TrackingParameterName { get; set; }
        public string OutputParameterName { get; set; }
        public string ToolTip
        {
            get
            {
                return $"Rule Name: {Name}" + Environment.NewLine +
                            $"Applies To: {String.Join(", ", TargetCategoryIds)}" + Environment.NewLine +
                            $"Created By: {Environment.UserName}" + Environment.NewLine +
                            $"Created At: {DateTime.Now.ToString("r")}" + Environment.NewLine +
                            $"Regex String: {RegexString}";
            }
            set
            {
                toolTipString = value;
                NotifyPropertyChanged("ToolTip");
                NotifyPropertyChanged("Name");
                NotifyPropertyChanged("TargetCategoryIds");
                NotifyPropertyChanged("RegexString");
            }
        }
        public string RegexString { get; set; }
        public ObservableCollection<RegexRulePart> RegexRuleParts
        {
            get { return regexRuleParts; }
            set
            {
                regexRuleParts = value;
                NotifyPropertyChanged("RegexRuleParts");
            }
        }

        // Constructor
        public RegexRule()
        {
            Guid = System.Guid.NewGuid().ToString();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
