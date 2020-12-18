using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Kola.Infrastructure.Controls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CheckedElement : Grid
    {
        private static Color _selectedBorder = Color.FromHex("#B51A64");
        private static Color _selectedBackground = Color.FromHex("#B51A64");
        private static Color _selectedText = Color.FromHex("#FFFFFF");
        private static Color _unSelectedBorder = Color.FromHex("#DBDBDB");
        private static Color _unSelectedBackground = Color.FromHex("#FFFFFF");
        private static Color _unSelectedText = Color.FromHex("#707070");

        public static readonly BindableProperty FirstElementProperty = BindableProperty.Create(
            propertyName: "FirstElement",
            returnType: typeof(string),
            declaringType: typeof(CheckedElement),
            defaultValue: string.Empty,
            defaultBindingMode: BindingMode.TwoWay,
            propertyChanged: HandleFirstElementPropertyChanged);
        public string FirstElement
        {
            get
            {
                return (string)GetValue(FirstElementProperty);
            }

            set
            {
                SetValue(FirstElementProperty, value);
            }
        }

        public static readonly BindableProperty SecondElementProperty = BindableProperty.Create(
            propertyName: "SecondElement",
            returnType: typeof(string),
            declaringType: typeof(CheckedElement),
            defaultValue: string.Empty,
            defaultBindingMode: BindingMode.TwoWay,
            propertyChanged: HandleSecondElementPropertyChanged);
        public string SecondElement
        {
            get
            {
                return (string)GetValue(SecondElementProperty);
            }

            set
            {
                SetValue(SecondElementProperty, value);
            }
        }

        public static readonly BindableProperty ElementSelectedProperty = BindableProperty.Create(
            propertyName: "ElementSelected",
            returnType: typeof(string),
            declaringType: typeof(CheckedElement),
            defaultValue: string.Empty,
            defaultBindingMode: BindingMode.TwoWay,
            propertyChanged: HandleElementSelectedPropertyChanged);
        public string ElementSelected
        {
            get
            {
                return (string)GetValue(ElementSelectedProperty);
            }

            set
            {
                SetValue(ElementSelectedProperty, value);
            }
        }

        public CheckedElement()
        {
            InitializeComponent();
        }

        private static void HandleFirstElementPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var content = (CheckedElement)bindable;
            if (string.IsNullOrEmpty(content.FirstElement)) return;
            content.firstLabel.Text = newValue.ToString();
            content.firstLabel.TextColor = _unSelectedText;
            content.first.BorderColor = _unSelectedBorder;
            content.first.BackgroundColor = _unSelectedBackground;
        }

        private static void HandleSecondElementPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var content = (CheckedElement)bindable;
            if (string.IsNullOrEmpty(content.SecondElement)) return;
            content.secondtLabel.Text = newValue.ToString();
            content.secondtLabel.TextColor = _unSelectedText;
            content.second.BorderColor = _unSelectedBorder;
            content.second.BackgroundColor = _unSelectedBackground;
        }

        private static void HandleElementSelectedPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var content = (CheckedElement)bindable;
            if (string.IsNullOrEmpty(content.ElementSelected)) return;
        }


        private void first_Tapped(object sender, EventArgs e)
        {
            firstLabel.TextColor = _selectedText;
            first.BorderColor = _selectedBorder;
            first.BackgroundColor = _selectedBackground;

            secondtLabel.TextColor = _unSelectedText;
            second.BorderColor = _unSelectedBorder;
            second.BackgroundColor = _unSelectedBackground;
            ElementSelected = firstLabel.Text;
        }

        private void second_Tapped(object sender, EventArgs e)
        {
            secondtLabel.TextColor = _selectedText;
            second.BorderColor = _selectedBorder;
            second.BackgroundColor = _selectedBackground;

            firstLabel.TextColor = _unSelectedText;
            first.BorderColor = _unSelectedBorder;
            first.BackgroundColor = _unSelectedBackground;
            ElementSelected = secondtLabel.Text;
        }
    }
}