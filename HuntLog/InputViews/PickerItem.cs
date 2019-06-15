using System;
using System.Threading.Tasks;
using HuntLog.AppModule;
using Xamarin.Forms;

namespace HuntLog.InputViews
{
    public class PickerItem : ViewModelBase
    {
        public bool Selected { get; set; }
        public Command Tapped { get; set; }
        public bool Custom { get; internal set; }
        public Func<PickerItem, Task> ParentAction { get; set; }
        public PickerItem()
        {
            Tapped = new Command(async () =>
            {
                Selected = !Selected;
                await ParentAction?.Invoke(this);
            });
        }
    }
}
