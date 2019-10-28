using System;
using HuntLog.Services;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;

namespace HuntLog.Cells
{
    public class BaseCell : ViewCell
    {
        protected INavigator _navigator;

        public StackLayout ViewLayout { get; set; }

        public static readonly BindableProperty CellActionProperty = BindableProperty.Create(nameof(CellAction), typeof(CellAction), typeof(ImageHeaderCell), null);

        public CellAction CellAction
        {
            get { return (CellAction)GetValue(CellActionProperty); }
            set { SetValue(CellActionProperty, value); }
        }



    public BaseCell()
        {
            _navigator = App.Navigator;

            ViewLayout = new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                Margin = new Thickness(20, 5, 5, 5),
                HeightRequest = 50
            };

            this.On<iOS>().SetDefaultBackgroundColor(Color.Transparent);
        }
    }
}
