using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Threading;
using Regular.Models;
using Regular.UI.RuleManager.Commands;

namespace Regular.UI.RuleManager.ViewModel
{
    public class RuleManagerViewModel : NotifyPropertyChangedBase
    {
        public string DocumentGuid { get; set; }


        // Private Members & Default Values
        private ObservableCollection<RegexRule> regexRules = new ObservableCollection<RegexRule>();
        private RegexRule selectedRegexRule = null;
        private ObservableCollection<RuleValidationOutput> ruleValidationOutputs = new ObservableCollection<RuleValidationOutput>();
        private int progressBarTotalNumberOfElements = 0;
        private int progressBarTotalNumberElementsProcessed = 0;
        private string trackingParameterName = "";
        private int progressBarPercentage = 0;
        

        // Public Properties and NotifyPropertyChanged
        public ObservableCollection<RegexRule> RegexRules
        {
            get => regexRules;
            set
            {
                regexRules = value;
                NotifyPropertyChanged();
            }
        }
        public RegexRule SelectedRegexRule
        {
            get => selectedRegexRule;
            set
            {
                selectedRegexRule = value;
                NotifyPropertyChanged();
            }
        }
        public ObservableCollection<RuleValidationOutput> RuleValidationOutputs
        {
            get => ruleValidationOutputs;
            set
            {
                ruleValidationOutputs = value;
                NotifyPropertyChanged();
            }
        }
        public int ProgressBarTotalNumberOfElements
        {
            get => progressBarTotalNumberOfElements;
            set
            {
                progressBarTotalNumberOfElements = value;
                NotifyPropertyChanged();
            }
        }
        public int ProgressBarTotalNumberElementsProcessed
        {
            get => progressBarTotalNumberElementsProcessed;
            set
            {
                progressBarTotalNumberElementsProcessed = value;
                NotifyPropertyChanged();
            }
        }
        public int ProgressBarPercentage
        {
            get => progressBarPercentage;
            set
            {
                progressBarPercentage = value;
                NotifyPropertyChanged();
            }
        }
        public string TrackingParameterName
        {
            get => trackingParameterName;
            set
            {
                trackingParameterName = value;
                NotifyPropertyChanged();
            }
        }

        public bool ExportReportEnabled { get; set; } = false;

        // ICommands
        public AddRuleCommand AddRuleCommand { get; }
        public DeleteRuleCommand DeleteRuleCommand { get; }
        public EditRuleCommand EditRuleCommand { get; }
        public DuplicateRuleCommand DuplicateRuleCommand { get; }
        public MoveRuleUpCommand MoveRuleUpCommand { get; }
        public MoveRuleDownCommand MoveRuleDownCommand { get; }
        public ExecuteRuleCommand ExecuteRuleCommand { get; }
        public ExportReportCommand ExportReportCommand { get; }

        
        // Progress Bar Thread Stuff
        public delegate void ProgressBarDelegate();
        public System.Windows.Controls.ProgressBar ProgressBar { get; set; } = new System.Windows.Controls.ProgressBar();
        void UpdateProgress() => ProgressBarPercentage = Convert.ToInt32(ProgressBarTotalNumberElementsProcessed* 100 / ProgressBarTotalNumberOfElements);
        private void ProgressBarWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            (sender as BackgroundWorker)?.ReportProgress(ProgressBarPercentage);
        }
        public void UpdateProgressBar()
        {
            BackgroundWorker progressBarWorker = new BackgroundWorker { WorkerReportsProgress = true };
            progressBarWorker.DoWork += ProgressBarWorker_DoWork;
            progressBarWorker.RunWorkerAsync();

            ProgressBarTotalNumberElementsProcessed++;
            ProgressBar.Dispatcher.Invoke(new ProgressBarDelegate(UpdateProgress), DispatcherPriority.Background);
        }


        public RuleManagerViewModel(string documentGuid)
        {
            DocumentGuid = documentGuid;
            RegexRules = RegularApp.RegexRuleCacheService.GetDocumentRules(DocumentGuid);

            AddRuleCommand = new AddRuleCommand(this);
            DeleteRuleCommand = new DeleteRuleCommand(this);
            EditRuleCommand = new EditRuleCommand(this);
            DuplicateRuleCommand = new DuplicateRuleCommand(this);
            MoveRuleUpCommand = new MoveRuleUpCommand(this);
            MoveRuleDownCommand = new MoveRuleDownCommand(this);
            ExecuteRuleCommand = new ExecuteRuleCommand(this);
            ExportReportCommand = new ExportReportCommand(this);
        }
    }
}
