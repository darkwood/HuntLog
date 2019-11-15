using System;
using Xamarin.Forms;

namespace HuntLog.Controls
{
    public class ExtendedActivityIndicator : ActivityIndicator
    {
        public ExtendedActivityIndicator()
        {
            SetDynamicResource(BackgroundColorProperty, "PrimaryBackgroundColor");
            SetDynamicResource(ColorProperty, "Primary");
        }
    }
}
