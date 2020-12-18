using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kola.Starting.Models
{
    public class SlideItem : BindableBase
    {
        private string _image;
        private string _icon;
        private int _iconSize;
        private string _primarytext;
        private string _secondarytext;
        private string _url;
        public string Image
        {
            get => _image;
            set
            {
                SetProperty(ref _image, value);
            }
        }

        public string Icon
        {
            get => _icon;
            set
            {
                SetProperty(ref _icon, value);
            }
        }

        public int IconSize
        {
            get => _iconSize;
            set
            {
                SetProperty(ref _iconSize, value);
            }
        }

        public string PrimayText
        {
            get => _primarytext;
            set
            {
                SetProperty(ref _primarytext, value);
            }
        }
        public string Url
        {
            get => _url;
            set
            {
                SetProperty(ref _url, value);
            }
        }
        public string Secondarytext
        {
            get => _secondarytext;
            set
            {
                SetProperty(ref _secondarytext, value);
            }
        }
    }
}
