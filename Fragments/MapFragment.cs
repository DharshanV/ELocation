using System;
using System.Collections.Generic;
using Android;
using Android.Content;
using Android.Content.PM;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.OS;
using Android.Views;
using Android.Widget;
using Android.Locations;
using Android.Support.Design.Widget;
using System.ComponentModel;
using System.Diagnostics;

namespace ELocation
{
    public class MapFragment : Android.Support.V4.App.Fragment, View.IOnTouchListener, IOnMapReadyCallback
    {
        private FrameLayout fragmentContainer;
        private List<BusInformation> busesInformation;
        private GoogleMap myGoogleMap;
        private MapView mapView;
        private LatLng position;
        private Stopwatch watch;
        private FloatingActionButton refreshButton;
        private LocationManager locationManager;
        private SendBusInfo busInfoSender;
        private float mLastPosY;
        private string myAgency, myRoute;
        private const int RequestLocationId = 0;
        private bool GPSEnabled = false;
        private readonly string[] Permissions =
        {
            Manifest.Permission.AccessFineLocation,
            Android.Manifest.Permission.AccessCoarseLocation
        };

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = inflater.Inflate(Resource.Layout.MapFragmentLayout, container, false);
            SetViewIDs(view);
            Inilitize();
            Instance(savedInstanceState);
            SetClickEvents();
            return view;
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
            if (requestCode == RequestLocationId)
            {
                if (grantResults[0] == Permission.Granted)
                {
                    myGoogleMap.MyLocationEnabled = true;
                    MoveToCurrentLocation(true);
                }
                else
                    myGoogleMap.MyLocationEnabled = false;
            }
        }

        public override void OnAttach(Context context)
        {
            try { busInfoSender = (SendBusInfo)Activity; }
            catch { }
            base.OnAttach(context);
        }

        public interface SendBusInfo
        {
            void SendBus(BusInformation bus);
        }

        public void SetBusesData(List<BusInformation> BusesData, string Agency, string Route)
        {
            myAgency = Agency;
            myRoute = Route;
            SetMapData(BusesData);
            watch.Start();
        }

        public bool OnTouch(View v, MotionEvent e)
        {
            switch (e.Action)
            {
                case MotionEventActions.Down:
                    mLastPosY = e.GetY();
                    return true;
                case MotionEventActions.Move:
                    float curretPosition = e.GetY();
                    float deltaY = mLastPosY - curretPosition;
                    float transY = v.TranslationY;
                    transY -= deltaY;
                    if (transY < 0)
                        transY = 0;
                    v.TranslationY = transY;
                    return true;
                default:
                    return v.OnTouchEvent(e);
            }
        }

        private void RefreshButton_Click(object sender, EventArgs e)
        {
            if (myAgency != null && watch.ElapsedMilliseconds >= 10000)
            {
                Buses buses = new Buses();
                buses.SetContext(Context);
                buses.SetRoute(myAgency, myRoute);
                BackgroundWorker worker = new BackgroundWorker();
                worker.DoWork += (o, e1) => { buses.StartBusData(); };
                worker.RunWorkerCompleted += (o, e1) => { SetMapData(buses.GetBusData()); };
                worker.RunWorkerAsync();
                worker.Dispose();
                watch.Restart();
            }
        }

        private void MyGoogleMap_MarkerClick(object sender, GoogleMap.MarkerClickEventArgs e)
        {
            if (busesInformation != null && busesInformation.Count != 0)
                busInfoSender.SendBus(busesInformation[int.Parse(e.Marker.Title)]);

            if (fragmentContainer.TranslationY + 20 >= fragmentContainer.Height)
            {
                var interpolator = new Android.Views.Animations.OvershootInterpolator(5);
                fragmentContainer.Animate().SetInterpolator(interpolator)
                    .TranslationYBy(-200)
                    .SetDuration(500);
            }
            fragmentContainer.SetOnTouchListener(this);
        }

        public void OnMapReady(GoogleMap googleMap)
        {
            myGoogleMap = googleMap;
            if (GPSEnabled == true)
            {
                if (PermissionGranted())
                    myGoogleMap.MyLocationEnabled = true;

                MoveToCurrentLocation(myGoogleMap.MyLocationEnabled);
            }

            myGoogleMap.MarkerClick += MyGoogleMap_MarkerClick;
        }

        private void SetMapData(List<BusInformation> BusesData)
        {
            if(BusesData == null || BusesData.Count ==0)
                return;
            myGoogleMap.Clear();
            busesInformation.Clear();
            busesInformation = BusesData;

            for (int x = 0; x < busesInformation.Count; x++)
            {
                MarkerOptions markerOptions = new MarkerOptions();
                position = new LatLng(Double.Parse(busesInformation[x].Lat), Double.Parse(busesInformation[x].Lon));
                markerOptions.SetPosition(position);
                markerOptions.SetIcon(BitmapDescriptorFactory.FromResource(Resource.Drawable.ic_directions_bus));
                markerOptions.Anchor(.5f, .7f);
                markerOptions.SetTitle(x.ToString());
                myGoogleMap.AddMarker(markerOptions);
            }
        }

        public void MoveToCurrentLocation(bool move)
        {
            Location location = myGetLastKnowLocation();
            double latitude = location.Latitude;
            double longitude = location.Longitude;
            LatLng latLng = new LatLng(latitude, longitude);
            myGoogleMap.MoveCamera(CameraUpdateFactory.NewLatLngZoom(latLng, 10));
        }

        private bool PermissionGranted()
        {
            if (Context.CheckSelfPermission(Permissions[0]) == (int)Permission.Granted)
            {
                return true;
            }
            else
            {
                if (ShouldShowRequestPermissionRationale(Permissions[0])) { }
                RequestPermissions(Permissions, RequestLocationId);
            }
            return false;
        }

        private Location myGetLastKnowLocation()
        {
            IList<String> providers = locationManager.GetProviders(true);
            Location bestLocation = null;
            foreach (String provider in providers)
            {
                Location l = locationManager.GetLastKnownLocation(provider);
                if (l == null)
                    continue;
                if (bestLocation == null || l.Accuracy < bestLocation.Accuracy)
                    bestLocation = l;
            }
            return bestLocation;
        }

        private void Inilitize()
        {
            watch = new Stopwatch();
            busesInformation = new List<BusInformation>();
            locationManager = (LocationManager)Context.GetSystemService(Context.LocationService);
            try { MapsInitializer.Initialize(Activity.ApplicationContext); }
            catch (Exception e) { Console.WriteLine(e.Message); }

            try { GPSEnabled = locationManager.IsProviderEnabled(LocationManager.GpsProvider); }
            catch (Exception e) { Console.WriteLine(e.Message); }
        }

        private void Instance(Bundle savedInstanceState)
        {
            mapView.OnCreate(savedInstanceState);
            mapView.OnResume();
            mapView.GetMapAsync(this);
        }

        private void SetViewIDs(View view)
        {
            fragmentContainer = view.FindViewById<FrameLayout>(Resource.Id.PopUpFragmentContainer);
            refreshButton = view.FindViewById<FloatingActionButton>(Resource.Id.FAB_Refresh);
            mapView = view.FindViewById<MapView>(Resource.Id.mapView);
            var trans = Activity.SupportFragmentManager.BeginTransaction();
            trans.Add(fragmentContainer.Id, new PopUpFragment(), "PopUpFragment");
            trans.Commit();
        }

        private void SetClickEvents()
        {
            refreshButton.Click += RefreshButton_Click;
        }

    }
}