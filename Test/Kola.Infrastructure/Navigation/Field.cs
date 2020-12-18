using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace Kola.Infrastructure.Navigation
{
    public class Field<T> : BindableBase // INotifyPropertyChanged
    {
        private string _id;
        private T _value;
        private string _icon;
        private bool _hasError;
        private string _errorText;
        private bool _isVisible;
        private bool _isLoading;
        private bool _isSelected;
        //public event PropertyChangedEventHandler PropertyChanged;

        public string Id
        {
            get { return _id; }
            set { SetProperty(ref _id, value); }
        }

        public T Value
        {
            get { return _value; }
            set { SetProperty(ref _value, value); }
        }

        public string Icon
        {
            get { return _icon; }
            set { SetProperty(ref _icon, value); }
        }

        public bool HasError
        {
            get { return _hasError; }
            set { SetProperty(ref _hasError, value); }
        }
        public string ErrorText
        {
            get { return _errorText; }
            set { SetProperty(ref _errorText, value); }
        }
        public bool IsVisible
        {
            get { return _isVisible; }
            set { SetProperty(ref _isVisible, value); }
        }

        public bool IsLoading
        {
            get { return _isLoading; }
            set { SetProperty(ref _isLoading, value); }
        }

        public bool IsSelected
        {
            get { return _isSelected; }
            set { SetProperty(ref _isSelected, value); }
        }

        public Field()
        {
            _value = default(T);
            _isVisible = true;
            _isLoading = false;
        }
        public Field(T pvalue)
        {
            _value = pvalue;
            _isVisible = true;
            _isLoading = false;
        }
        public Field(string id, T pvalue, bool pvisible)
        {
            _id = id;
            _value = pvalue;
            _isVisible = pvisible;
            _isLoading = false;
        }

        public Field(string id, T pvalue, string icon, bool pvisible)
        {
            _id = id;
            _value = pvalue;
            _icon = icon;
            _isVisible = pvisible;
            _isLoading = false;
        }

        public Field(string id, T pvalue, string icon, string invalidMessageError, bool pvisible)
        {
            _id = id;
            _value = pvalue;
            _icon = icon;
            _errorText = invalidMessageError;
            _isVisible = pvisible;
            _isLoading = false;
        }

        //protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        //{
        //    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        //}
    }
}
