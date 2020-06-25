﻿using System.ComponentModel;

namespace Regular.ViewModel
{
    public class ParameterObject : INotifyPropertyChanged
    {
        private string name;
        private int id;

        public string Name
        {
            get => name;
            set
            {
                name = value;
                NotifyPropertyChanged("RuleName");
            }
        }
        public int Id
        {
            get => id;
            set
            {
                id = value;
                NotifyPropertyChanged("Id");
            }
        }
        
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

