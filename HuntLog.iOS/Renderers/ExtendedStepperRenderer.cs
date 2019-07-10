using System;
using HuntLog.Helpers;
using HuntLog.iOS.Renderers;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(Stepper), typeof(ExtendedStepperRenderer))]
namespace HuntLog.iOS.Renderers
{
    public class ExtendedStepperRenderer : StepperRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Stepper> e)
        {
            base.OnElementChanged(e);
            if (Control != null)
                Control.TintColor = Utility.PRIMARY_COLOR.ToUIColor();
        }
    }
}
