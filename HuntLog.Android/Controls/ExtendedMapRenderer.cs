using Android.Gms.Maps;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Maps.Android;
using Xamarin.Forms.Platform.Android;
using HuntLog.Controls;
using HuntLog.Android.Controls;
using Android.Content;

[assembly: ExportRenderer(typeof (ExtendedMap), typeof (ExtendedMapRenderer))]

namespace HuntLog.Android.Controls
{
    public class ExtendedMapRenderer : MapRenderer, IOnMapReadyCallback
    {
        private GoogleMap _map;

        public ExtendedMapRenderer(Context context) : base(context)
        {
        }
        
        protected override void OnMapReady(GoogleMap map)
        {
            _map = map;
            if (_map != null)
            {
                _map.MapClick += googleMap_MapClick;
                //_map.MapType = GoogleMap.MapTypeTerrain;
            }
                
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Map> e)
        {
            if (_map != null)
            {
                _map.MapClick -= googleMap_MapClick; 
            }
               
            base.OnElementChanged(e);
            if (Control != null)
            {
                ((MapView)Control).GetMapAsync(this);
            }   
        }

        private void googleMap_MapClick(object sender, GoogleMap.MapClickEventArgs e)
        {
            ((ExtendedMap)Element).OnTap(new Position(e.Point.Latitude, e.Point.Longitude));
        }
    }
}