using System.Collections.Generic;
using Android.Content;
using Android.Widget;
using System.Xml;

namespace ELocation
{
    class Routes
    {
        private string routeTag, agencyTag;
        private List<string> routeList;
        private XmlReader xmlReader;
        private Context myContext;
        private string URL = "http://webservices.nextbus.com/service/publicXMLFeed?command=routeList&a=";

        public Routes(){ }

        public void SetContext(Context context)
        {
            myContext = context;
        }

        public void SetAgencyTag(string AgencyTag)
        {
            agencyTag = AgencyTag;
        }

        public void StartRouteDate()
        {
            xmlReader = XmlReader.Create(URL + agencyTag);
            routeList = new List<string>();
            while (xmlReader.Read())
            {
                if (xmlReader.HasAttributes)
                {
                    if (xmlReader.Name == "route")
                    {
                        xmlReader.MoveToAttribute(1);
                        routeList.Add(xmlReader.GetAttribute(1));
                    }
                }
            }
        }

        public void setRouteTag(string value)
        {
            routeTag = string.Empty;
            if (value.Contains(" "))
                routeTag = value.Substring(0, value.IndexOf(" "));
            else
                routeTag = value;
        }

        public string Tag()
        {
            return routeTag;
        }

        public bool Contains(string value)
        {
            if (routeList.Contains(value))
                return true;
            return false;
        }

        public ArrayAdapter<string> Adapter()
        {
            return new ArrayAdapter<string>(myContext, Android.Resource.Layout.SimpleDropDownItem1Line, routeList);
        }
    }
}