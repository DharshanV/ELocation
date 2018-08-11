using System.Collections.Generic;
using Android.Content;
using Android.Widget;
using System.Xml;
namespace ELocation
{
    class Agencys
    {
        private string AgencyTag;
        private XmlReader xmlReader;
        private Context myContext;
        private List<List<string>> agencyData;
        private int agencyCount = 0;
        readonly private string URL = "http://webservices.nextbus.com/service/publicXMLFeed?command=agencyList";

        public Agencys() { }

        public void SetContext(Context context)
        {
            myContext = context;
        }

        public void StoreDataInList()
        {
            agencyData = new List<List<string>>();
            xmlReader = XmlReader.Create(URL);
            while (xmlReader.Read())
            {
                if (xmlReader.HasAttributes)
                {
                    if (xmlReader.Name == "agency")
                    {
                        List<string> temp = new List<string>();
                        for (int x = 0; x < 2; x++)
                        {
                            xmlReader.MoveToAttribute(x);
                            temp.Add(xmlReader.GetAttribute(x));
                        }
                        agencyData.Add(temp);
                        agencyCount++;
                    }
                }
            }
        }

        public int getCount()
        {
            return agencyCount;
        }

        public bool Contains(string textValue)
        {
            for (int x = 0; x < agencyCount; x++)
            {
                for (int y = 0; y < 2; y++)
                {
                    if (agencyData[x][y] == textValue)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public void SetTag(string value)
        {
            for (int x = 0; x < agencyCount; x++)
            {
                for (int y = 0; y < 2; y++)
                {
                    if (agencyData[x][y] == value)
                    {
                        AgencyTag = agencyData[x][0];
                        return;
                    }
                }
            }
        }

        public string Tag()
        {
            return AgencyTag;
        }

        private string GetListValue(int ListPostion, int position)
        {
            return agencyData[ListPostion][position];
        }

        public ArrayAdapter<string> Adapter()
        {
            List<string> agencyList = new List<string>();
            for (int x = 0; x < agencyCount; x++)
            {
                for (int y = 0; y < 2; y++)
                {
                    agencyList.Add(GetListValue(x, y));
                }
            }

            return new ArrayAdapter<string>(myContext, Android.Resource.Layout.SimpleListItem1, agencyList);
        }
    }
}