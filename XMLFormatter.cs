using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace WeatherPusher
{
    public struct locationdata
    {
        public string city;
        public string state;
        public string country;
        public int zip;
    }
    public struct day10data
    {
        public DateTime TheTime;
        public int high;
        public int low;
        public int maxwind;
        public int avewind;
        public string direction;
        public int avehumidity;
        public string conditions;
    }
    static class XMLFormatter
    {
        private static string BaseUrl = "http://api.wunderground.com/api/";
        private static string APIkey = "db468b66c0d8382f";
        public static string[] features = {"/alerts","/almanac","/astronomy","/conditions","/currenthurrican",
                                          "/forecast", "/forecast10day","/geolookup","/history","/houly","/hourly10day","/planner",
                                          "/rawtide","/tide","/webcams","/yesterday"};
        public static  bool[] enablefeatures = { false, false, false, false, false, false, false, false,
                                           false, false, false, false, false, false, false, false }; //This list allows us to choose which features we enable when 
                                                                                                     //we build our lookupURL
        public static string lookupstate = null; //This is the state we use to look up the location, it is not the official location data
        public static string lookupcity = null; // This is the city we use to look up the location, it is not the official location data
        public static string lookupzip = null; //This is the zip code we use to look up the lcation, it is not the official location data
        public static day10data[] Tenday = new day10data[10]; //This is the data structure used to store the 10 day forcast data
        public static locationdata location = new locationdata(); //This is the data structure used to store the official location data
        private const string autoip = "autoip"; //autoup lookup url string 
        private const string format = "xml";
        private static string LookupUrl;
        static XMLFormatter()
        {
            for (int i = 0; i < 10; ++i)
            {
                Tenday[i] = new day10data();
            }

        }
        private static void BuildUrl()
        {
            LookupUrl = BaseUrl + APIkey;
            for (int i = 0; i < 16; i++)
            {
                if (enablefeatures[i] == true)
                {
                    LookupUrl = LookupUrl + features[i];
                }
            }
            LookupUrl = LookupUrl + "/q/";
            if (lookupstate == null && lookupcity == null && lookupzip == null)
            {
                LookupUrl = LookupUrl + autoip;
            }
            if (lookupstate != null && lookupcity != null && lookupzip != null)
            {
                LookupUrl = LookupUrl + lookupzip;
            }
            if (lookupstate != null && lookupcity == null && lookupzip == null)
            {
                LookupUrl = LookupUrl + lookupzip;
            }
            if (lookupstate != null && lookupcity != null && lookupzip == null)
            {
                LookupUrl = LookupUrl + lookupstate + "/" + lookupcity;
            }
            LookupUrl = LookupUrl + "." + format;
            Console.WriteLine(LookupUrl);

        }
        static public void get10day()
        {
            enablefeatures[6] = true;
            BuildUrl();
            enablefeatures[6] = false;
            int i = 0;
            XmlTextReader reader = new XmlTextReader(LookupUrl);
            reader.ReadToFollowing("simpleforecast");
            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        if (reader.Name == "date")
                        {
                            reader.ReadToFollowing("epoch");
                            reader.Read();
                            var epoch = new DateTime(1969, 12, 31, 0, 0, 0, DateTimeKind.Utc);
                            Tenday[i].TheTime = epoch.AddSeconds(Convert.ToDouble(reader.Value));
                            //Console.WriteLine("epoch: " + reader.Value);
                            reader.ReadToFollowing("high");
                            reader.ReadToFollowing("fahrenheit");
                            reader.Read();
                            Tenday[i].high = Convert.ToInt32(reader.Value);
                            //Console.WriteLine("high: " + reader.Value);
                            reader.ReadToFollowing("low");
                            reader.ReadToFollowing("fahrenheit");
                            reader.Read();
                            Tenday[i].low = Convert.ToInt32(reader.Value);
                            //Console.WriteLine("low: " + reader.Value);
                            reader.ReadToFollowing("conditions");
                            reader.Read();
                            Tenday[i].conditions = reader.Value;
                            //Console.WriteLine("Conditions: " + reader.Value);
                            reader.ReadToFollowing("maxwind");
                            reader.ReadToFollowing("mph");
                            reader.Read();
                            Tenday[i].maxwind = Convert.ToInt32(reader.Value);
                            //Console.WriteLine("maxwind: " + reader.Value);
                            reader.ReadToFollowing("avewind");
                            reader.ReadToFollowing("mph");
                            reader.Read();
                            //Console.WriteLine("avewind: " + reader.Value);
                            reader.ReadToFollowing("avehumidity");
                            reader.Read();
                            Tenday[i].avehumidity = Convert.ToInt32(reader.Value);
                            //Console.WriteLine("ave humidity" + reader.Value);
                            i++;

                        }
                        break;
                }
            }
            for (int j = 0; j < 10; j++)
            {
                //For debugging purposes, uncommonent the next line to inspect the weather data in the array data structure. 
                Console.WriteLine(Tenday[j].TheTime.Month + "/" +  Tenday[j].TheTime.Day + "/" + Tenday[j].TheTime.Year + " High: " + Tenday[j].high + " Low: " + Tenday[j].low + " Condition: " +Tenday[j].conditions);
            }
        }
        static public void geolookup()
        {
            enablefeatures[7] = true;
            BuildUrl();
            enablefeatures[7] = false;
            XmlTextReader reader = new XmlTextReader(LookupUrl);
         
            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        if (reader.Name == "location")
                        {
                            reader.ReadToFollowing("country");
                            reader.Read();
                            location.country = reader.Value;
                            //Console.WriteLine("country: " + reader.Value);
                            reader.ReadToFollowing("state");
                            reader.Read();
                            location.state = reader.Value;
                            //Console.WriteLine("state: " + reader.Value);
                            reader.ReadToFollowing("city");
                            reader.Read();
                            location.city = reader.Value;
                            //Console.WriteLine("city: " + reader.Value);
                            reader.ReadToFollowing("zip");
                            reader.Read();
                            location.zip = Convert.ToInt32(reader.Value);
                            //Console.WriteLine("zip: " + reader.Value);
                            Console.WriteLine("This is the weather for " + location.city + " ," + location.state + " ," + location.zip);
                        }
                        
                        break;
                }
            }
            
        }
    }
}
