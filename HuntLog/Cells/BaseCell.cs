using System;
using HuntLog.Services;
using Xamarin.Forms;

namespace HuntLog.Cells
{
    public class BaseCell : ViewCell
    {
        protected INavigator _navigator;

        public static readonly BindableProperty CellActionProperty = BindableProperty.Create(nameof(CellAction), typeof(CellAction), typeof(ImageHeaderCell), null);

        public CellAction CellAction
        {
            get { return (CellAction)GetValue(CellActionProperty); }
            set { SetValue(CellActionProperty, value); }
        }


        public BaseCell()
        {
            _navigator = App.Navigator;
        }
    }
}
