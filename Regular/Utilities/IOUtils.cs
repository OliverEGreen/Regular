using System;
using System.Collections.Generic;
using Microsoft.Win32;
using Regular.Models;
using Regular.UI.InfoWindow.View;

namespace Regular.Utilities
{
    public static class IOUtils
    {
        public static string PromptUserToSelectDestination()
        {
            string timeStamp = DateTime.Now.ToString("yyMMdd HHmm");
            
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                FileName = $"Regular - DataSpec Rules {timeStamp}",
                DefaultExt = ".json",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop)
            };

            bool? result = saveFileDialog.ShowDialog();
            if (result == true) return saveFileDialog.FileName;
            
            new InfoWindowView
            (
                "Regular DataSpec",
                "Rule Export Was Cancelled",
                "The user cancelled the export.",
                true
            ).ShowDialog();

            return null;
        }

        public static string PromptUserToSelectJSONFile()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                DefaultExt = ".json",
                Filter = "JSON Files (.json)|*.json",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop)
            };

            bool? result = openFileDialog.ShowDialog();
            if (result != true)
            {
                new InfoWindowView
                (
                    "Regular DataSpec",
                    "Rule Import Was Cancelled",
                    "The user cancelled the import.",
                    true
                ).ShowDialog();

                return null;
            }

            if (openFileDialog.FileName.EndsWith(@".json")) return openFileDialog.FileName;
            
            new InfoWindowView
            (
                "Regular DataSpec",
                "Invalid File Type",
                "DataSpec required rule files to be in the .json file format.",
                true
            ).ShowDialog();

            return null;
        }
    }
}
