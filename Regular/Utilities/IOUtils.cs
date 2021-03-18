using System;
using Microsoft.Win32;
using Regular.UI.InfoWindow.View;

namespace Regular.Utilities
{
    public static class IOUtils
    {
        public static string GetFilterString(string fileExtension)
        {
            switch (fileExtension)
            {
                case ".json":
                    return $"JSON Files (*{fileExtension})|*{fileExtension}";
                case ".jpeg":
                    return $"JPEG Files (*{fileExtension})|*{fileExtension}";
                case ".gif":
                    return $"GIF Files (*{fileExtension})|*{fileExtension}";
                case ".bmp":
                    return $"Bitmap Files (*{fileExtension})|*{fileExtension}";
                case ".png":
                    return $"PNG Files (*{fileExtension})|*{fileExtension}";
                case ".csv":
                    return $"Comma Separated Value Files (*{fileExtension})|*{fileExtension}";
                case ".txt":
                    return $"Text Files (*{fileExtension})|*{fileExtension}";
            }
            return "";
        }
        
        public static string PromptUserToSelectDestination(string fileName, string fileExtension = "")
        {
            string timeStamp = DateTime.Now.ToString("yyMMdd HHmm");
            
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                FileName = $"{fileName} - {timeStamp}",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop)
            };

            if (!string.IsNullOrWhiteSpace(fileExtension))
            {
                saveFileDialog.DefaultExt = fileExtension;
                saveFileDialog.Filter = GetFilterString(fileExtension);
            }

            bool? result = saveFileDialog.ShowDialog();
            return result == true ? saveFileDialog.FileName : null;
        }

        public static string PromptUserToSelectFile(string fileExtension)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                DefaultExt = fileExtension,
                Filter = GetFilterString(fileExtension),
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
