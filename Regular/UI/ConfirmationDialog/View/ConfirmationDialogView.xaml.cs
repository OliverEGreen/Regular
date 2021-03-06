﻿using System.Windows;

namespace Regular.UI.ConfirmationDialog.View
{
    public partial class ConfirmationDialogView
    {
        public ConfirmationDialogView()
        {
            InitializeComponent();
        }

        private void ButtonCancel_OnClick(object sender, RoutedEventArgs e) => Close();
        
        private void ButtonDelete_OnClick(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }
    }
}
