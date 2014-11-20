using System;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;

namespace FanfouWP2.Utils
{
    public class GeoLocator
    {
        public static async Task<string> getGeolocator()
        {
            var geolocator = new Geolocator();
            geolocator.DesiredAccuracyInMeters = 50;

            try
            {
                Geoposition pos = await geolocator.GetGeopositionAsync();
                return pos.Coordinate.Point.Position.Latitude + ", " + pos.Coordinate.Point.Position.Longitude;
            }
            catch (Exception)
            {
                return "";
            }
        }
    }
}