using Android;
using Android.App;
using Android.Gms.Location;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.OS;
using Android.Runtime;
using Android.Widget;
using AndroidX.AppCompat.App;
using AndroidX.Core.App;
using System.Xml.Serialization;

namespace Location_App_Labajo
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity, IOnMapReadyCallback
    {

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

            map.UiSettings.ZoomControlsEnabled = true;


            if (checKPermission())
            {
                displayLocationAsync();
            }

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