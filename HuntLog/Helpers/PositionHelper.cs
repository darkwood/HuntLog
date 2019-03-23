using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms.Maps;

namespace HuntLog.Helpers
{
    public static class PositionHelper
    {
        public static async Task<Location> GetLocationAsync() 
        {
            try
            {
                var request = new GeolocationRequest(GeolocationAccuracy.Medium);
                var location = await Geolocation.GetLocationAsync(request);
                return location;
            }
            catch (FeatureNotSupportedException fnsEx)
            {
                // Handle not supported on device exception
                Console.WriteLine("GetLocationAsync() failed: FeatureNotSupportedException:" + fnsEx.Message);
            }
            catch (FeatureNotEnabledException fneEx)
            {
                // Handle not enabled on device exception
                Console.WriteLine("GetLocationAsync() failed: FeatureNotEnabledException:" + fneEx.Message);
            }
            catch (PermissionException pEx)
            {
                // Handle permission exception
                Console.WriteLine("GetLocationAsync() failed: PermissionException:" + pEx.Message);
            }
            catch (Exception ex)
            {
                // Unable to get location
                Console.WriteLine("GetLocationAsync() failed: Exception:" + ex.Message);
            }
            return null;
        }
        public static async Task<string> GetLocationNameForPosition(double latitude, double longitude)
        {
            string sted = string.Empty;
            try
            {
                var geoCoder = new Geocoder();
                var geoPos = new Position(latitude, longitude);
                var possibleAddresses = await geoCoder.GetAddressesForPositionAsync(geoPos);
                if (possibleAddresses.Any())
                {
                    sted = possibleAddresses.First();
                    int newLinePos = sted.IndexOf(Environment.NewLine, StringComparison.CurrentCultureIgnoreCase);
                    if (newLinePos > 0) //removes line 2
                    {
                        sted = sted.Substring(0, newLinePos);
                    }
                    if (sted.Length > 5 && Regex.IsMatch(sted, "^\\d{4}[\" \"]")) //removes zipcode
                    {
                        sted = sted.Substring(5);
                    }
                }
            }
            catch (Exception ex)
            {
                //TODO: Log this
                throw new Exception(ex.Message, ex);
            }
            return sted;
        }

    }
}
