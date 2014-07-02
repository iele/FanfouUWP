using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;

namespace FanfouWP2.Utils
{
    public class GeoLocator
    {
        public async static Task<string> getGeolocator()
        {
            Geolocator geolocator = new Geolocator();
            geolocator.DesiredAccuracyInMeters = 50;

            try
            {
                Geoposition pos = await geolocator.GetGeopositionAsync();
                return pos.Coordinate.Point.Position.Latitude.ToString() + ", " + pos.Coordinate.Point.Position.Longitude.ToString();
            }
            catch (Exception)
            {
                return "";
            }
        }
    }
}
