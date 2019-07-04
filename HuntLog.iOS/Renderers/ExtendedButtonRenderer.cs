using System;
using HuntLog.Helpers;
using HuntLog.iOS.Renderers;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(Button), typeof(ExtendedButtonRenderer))]
namespace HuntLog.iOS.Renderers
{
    public class ExtendedButtonRenderer : ButtonRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Button> e)
        {
            base.OnElementChanged(e);
            if (Control != null)
            {
                Control.TintColor = Utility.PRIMARYBRIGHT_COLOR.ToUIColor();
            }
        }
    }
}
