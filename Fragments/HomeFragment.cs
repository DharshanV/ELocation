using System;
using System.Collections.Generic;
using System.ComponentModel;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;

namespace ELocation
{
    public class HomeFragment : Android.Support.V4.App.Fragment
    {
        private AutoCompleteTextView AgencyTextView, RouteTextView;
        private Button SubmitButton, ClearButton;
        private ImageView agencyDropDown, routeDropDown;
        private ListView infoListView;
        private TextView errorTextView;
        private Agencys agency;
        private Routes route;
        private Buses buses;
        private MapFragment mapFragment;
        private SendBusesInfo sender;
        private bool
        showAgencyDropdown = true,
        showRouteDropdown = true,
        startBusSearch = false,
        sameData = false;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            agency = new Agencys();
            route = new Routes();
            buses = new Buses();
            mapFragment = new MapFragment();
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = inflater.Inflate(Resource.Layout.HomeFragmentLayout, container, false);
            AgencyTextView = view.FindViewById<AutoCompleteTextView>(Resource.Id.AC_Agency_TextView);

            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += (o, e) =>
            { agency.StoreDataInList(); };
            worker.RunWorkerCompleted += (o, e) =>
            { AgencyTextView.Adapter = agency.Adapter(); };

            worker.RunWorkerAsync();
            worker.Dispose();
            return view;
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);
            SetFindIDs(view);
            SetContext(view);
            SetClickEvents();

            SubmitButton.Click += delegate
            {
                if (startBusSearch && sameData == false)
                {
                    BackgroundWorker worker = new BackgroundWorker();
                    buses.SetRoute(agency.Tag(), route.Tag());

                    worker.DoWork += (o, e) =>
                    { buses.StartBusData(); };
                    worker.RunWorkerCompleted += (o, e) =>
                    {
                        infoListView.Adapter = buses.InfoViewAdapter();
                        sender.SendBusesData(buses.GetBusData(),agency.Tag(),route.Tag());
                    };
                    worker.RunWorkerAsync();
                    worker.Dispose();
                }
                sameData = true;
            };
        }

        public override void OnAttach(Context context)
        {
            try { sender = (SendBusesInfo)Activity; }
            catch { new Exception("Error"); }
            base.OnAttach(context);
        }

        public interface SendBusesInfo
        {
            void SendBusesData(List<BusInformation> BusesData,string Agency,string Route);
        }

        private void SetFindIDs(View view)
        {
            RouteTextView = view.FindViewById<AutoCompleteTextView>(Resource.Id.AC_Route_TextView);
            agencyDropDown = view.FindViewById<ImageView>(Resource.Id.Dropdown_Img1);
            routeDropDown = view.FindViewById<ImageView>(Resource.Id.Dropdown_Img2);
            infoListView = view.FindViewById<ListView>(Resource.Id.Information_View);
            errorTextView = view.FindViewById<TextView>(Resource.Id.errorTextView);
            SubmitButton = view.FindViewById<Button>(Resource.Id.Submit_Button);
            ClearButton = view.FindViewById<Button>(Resource.Id.Clear_Button);
            RouteTextView.Enabled = false;
        }

        private void SetContext(View view)
        {
            agency.SetContext(view.Context);
            route.SetContext(view.Context);
            buses.SetContext(view.Context);
        }

        private void SetClickEvents()
        {
            agencyDropDown.Click += AgencyDropDown_Click;
            routeDropDown.Click += RouteDropDown_Click;

            AgencyTextView.ItemClick += AgencyTextView_ItemClick;
            AgencyTextView.TextChanged += AgencyTextView_TextChanged;
            RouteTextView.ItemClick += RouteTextView_ItemClick;
            RouteTextView.TextChanged += RouteTextView_TextChanged;
            ClearButton.Click += ClearButton_Click;
        }

        private void AgencyTextView_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            RouteTextView.Enabled = false;
            routeDropDown.Enabled = false;
            RouteTextView.Text = string.Empty;
            errorTextView.Text = string.Empty;
            if (e.Text.ToString() != string.Empty)
                if (!agency.Contains(e.Text.ToString()))
                    errorTextView.Text = "Invalid Agency";
        }

        private void AgencyTextView_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            AgencyTextView.ClearFocus();
            agency.SetTag(AgencyTextView.Text);

            route.SetAgencyTag(agency.Tag());
            route.StartRouteDate();

            RouteTextView.Adapter = route.Adapter();
            RouteTextView.Text = string.Empty;

            RouteTextView.Enabled = true;
            routeDropDown.Enabled = true;
        }

        private void RouteTextView_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            errorTextView.Text = string.Empty;
            startBusSearch = false;
            sameData = true;
            buses.ClearData();
            infoListView.Adapter = buses.InfoViewAdapter();

            if (e.Text.ToString() != string.Empty)
                if (!route.Contains(e.Text.ToString()))
                    errorTextView.Text = "Invalid Route";
        }

        private void RouteTextView_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            RouteTextView.ClearFocus();
            route.setRouteTag(RouteTextView.Text);
            startBusSearch = true;
            sameData = false;
        }

        private void AgencyDropDown_Click(object sender, EventArgs e)
        {
            if (showAgencyDropdown) {
                AgencyTextView.ShowDropDown();
                showAgencyDropdown = false;
            }
            else
            {
                AgencyTextView.DismissDropDown();
                showAgencyDropdown = true;
            }
        }

        private void RouteDropDown_Click(object sender, EventArgs e)
        {
            if (showRouteDropdown)
            {
                RouteTextView.ShowDropDown();
                showRouteDropdown = false;
            }
            else
            {
                RouteTextView.DismissDropDown();
                showRouteDropdown = true;
            }
        }

        private void ClearButton_Click(object sender, EventArgs e)
        {
            AgencyTextView.Text = string.Empty;
            RouteTextView.Text = string.Empty;
            buses.ClearData();
            infoListView.Adapter = buses.InfoViewAdapter();
            startBusSearch = false;
            sameData = false;
        }
    }
}