using Android.App;
using Android.Content;
using Android.Gms.Maps.Model;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using ufinix.Helpers;

namespace Location_App_Labajo.Helpers
{
    internal class MapHelpers
    {
         public async Task <string> FindCordinateAddress(LatLng postion, string mapkey)
        {
            string url = " https://maps.googleapis.com/maps/api/geocode/json?latlng=" + postion.Latitude.ToString() + "," + postion.Longitude.ToString() +"&key=" + mapkey;
            string placesAddress = "";
            var handler = new HttpClientHandler();
            HttpClient httpClient = new HttpClient(handler);
            string result = await httpClient.GetStringAsync(url);


            if (!string.IsNullOrEmpty(result))
            {
                var geoCodeData = JsonConvert.DeserializeObject<GeocodingParser>(result);
                if (geoCodeData.status.Contains("OK"))
                {
                    placesAddress = geoCodeData.results[0].formatted_address;
                }
            }
            return placesAddress;
        }
    }
}