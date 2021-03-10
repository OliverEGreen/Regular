using System;
using System.ComponentModel;
using System.Windows.Threading;
using Regular.Models;
using Regular.Views;

namespace Regular.ViewModels
{
    public class ProgressBarViewModel : NotifyPropertyChangedBase
    {
        // Private Members & Default Values
        private int numberItemsTotal = 0;
        private int numberModificationsMade = 0;
        private int numberItemsProcessed = 0;
        private int progressPercentage = 0;
        private string title = "";
        
        // Public Properties & NotifyPropertyChanged Calls
        public int NumberItemsTotal
        {
            get => numberItemsTotal;
            set
            {
                numberItemsTotal = value;
                NotifyPropertyChanged();
            }
        }
        public int NumberModificationsMade
        {
            get => numberModificationsMade;
            set
            {
                numberModificationsMade = value;
                NotifyPropertyChanged();
            }
        }
        public int NumberItemsProcessed
        {
            get => numberItemsProcessed;
            set
            {
                numberItemsProcessed = value;
                NotifyPropertyChanged();
            }
        }
        public int ProgressPercentage
        {
            get => progressPercentage;
            set
            {
                progressPercentage = value;
                NotifyPropertyChanged();
            }
        }
        public string Title
        {
            get => title;
            set
            {
                title = value;
                NotifyPropertyChanged();
            }
        }
        
        public delegate void ProgressBarDelegate();
        
        // Unseen ProgressBar that allows us to fire the delegate method
        public System.Windows.Controls.ProgressBar ProgressBar { get; set; } = new System.Windows.Controls.ProgressBar();
        void UpdateProgress() => ProgressPercentage = Convert.ToInt32(NumberItemsProcessed * 100 / NumberItemsTotal);
        private void ProgressBarWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            (sender as BackgroundWorker)?.ReportProgress(ProgressPercentage);
        }

        public void UpdateProgressBar()
        {
            BackgroundWorker progressBarWorker = new BackgroundWorker { WorkerReportsProgress = true };
            progressBarWorker.DoWork += ProgressBarWorker_DoWork;
            progressBarWorker.RunWorkerAsync();

            NumberItemsProcessed++;
            ProgressBar.Dispatcher.Invoke(new ProgressBarDelegate(UpdateProgress), DispatcherPriority.Background);
        }
        
        public ProgressBarViewModel(RegexRule regexRule, int totalNumberOfItems)
        {
            Title = $"Running Rule: {regexRule.RuleName}";
            NumberItemsProcessed = 0;
            ProgressPercentage = 0;
            NumberItemsTotal = totalNumberOfItems;
            NumberModificationsMade = 0;
        }
    }
}
