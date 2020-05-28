using Autodesk.Revit.DB;
using Autodesk.Revit.DB.ExtensibleStorage;
using Regular.Models;
using Regular.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Regular.Services;
using Regular.Models;
using System.Text;
using System.Threading.Tasks;

namespace Regular
{
    public static class RegexRuleManager
    {
        // A central class with CRUD functionality to manage the document's RegexRules
        public static RegexRule CreateRegexRule(string documentGuid, string ruleName, string targetCategoryName, string trackingParameterName, string outputParameterName, string regexString)
        {
            // Must add the rule to both the static cache and ExtensibleStorage (if successful)

            Document document = DocumentServices.GetRevitDocumentByGuid(documentGuid);
            RegexRule regexRule = new RegexRule(ruleName, targetCategoryName, trackingParameterName, outputParameterName);

            //Creating the necessary categoryset to create the outputParameter
            List<Category> categoriesList = new List<Category>();
            categoriesList.Add(Utilities.GetCategoryFromBuiltInCategory(document, BuiltInCategory.OST_Doors)); // Placeholder; we need to read this from the form
            CategorySet categorySet = Utilities.CreateCategorySetFromListOfCategories(document, RegularApp.RevitApplication, categoriesList);
            Parameter outputParameter = Utilities.CreateProjectParameter(document, RegularApp.RevitApplication, outputParameterName, ParameterType.Text, categorySet, BuiltInParameterGroup.PG_AREA, true);
            
            // RegexRule.OutputParameterName = outputParameter.Definition.Name;
            /*
            foreach (RegexRulePart regexRulePart in selectedRegexRuleParts)
            {
                RegexRule.RegexString += GetRegexPartFromRuleType(regexRulePart); // Something!! We build the string as we close the editor 
            }
            

            //This needs to be turned into a method taking a RegexRule and saving to ExtensibleStorage
            Entity entity = new Entity(Utilities.GetRegularSchema(Document));
            entity.Set("GUID", Guid.NewGuid());
            entity.Set("RuleName", ruleName);
            entity.Set("CategoryName", categoriesList.First().Name);
            entity.Set("TrackingParameterName", ((ComboBoxItem)ComboBoxInputTargetParameter.SelectedItem).Content.ToString());
            entity.Set("OutputParameterName", outputParameterName);
            entity.Set("RegexString", RegexRule.RegexString);
            IList<string> regexRulePartList = new List<string>();
            foreach (RegexRulePart regexRulePart in selectedRegexRuleParts)
            {
                regexRulePartList.Add($@"{regexRulePart.RawUserInputValue}:{regexRulePart.RuleType.ToString()}:{regexRulePart.IsOptional.ToString()}");
            }
            entity.Set<IList<string>>("regexRuleParts", regexRulePartList);
            using (Transaction transaction = new Transaction(Document, $"Saving RegexRule {ruleName}"))
            {
                transaction.Start();
                DataStorage dataStorage = DataStorage.Create(Document);
                dataStorage.SetEntity(entity);
                transaction.Commit();
            }*/




            return null;
        }
        public static RegexRule FetchRegexRule(string documentGuid, string regexRuleGuid)
        {
            ObservableCollection<RegexRule> documentRegexRules = FetchDocRegexRules(documentGuid);
            if (documentRegexRules == null) return null;
            return documentRegexRules.Where(x => x.Guid == regexRuleGuid).FirstOrDefault();
        }
        public static ObservableCollection<RegexRule> FetchDocRegexRules(string documentGuid)
        {
            if (RegularApp.AllRegexRules.ContainsKey(documentGuid)) { return RegularApp.AllRegexRules[documentGuid]; }
            return null;
        }
        public static void UpdateRegexRule(string documentGuid, string regexRuleGuid, RegexRule newRegexRule)
        {
            // Takes a newly-generated RegexRule object and sets an existing rules values to match
            // To be used when updating an existing rule from the Rule Editor
            RegexRule existingRegexRule = FetchRegexRule(documentGuid, regexRuleGuid);
            if (existingRegexRule == null) return;

            existingRegexRule.IsCaseSensitive = newRegexRule.IsCaseSensitive;
            existingRegexRule.OutputParameterName = newRegexRule.OutputParameterName;
            existingRegexRule.RegexRuleParts = newRegexRule.RegexRuleParts;
            existingRegexRule.RegexString = newRegexRule.RegexString;
            existingRegexRule.RuleName = newRegexRule.RuleName;
            existingRegexRule.TargetCategory = newRegexRule.TargetCategory;
            existingRegexRule.TrackingParameterName = newRegexRule.TrackingParameterName;
            return;
        }
        public static void DeleteRegexRule(string documentGuid, string regexRuleGuid)
        {
            // Deletes a RegexRule from the static cache and the document's ExtensibleStorage
            RegexRule regexRule = FetchRegexRule(documentGuid, regexRuleGuid);
            if (regexRule == null) return;
            // Some tidying up needs to happen here. We need to remove from the static cache as well 
            // As from extensiblestorage
        }
    }
}
