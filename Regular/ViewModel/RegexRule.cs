using System;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Regular.ViewModel
{
    public class RegexRule : INotifyPropertyChanged
    {
        private string name;
        private readonly string dateTimeCreated;
        private readonly string createdBy;
        private ObservableCollection<ObservableObject> targetCategoryIds;
        private ObservableCollection<RegexRulePart> regexRuleParts;
        private string trackingParameterName; // Eventually, this should be some kind of ID
        private string outputParameterName; // This should also be an ID
        private string toolTipString;
        private string regexString;
        
        public string Name
        {
            get => name;
            set
            {
                name = value;
                NotifyPropertyChanged("Name");
            }
        }
        public string Guid
        {
            get;
            set;
        }
        public ObservableCollection<ObservableObject> TargetCategoryIds
        {
            get => targetCategoryIds;
            set
            {
                targetCategoryIds = value;
                NotifyPropertyChanged("TargetCategoryIds");
            }
        }
        public string TrackingParameterName
        {
            get => trackingParameterName;
            set
            {
                trackingParameterName = value;
                NotifyPropertyChanged("TrackingParameterName");
            }
        }
        public string OutputParameterName
        {
            get => outputParameterName;
            set
            {
                outputParameterName = value;
                NotifyPropertyChanged("OutputParameterName");
            }
        }
        public string ToolTip
        {
            get
            {
                toolTipString = $"Rule Name: {name}" + Environment.NewLine +
                                $"Applies To: {String.Join(", ", targetCategoryIds)}" + Environment.NewLine +
                                $"Tracks Parameter : {trackingParameterName}" + Environment.NewLine +
                                $"Regex String: {RegexString}" + Environment.NewLine +
                                $"Writes To : {outputParameterName}" + Environment.NewLine +
                                $"Created By: {createdBy}" + Environment.NewLine +
                                $"Created At: {dateTimeCreated}";
                return toolTipString;
            }
            set
            {
                toolTipString = value;
                NotifyPropertyChanged("Name");
                NotifyPropertyChanged("TargetCategoryIds");
                NotifyPropertyChanged("TrackingParameterName");
                NotifyPropertyChanged("OutputParameterName");
                NotifyPropertyChanged("ToolTip");
                NotifyPropertyChanged("RegexString");
            }
        }
        public string RegexString
        {
            get => regexString;
            set
            {
                regexString = value;
                NotifyPropertyChanged("RegexString");
            }
        }
        public ObservableCollection<RegexRulePart> RegexRuleParts
        {
            get => regexRuleParts;
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
            dateTimeCreated = DateTime.Now.ToString("r");
            createdBy = Environment.UserName;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
