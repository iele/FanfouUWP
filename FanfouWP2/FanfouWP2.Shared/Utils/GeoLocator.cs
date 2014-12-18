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
                double lat = pos.Coordinate.Point.Position.Latitude;
                double lon = pos.Coordinate.Point.Position.Longitude;
                double nlat, nlon;
                Utils.EvilTransform.transform(lat, lon, out nlat, out nlon);


                return nlat + "," + nlon;
            }
            catch (Exception)
            {
                return "";
            }
        }
    }
}