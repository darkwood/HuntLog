using System.Linq;
using System.Reflection;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using UIKit;
using HuntLog.iOS.Renderers.Renderers;
using System;
using HuntLog.Styles;

[assembly: ExportRenderer(typeof(ContentPage), typeof(ExtendedPageRenderer))]
namespace HuntLog.iOS.Renderers.Renderers
{
    /// <summary>
    /// Custom renderer for ContentPage that allows for action buttons to be on the left or the right hand side (ex: a modal with cancel and done buttons)
    /// ToolbarItems need to have Priority set to 0 to show on the left, 1 to show on the right (ref: https://gist.github.com/alexlau811/f1fff9e726333e6b4a2f)
    /// </summary>
    public class ExtendedPageRenderer : PageRenderer
    {
        protected override void OnElementChanged(VisualElementChangedEventArgs e)
        {
            base.OnElementChanged(e);

            if (e.OldElement != null || Element == null)
            {
                return;
            }

            try
            {
                SetTheme();
            }
            catch (Exception) { }
        }

        public override void TraitCollectionDidChange(UITraitCollection previousTraitCollection)
        {
            base.TraitCollectionDidChange(previousTraitCollection);

            if(TraitCollection.UserInterfaceStyle != previousTraitCollection.UserInterfaceStyle)
            {
                SetTheme();
            }
        }

        private void SetTheme()
        {
            if (TraitCollection.UserInterfaceStyle == UIUserInterfaceStyle.Dark)
                Xamarin.Forms.Application.Current.Resources = new DarkTheme();
            else
                Xamarin.Forms.Application.Current.Resources = new LightTheme();
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            var contentPage = this.Element as ContentPage;
            if (contentPage == null || NavigationController == null)
                return;

            var itemsInfo = contentPage.ToolbarItems;

            var navigationItem = this.NavigationController.TopViewController.NavigationItem;
            var leftNativeButtons = (navigationItem.LeftBarButtonItems ?? new UIBarButtonItem[] { }).ToList();
            var rightNativeButtons = (navigationItem.RightBarButtonItems ?? new UIBarButtonItem[] { }).ToList();

            var newLeftButtons = new UIBarButtonItem[] { }.ToList();
            var newRightButtons = new UIBarButtonItem[] { }.ToList();

            rightNativeButtons.ForEach(nativeItem =>
            {
                // [Hack] Get Xamarin private field "item"
                var field = nativeItem.GetType().GetField("_item", BindingFlags.NonPublic | BindingFlags.Instance);
                if (field == null)
                    return;

                var info = field.GetValue(nativeItem) as ToolbarItem;
                if (info == null)
                    return;

                if (info.Priority == 0)
                    newLeftButtons.Add(nativeItem);
                else
                    newRightButtons.Add(nativeItem);
            });

            leftNativeButtons.ForEach(nativeItem =>
            {
                newLeftButtons.Add(nativeItem);
            });

            navigationItem.RightBarButtonItems = newRightButtons.ToArray();
            navigationItem.LeftBarButtonItems = newLeftButtons.ToArray();
        }
    }
}