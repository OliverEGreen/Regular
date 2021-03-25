using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Threading;
using Regular.Enums;
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
        private string reportSummary = "";
        private GridLength columnMarginWidth = new GridLength(0);
        private GridLength columnReportWidth = new GridLength(0);
        private int windowMinWidth = 350;
        private int windowWidth = 350;
        private int windowMaxWidth = 350;
        private int windowMinHeight = 500;
        private int windowHeight = 500;
        private int windowMaxHeight = 500;


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

        public string ReportSummary
        {
            get => reportSummary;
            set
            {
                reportSummary = value;
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
        public GridLength ColumnMarginWidth
        {
            get => columnMarginWidth;
            set
            {
                columnMarginWidth = value;
                NotifyPropertyChanged();
            }
        }
        public GridLength ColumnReportWidth
        {
            get => columnReportWidth;
            set
            {
                columnReportWidth = value;
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
        public int WindowMinWidth
        {
            get => windowMinWidth;
            set
            {
                windowMinWidth = value;
                NotifyPropertyChanged();
            }
        }
        public int WindowMaxHeight
        {
            get => windowMaxHeight;
            set
            {
                windowMaxHeight = value;
                NotifyPropertyChanged();
            }
        }
        public int WindowMinHeight
        {
            get => windowMinHeight;
            set
            {
                windowMinHeight = value;
                NotifyPropertyChanged();
            }
        }
        public int WindowWidth
        {
            get => windowWidth;
            set
            {
                windowWidth = value;
                NotifyPropertyChanged();
            }
        }
        public int WindowHeight
        {
            get => windowHeight;
            set
            {
                windowHeight = value;
                NotifyPropertyChanged();
            }
        }

        public bool ExportReportEnabled { get; set; } = false;
        public int NumberElementsValid { get; set; } = 0;

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

        public void UpdateReportSummary()
        {
            NumberElementsValid = RuleValidationOutputs.Count(x => x.RuleValidationResult == RuleValidationResult.Valid);
            double percentageValid = Math.Round(NumberElementsValid * 100.0 / ProgressBarTotalNumberElementsProcessed, 1);
            ReportSummary = $"{NumberElementsValid}/{ProgressBarTotalNumberElementsProcessed} ({percentageValid}%) values are valid.";
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
