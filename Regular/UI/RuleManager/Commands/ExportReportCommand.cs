using System;
using System.IO;
using System.Windows.Input;
using CsvHelper;
using CsvHelper.Configuration;
using Regular.Models;
using Regular.UI.RuleManager.ViewModel;
using Regular.Utilities;

namespace Regular.UI.RuleManager.Commands
{
    public class ExportReportCommand : ICommand
    {
        private readonly RuleManagerViewModel ruleManagerViewModel;

        public ExportReportCommand(RuleManagerViewModel ruleManagerViewModel)
        {
            this.ruleManagerViewModel = ruleManagerViewModel;
        }

        public bool CanExecute(object parameter) => ruleManagerViewModel.ExportReportEnabled;
        
        public void Execute(object parameter)
        {
            string filePath = IOUtils.PromptUserToSelectDestination("DataSpec Report", ".csv");
            if (string.IsNullOrWhiteSpace(filePath)) return;
            
            using (var writer = new StreamWriter(filePath))
            using (var csv = new CsvWriter(writer, false))
            {
                csv.Context.WriterConfiguration.RegisterClassMap(new RuleValidationOutputMap());
                csv.WriteRecords(ruleManagerViewModel.RuleValidationOutputs);
            }
        }

        public sealed class RuleValidationOutputMap : ClassMap<RuleValidationOutput>
        {
            public RuleValidationOutputMap()
            {
                Map(m => m.ElementId).Index(0).Name("Element ID");
                Map(m => m.ElementName).Index(1).Name("Name");
                Map(m => m.TrackingParameterValue).Index(2).Name("Value");
                Map(m => m.ValidationText).Index(3).Name("Validity");
                Map(m => m.CompliantExample).Index(4).Name("Compliant Example");
            }
        }


        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
    }
}