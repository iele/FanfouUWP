using System;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.Services.Maps;

namespace FanfouUWP.Utils
{
    public static class GeoLocator
    {
        public static async Task<Tuple<double, double>> locator()
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

                return new Tuple<double, double>(nlat, nlon);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static async Task<string> geolocator()
        {
            var loc = await locator();
            if (loc != null)
                return loc.Item1 + ", " + loc.Item2;
            else
                return "";
        }

        public static async Task<MapLocationFinderResult> geocode(string location)
        {
            BasicGeoposition queryHint = new BasicGeoposition();
            var loc = await locator();
            queryHint.Latitude = loc.Item1;
            queryHint.Longitude = loc.Item2;
            Geopoint hintPoint = new Geopoint(queryHint);

            MapLocationFinderResult result = await MapLocationFinder.FindLocationsAsync(location, hintPoint);

            return result;
        }
    }
}