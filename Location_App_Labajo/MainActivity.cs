using Android;
using Android.App;
using Android.Gms.Location;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.Locations;
using Android.OS;
using Android.Runtime;
using Android.Widget;
using AndroidX.Annotations;
using AndroidX.AppCompat.App;
using AndroidX.Core.App;
using Location_App_Labajo.Helpers;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Location_App_Labajo
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity, IOnMapReadyCallback
    {
        TextView placeTextView;
        ImageButton locationButton;
        ImageView locationPin;

        //permission
        readonly string[] permissionGroup =
        {
          Manifest.Permission.AccessCoarseLocation,
          Manifest.Permission.AccessFineLocation
        };
        //object for googlemaps
        GoogleMap map;

        //last location 
        IFusedLocationProviderClient lpc;
        Android.Locations.Location myLastLocation;
        private LatLng myPosition;

        MapHelpers mapHelper = new MapHelpers();
        IList<Address> addresses = new List<Address>();
        Geocoder geocoder;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);



            RequestPermissions(permissionGroup, 0);

            //referencing the map  fragment
            SupportMapFragment mapFragment = (SupportMapFragment)SupportFragmentManager.FindFragmentById(Resource.Id.map);
            mapFragment.GetMapAsync(this);



            locationButton = FindViewById<ImageButton>(Resource.Id.locationButton);
            locationPin = FindViewById<ImageView>(Resource.Id.locationPin);
            placeTextView = (TextView)FindViewById(Resource.Id.placeTextView);
            locationButton.Click += LocationButton_Click;
        }

        private void LocationButton_Click(object sender, System.EventArgs e)
        {
           
            if (checKPermission())
            {
                displayLocationAsync();
            }

        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        
            if(grantResults.Length < 1)
            {
                return;
            }

            if (grantResults[0] == Android.Content.PM.Permission.Granted)
            {
                displayLocationAsync();
            }
        }

        public void OnMapReady(GoogleMap googleMap)
        {
            var mapStyle = MapStyleOptions.LoadRawResourceStyle
                (this, Resource.Raw.mapstyle);
            googleMap.SetMapStyle(mapStyle);

            map = googleMap;
            map.CameraMoveStarted += Map_CameraMoveStarted;
            map.CameraIdle += Map_CameraIdle;
            map.UiSettings.ZoomControlsEnabled = true;


            if (checKPermission())
            {
                displayLocationAsync();
            }

        }
       
        private async void Map_CameraIdle(object sender, System.EventArgs e)
        {


            var position = map.CameraPosition.Target;
            geocoder = new Geocoder(this);
            addresses = await geocoder.GetFromLocationAsync(position.Latitude, position.Longitude, 1);

            if (addresses != null && addresses.Count >0)
            {
                Address addresss = addresses[0];
                string formattedAddress = addresss.GetAddressLine(0);
                placeTextView.Text = formattedAddress.ToUpper();
            }
            else
            {
                placeTextView.Text = "Where to?";
            }
           /* string key = Resources.GetString(Resource.String.mapkey);
            string address = await mapHelper.FindCordinateAddress(  position, key);
            placeTextView.Text = address;

            if (!string.IsNullOrEmpty(address))
            {
                placeTextView.Text = address;
            }
            else
            {
                placeTextView.Text = "Where to?";
            }*/
        }

        private void Map_CameraMoveStarted(object sender, GoogleMap.CameraMoveStartedEventArgs e)
        {
           
            placeTextView.Text = "Setting new location";
        }

        bool checKPermission()
        {
            bool permissionGranted = false;
            if (ActivityCompat.CheckSelfPermission(this, Manifest.Permission.AccessCoarseLocation)
                != Android.Content.PM.Permission.Granted &&
                ActivityCompat.CheckSelfPermission(this, Manifest.Permission.AccessFineLocation)
                != Android.Content.PM.Permission.Granted)
            {
                permissionGranted = false;
            }
            else
            {
                permissionGranted = true;
            }
            return permissionGranted;
        }


        async void displayLocationAsync()
        {
            if(lpc == null)
            {
                lpc = LocationServices.GetFusedLocationProviderClient(this);
            }

            myLastLocation = await lpc.GetLastLocationAsync();


            if(myLastLocation != null)
            {
                myPosition = new LatLng(myLastLocation.Latitude, myLastLocation.Longitude);
                map.AnimateCamera(CameraUpdateFactory.NewLatLngZoom(myPosition,15));
            }
        }

    }
}