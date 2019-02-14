using System;
using System.Linq;
using System.Reflection;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using HuntLog.Droid.Renderers;

[assembly: ExportRenderer(typeof(ContentPage), typeof(ExtendedPageRenderer))]
namespace HuntLog.Droid.Renderers
{
    /// <summary>
    /// Custom renderer for ContentPage that allows for action buttons to be on the left or the right hand side (ex: a modal with cancel and done buttons)
    /// ToolbarItems need to have Priority set to 0 to show on the left, 1 to show on the right (ref: https://gist.github.com/alexlau811/f1fff9e726333e6b4a2f)
    /// </summary>
    public class ExtendedPageRenderer : ContentPage
    {
        public ExtendedPageRenderer()
        {
        }
    }
}