using HuntLog.AppModule;
using Xamarin.Forms;

namespace HuntLog.InputViews
{
    public class PickerItem : ViewModelBase
    {
        public bool Selected { get; set; }
        public Command Tapped { get; set; }
        public bool Custom { get; internal set; }

        public PickerItem()
        {
            Tapped = new Command(() =>
            {
                Selected = !Selected;
            });
        }
    }
}
