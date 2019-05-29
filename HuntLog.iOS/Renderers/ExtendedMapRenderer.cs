using CoreGraphics;
using HuntLog.iOS.Renderers;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Maps.iOS;
using Xamarin.Forms.Platform.iOS;
using CoreLocation;
using MapKit;
using HuntLog.Controls;

[assembly: ExportRenderer(typeof (ExtendedMap), typeof (ExtendedMapRenderer))]

namespace HuntLog.iOS.Renderers
{
    public class ExtendedMapRenderer : MapRenderer
    {
        private readonly UITapGestureRecognizer _tapRecogniser;

        public ExtendedMapRenderer()
        {
            _tapRecogniser = new UITapGestureRecognizer(OnTap)
            {
                NumberOfTapsRequired = 1,
                NumberOfTouchesRequired = 1
            };
        }

        private void OnTap(UITapGestureRecognizer recognizer)
        {
            var cgPoint = recognizer.LocationInView(Control);
            var location = ((MKMapView)Control).ConvertPoint(cgPoint, Control);
            ((ExtendedMap)Element).OnTap(new Position(location.Latitude, location.Longitude));
        }

        protected override void OnElementChanged(ElementChangedEventArgs<View> e)
        {
            if (Control != null)
                Control.RemoveGestureRecognizer(_tapRecogniser);
            base.OnElementChanged(e);
            if (Control != null)
                Control.AddGestureRecognizer(_tapRecogniser);
        }

        //protected override MKAnnotationView GetViewForAnnotation(MKMapView mapView, IMKAnnotation annotation)
        //{
        //    MKAnnotationView annotationView = null;

        //    if (annotation is MKUserLocation)
        //        return null;

        //    var id = "myAnnotation";
        //    annotationView = mapView.DequeueReusableAnnotation(id);
        //    if (annotationView == null)
        //    {
        //        annotationView = new MKAnnotationView(annotation, id);
        //        annotationView.Image = UIImage.FromFile("Tabbar/gevir.png");
        //        annotationView.CalloutOffset = new CGPoint(0, 0);
        //        //annotationView.LeftCalloutAccessoryView = new UIImageView(UIImage.FromFile("monkey.png"));
        //        annotationView.RightCalloutAccessoryView = UIButton.FromType(UIButtonType.DetailDisclosure);

        //    }
        //    annotationView.CanShowCallout = true;

        //    return annotationView;
        //}
    }
}