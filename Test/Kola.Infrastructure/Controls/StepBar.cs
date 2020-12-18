using System.Linq;
using Xamarin.Forms;

namespace Kola.Infrastructure.Controls
{

    public class StepBar : StackLayout
    {
        #region Properties
        public Color StepColor
        {
            get => (Color)GetValue(StepColorProperty);
            set => SetValue(StepColorProperty, value);
        }

        public Color StepTextColor
        {
            get => (Color)GetValue(StepTextColorProperty);
            set => SetValue(StepTextColorProperty, value);
        }

        public Color StepTextColorNotActive
        {
            get => (Color)GetValue(StepTextColorNotActiveProperty);
            set => SetValue(StepTextColorNotActiveProperty, value);
        }

        public Color StepColorNotActive
        {
            get => (Color)GetValue(StepColorNotActiveProperty);
            set => SetValue(StepColorNotActiveProperty, value);
        }

        public Color BarColor
        {
            get => (Color)GetValue(BarColorProperty);
            set => SetValue(BarColorProperty, value);
        }

        public int StepsTotal
        {
            get => (int)GetValue(StepsTotalProperty);
            set => SetValue(StepsTotalProperty, value);
        }

        public int StepCurrent
        {
            get => (int)GetValue(StepCurrentProperty);
            set => SetValue(StepCurrentProperty, value);
        }

        public int StepOld
        {
            get;
            set;
        }

        private double _opacityLow = 0.2;
        private double _opacityHigh = 1;
        #endregion

        #region Fields        
        public static readonly BindableProperty StepsTotalProperty = BindableProperty.Create(nameof(StepsTotal), typeof(int), typeof(StepBar), 0);
        public static readonly BindableProperty StepCurrentProperty = BindableProperty.Create(nameof(StepCurrent), typeof(int), typeof(StepBar), 0, defaultBindingMode: BindingMode.TwoWay);
        public static readonly BindableProperty StepColorProperty = BindableProperty.Create(nameof(StepColor), typeof(Color), typeof(StepBar), Color.Blue, defaultBindingMode: BindingMode.TwoWay);
        public static readonly BindableProperty StepTextColorProperty = BindableProperty.Create(nameof(StepTextColor), typeof(Color), typeof(StepBar), Color.White, defaultBindingMode: BindingMode.TwoWay);
        public static readonly BindableProperty StepColorNotActiveProperty = BindableProperty.Create(nameof(StepColorNotActive), typeof(Color), typeof(StepBar), Color.LightBlue, defaultBindingMode: BindingMode.TwoWay);
        public static readonly BindableProperty StepTextColorNotActiveProperty = BindableProperty.Create(nameof(StepTextColorNotActive), typeof(Color), typeof(StepBar), Color.Black, defaultBindingMode: BindingMode.TwoWay);
        public static readonly BindableProperty BarColorProperty = BindableProperty.Create(nameof(BarColor), typeof(Color), typeof(StepBar), Color.Transparent, defaultBindingMode: BindingMode.TwoWay);

        #endregion

        public StepBar()
        {
            HorizontalOptions = LayoutOptions.FillAndExpand;
            VerticalOptions = LayoutOptions.Start;
            ClassId = $"StepBarClassId";
            AutomationId = $"StepBardId";
            StepOld = 1;
            Padding = new Thickness(0, 0);
            Spacing = 0;
            Padding = 0;
            Margin = 0;
            Orientation = StackOrientation.Horizontal;
        }

        protected override void OnPropertyChanged(string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);
            if (propertyName == StepsTotalProperty.PropertyName)
            {
                for (var i = 1; i <= StepsTotal; i++)
                {
                    var button = new Button()
                    {
                        Text = $"{i}",
                        TextColor = StepTextColorNotActive,
                        ClassId = $"Step_{i}",
                        AutomationId = $"Step_{i}",
                        BackgroundColor = StepColorNotActive,
                        // BorderColor = StepColor,
                        BorderWidth = 0.5,
                        WidthRequest = 40,
                        HeightRequest = 40,
                        CornerRadius = 20,
                        Opacity = _opacityHigh,
                        Margin = 0
                    };
                    if (i <= StepCurrent)
                    {
                        button.BackgroundColor = StepColor;
                        button.TextColor = StepTextColor;
                    }

                    this.Children.Add(button);

                    if (i < StepsTotal)
                    {
                        var separatorLine = new BoxView()
                        {
                            BackgroundColor = BarColor,
                            HeightRequest = 1,
                            WidthRequest = 5,
                            Margin = 0,
                            VerticalOptions = LayoutOptions.Center,
                            HorizontalOptions = LayoutOptions.FillAndExpand
                        };
                        this.Children.Add(separatorLine);
                    }
                }
            }
            else if (propertyName == StepCurrentProperty.PropertyName)
            {
                Button _childrenCurrent = (Button)this.Children.Single(p => p.ClassId == $"Step_{StepCurrent}");
                Button _childrenOld = (Button)this.Children.Single(p => p.ClassId == $"Step_{StepOld}");

                if (_childrenCurrent != null && StepCurrent >= StepOld)
                {
                    _childrenCurrent.BackgroundColor = StepColor;
                    _childrenCurrent.BorderColor = StepColor;
                    _childrenCurrent.TextColor = StepTextColor;
                }
                else if (_childrenOld != null && StepCurrent < StepOld)
                {
                    _childrenOld.BackgroundColor = StepColorNotActive;
                    _childrenOld.BorderColor = StepColorNotActive;
                    _childrenOld.TextColor = StepTextColorNotActive;
                }

                StepOld = StepCurrent;
            }
        }
    }
}
