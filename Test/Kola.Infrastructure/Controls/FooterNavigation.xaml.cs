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
    public partial class FooterNavigation : ContentView
    {
        //private static double _selectedOpacity = 1;
        //private static double _unselectedOpacity = 1;
        private static Color _selectedColor = Color.FromHex("#B51A64");
        private static Color _unselectedColor = Color.FromHex("#B51A64");
        private static Color _selectedBacgroundColor = Color.FromHex("#F0E1E8");
        private static Color _unselectedBacgroundColor = Color.FromHex("#F9F9F9");

        public static readonly BindableProperty ConfigurationProperty = BindableProperty.Create(
            propertyName: "Configuration",
            returnType: typeof(string),
            declaringType: typeof(FooterNavigation),
            defaultValue: string.Empty,
            defaultBindingMode: BindingMode.OneWay,
            propertyChanged: HandleConfigurationPropertyChanged);
        public string Configuration
        {
            get
            {
                return (string)GetValue(ConfigurationProperty);
            }

            set
            {
                SetValue(ConfigurationProperty, value);
            }
        }
        private static void HandleConfigurationPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var content = (FooterNavigation)bindable;
            DoConfiguration(content);
        }
        public FooterNavigation()
        {
            InitializeComponent();
        }

        private static void DoConfiguration(FooterNavigation content)
        {
            if (string.IsNullOrEmpty(content.Configuration)) return;

            try
            {
                var items = content.Configuration.Split(',');

                switch (items[0])
                {
                    case "1":
                        content.HomeLabelColor.TextColor = _unselectedColor;
                        content.HomeIconColor.TextColor = _unselectedColor;
                        content.HomeButton.BackgroundColor = _unselectedBacgroundColor;
                        break;
                    case "2":
                        content.HomeLabelColor.TextColor = _selectedColor;
                        content.HomeIconColor.TextColor = _selectedColor;
                        content.HomeButton.BackgroundColor = _selectedBacgroundColor;
                        //content.HomeButton.IsEnabled = false;
                        break;
                }
                switch (items[1])
                {
                    case "1":
                        content.TransfersIconColor.TextColor = _unselectedColor;
                        content.TransfersLabelColor.TextColor = _unselectedColor;
                        content.TransfersButton.BackgroundColor = _unselectedBacgroundColor;
                        break;
                    case "2":
                        content.TransfersIconColor.TextColor = _selectedColor;
                        content.TransfersLabelColor.TextColor = _selectedColor;
                        content.TransfersButton.BackgroundColor = _selectedBacgroundColor;
                        //content.BookingButton.IsEnabled = false;
                        break;
                }
                switch (items[2])
                {
                    case "1":
                        content.AccountIconColor.TextColor = _unselectedColor;
                        content.AccountLabelColor.TextColor = _unselectedColor;
                        content.AccountButton.BackgroundColor = _unselectedBacgroundColor;
                        break;
                    case "2":
                        content.AccountIconColor.TextColor = _selectedColor;
                        content.AccountLabelColor.TextColor = _selectedColor;
                        content.AccountButton.BackgroundColor = _selectedBacgroundColor;
                        //content.AccountButton.IsEnabled = false;
                        break;
                }
                switch (items[3])
                {
                    case "1":
                        content.OptionsIconColor.TextColor = _unselectedColor;
                        content.OptionsLabelColor.TextColor = _unselectedColor;
                        content.OptionsButton.BackgroundColor = _unselectedBacgroundColor;
                        break;
                    case "2":
                        content.OptionsIconColor.TextColor = _selectedColor;
                        content.OptionsLabelColor.TextColor = _selectedColor;
                        content.OptionsButton.BackgroundColor = _selectedBacgroundColor;
                        //content.AccountButton.IsEnabled = false;
                        break;
                }
            }
            catch (System.Exception)
            {
                content.OptionsIconColor.TextColor = _unselectedColor;
                content.OptionsLabelColor.TextColor = _unselectedColor;
                content.OptionsButton.BackgroundColor = _selectedBacgroundColor;
                content.AccountIconColor.TextColor = _unselectedColor;
                content.AccountLabelColor.TextColor = _unselectedColor;
                content.AccountButton.BackgroundColor = _unselectedBacgroundColor;
                content.TransfersIconColor.TextColor = _unselectedColor;
                content.TransfersLabelColor.TextColor = _unselectedColor;
                content.TransfersButton.BackgroundColor = _unselectedBacgroundColor;
                content.HomeLabelColor.TextColor = _unselectedColor;
                content.HomeIconColor.TextColor = _unselectedColor;
                content.HomeButton.BackgroundColor = _unselectedBacgroundColor;
            }
        }
    }
}