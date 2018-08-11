using System.Collections.Generic;
using Android.Content;
using Android.Widget;
using System.Xml;

namespace ELocation
{
    class Buses
    {
        private List<string> busInfoList;
        private List<BusInformation> busData;
        private Context myContext;
        private string myRouteTag, myAgencyTag;
        private XmlReader xmlReader;
        private int count = 0;
        private string URL = "http://webservices.nextbus.com/service/publicXMLFeed?command=vehicleLocations&a=";

        public Buses()
        {
            busInfoList = new List<string>();
            busData = new List<BusInformation>();
        }

        public void SetContext(Context context)
        {
            myContext = context;
        }

        public void SetRoute(string agencyTag, string routeTag)
        {
            myAgencyTag = agencyTag;
            myRouteTag = routeTag;
        }

        public void StartBusData()
        {
            busInfoList = new List<string>();
            busData = new List<BusInformation>();
            string newURL = URL + myAgencyTag + "&r=" + myRouteTag + "&t=1144953500233";
            xmlReader = XmlReader.Create(newURL);
            count = 0;
            while (xmlReader.Read())
            {
                if (xmlReader.HasAttributes)
                {
                    if (xmlReader.Name == "vehicle")
                    {
                        BusInformation bus = new BusInformation();
                        for (int x = 0; x < xmlReader.AttributeCount; x++)
                        {
                            xmlReader.MoveToAttribute(x);
                            switch (xmlReader.Name)
                            {
                                case "id":
                                    busInfoList.Add("Bus #"+(count+1)+" ID: "+ xmlReader.GetAttribute(x));
                                    bus.ID = xmlReader.GetAttribute(x);
                                    break;
                                case "lat":
                                    bus.Lat = xmlReader.GetAttribute(x);
                                    break;
                                case "lon":
                                    bus.Lon = xmlReader.GetAttribute(x);
                                    break;
                                case "secsSinceReport":
                                    bus.SecsPassed = xmlReader.GetAttribute(x);
                                    break;
                                case "heading":
                                    bus.Heading = xmlReader.GetAttribute(x);
                                    break;
                                case "speedKmHr":
                                    bus.SpeedKmHr = xmlReader.GetAttribute(x);
                                    break;
                            }
                        }
                        busData.Add(bus);
                        count++;
                    }
                }
            }
        }

        public int BusCount()
        {
            return count;
        }

        public ArrayAdapter<string> InfoViewAdapter()
        {
            return new ArrayAdapter<string>(myContext, Android.Resource.Layout.SimpleListItem1, busInfoList);
        }

        public List<BusInformation> GetBusData()
        {
            return busData;
        }

        public void ClearData()
        {
            busInfoList.Clear();
            busData.Clear();
        }
    }
}