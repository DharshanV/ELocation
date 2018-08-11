using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace ELocation
{
    public class PopUpFragment : Android.Support.V4.App.Fragment
    {
        BusInformation busInformation;
        TextView[] textViews;
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            textViews = new TextView[6];
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = inflater.Inflate(Resource.Layout.PopUpFragmentLayout, container, false);
            textViews[0] = view.FindViewById<TextView>(Resource.Id.TV_BusID);
            textViews[1] = view.FindViewById<TextView>(Resource.Id.TV_Lat);
            textViews[2] = view.FindViewById<TextView>(Resource.Id.TV_Lon);
            textViews[3] = view.FindViewById<TextView>(Resource.Id.TV_SecsPassed);
            textViews[4] = view.FindViewById<TextView>(Resource.Id.TV_Heading);
            textViews[5] = view.FindViewById<TextView>(Resource.Id.TV_Speed);
            return view;
        }

        public void GetBusInfo(BusInformation bus)
        {
            busInformation = bus;
            textViews[0].Text = busInformation.ID;
            textViews[1].Text = busInformation.Lat;
            textViews[2].Text = busInformation.Lon;
            textViews[3].Text = busInformation.SecsPassed + "Secs";
            textViews[4].Text = busInformation.Heading;
            textViews[5].Text = busInformation.SpeedKmHr + "KmHr";
        }
    }
}