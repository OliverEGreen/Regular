using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Input;
using Regular.Models;
using Regular.UI.InfoWindow.View;
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

        public bool CanExecute(object parameter) => ruleManagerViewModel.ButtonsEnabled;
        
        public void Execute(object parameter)
        {
            string filePath = IOUtils.PromptUserToSelectDestination("DataSpec Report", ".csv");
            if (string.IsNullOrWhiteSpace(filePath)) return;

            List<string> reportRows = new List<string>();
            
            string headers = $"Element ID, Name, {ruleManagerViewModel.TrackingParameterName}, Validity, Compliant Example";
            reportRows.Add(headers);

            foreach(RuleValidationOutput ruleValidationOutput in ruleManagerViewModel.RuleValidationOutputs)
            {
                string rowData = $"{ruleValidationOutput.ElementId.IntegerValue.ToString().Replace(",", "")}," +
                                 $"{ruleValidationOutput.ElementName.Replace(",", "")}," +
                                 $"{ruleValidationOutput.TrackingParameterValue.Replace(",", "")}," +
                                 $"{ruleValidationOutput.ValidationText.Replace(",", "")}," +
                                 $"{ruleValidationOutput.CompliantExample.Replace(",", "")}";
                
                reportRows.Add(rowData);
            }

            string report = string.Join(Environment.NewLine, reportRows);

            using (var writer = new StreamWriter(filePath))
            {
                writer.Write(report);
            }

            new InfoWindowView
            (
                "Regular DataSpec",
                "Export Completed",
                "The report has been exported to .csv format.",
                false
            ).ShowDialog();
        }

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
    }
}