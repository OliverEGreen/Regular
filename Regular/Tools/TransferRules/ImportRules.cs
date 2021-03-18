using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Regular.Models;
using Regular.UI;
using Regular.UI.InfoWindow.View;
using Regular.UI.SelectElements.Model;
using Regular.UI.SelectElements.View;
using Regular.Utilities;

namespace Regular.Tools.TransferRules
{
    [Transaction(TransactionMode.Manual)]
    public class ImportRules : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            try
            {
                Document document = commandData.Application.ActiveUIDocument.Document;
                
                string filePath = IOUtils.PromptUserToSelectFile(".json");
                if (string.IsNullOrWhiteSpace(filePath))
                {
                    new InfoWindowView
                    (
                        "Regular DataSpec",
                        "Rule Export Was Cancelled",
                        "The given file path was invalid.",
                        true
                    ).ShowDialog();
                    return Result.Cancelled;
                }

                string serialisedRegexRules = JSONFileUtils.ReadJSONFromFile(filePath);
                List<RegexRule> regexRules = SerializationUtils.DeserializeRegexRules(serialisedRegexRules);

                if (regexRules == null || regexRules.Count < 1)
                {
                    new InfoWindowView
                    (
                        "Regular DataSpec",
                        "No Rules Found",
                        "No DataSpec rules were found in the document",
                        true
                    ).ShowDialog();
                    return Result.Cancelled;
                }
                
                // Prompting the user to specify which rules are to be exported
                ObservableCollection<ObservableObject> observableObjects = new ObservableCollection<ObservableObject>();
                foreach (RegexRule regexRule in regexRules)
                {
                    observableObjects.Add(new ObservableObject(regexRule)
                    {
                        DisplayName =  regexRule.RuleName,
                        IsChecked = true,
                    });
                }

                SelectElementsInfo selectElementsInfo = new SelectElementsInfo
                {
                    Document = document,
                    ObservableObjects = observableObjects,
                    UIDocument = commandData.Application.ActiveUIDocument,
                    UserMessage = "Select one or more DataSpec rules to export to the .json file format.",
                };

                SelectElementsView selectElementsView = new SelectElementsView(selectElementsInfo);
                selectElementsView.ShowDialog();
                if (selectElementsView.DialogResult != true)
                {
                    new InfoWindowView
                    (
                        "Regular DataSpec",
                        "Rule Export Was Cancelled",
                        "The user cancelled the export.",
                        true
                    ).ShowDialog();
                    return Result.Cancelled;
                }
                
                List<RegexRule> selectedRegexRules = selectElementsView.SelectElementsViewModel.InputObservableObjects
                    .Where(x => x.IsChecked)
                    .Select(x => x.OriginalObject)
                    .OfType<RegexRule>()
                    .ToList();


                return Result.Succeeded;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return Result.Failed;
            }
        }
    }
}