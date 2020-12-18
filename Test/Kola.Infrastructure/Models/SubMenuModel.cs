using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kola.Infrastructure.Models
{
    public class SubMenuModel : BindableBase
    {
        
        private string _title;
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        private string _firstDescription;
        public string FirstDescription
        {
            get { return _firstDescription; }
            set { SetProperty(ref _firstDescription, value); }
        }

        private string _secondDescription;
        public string SecondDescription
        {
            get { return _secondDescription; }
            set { SetProperty(ref _secondDescription, value); }
        }

        private string _thirdDescription;
        public string ThirdDescription
        {
            get { return _thirdDescription; }
            set { SetProperty(ref _thirdDescription, value); }
        }

        private string _icon;
        public string Icon
        {
            get { return _icon; }
            set { SetProperty(ref _icon, value); }
        }

        private string _url;
        public string Url
        {
            get { return _url; }
            set { SetProperty(ref _url, value); }
        }


        public SubMenuModel()
        {

        }
    }
}
