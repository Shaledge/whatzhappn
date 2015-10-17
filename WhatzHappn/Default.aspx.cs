using System;
using System.Collections.Generic;
using System.Net;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Xml;
using System.Xml.XPath;
using System.IO;
using System.Diagnostics;
using System.Reflection;
using Newtonsoft;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;


//Twitter
//using Spring.Social.Twitter;
//using Spring.Social.Twitter.Api;
//using Spring.Social.Twitter.Api.Impl;


//Yelp
using SimpleOAuth;
using CommandLine;
using System.Text;


namespace WhatzHappn
{
    public class IPInfo
    {
        public string ip { get; set; }
        public string hostname { get; set; }
        public string city { get; set; }
        public string region { get; set; }
        public string country { get; set; }
        public string loc { get; set; }
        public string org { get; set; }
        public string postal { get; set; }
    }

    class WikipediaObject
    {
        [JsonProperty(PropertyName = "batchcomplete")]
        public string BatchComplete { get; set; }

        [JsonProperty(PropertyName = "query")]
        public QueryObject Query { get; set; }
    }

    class QueryObject
    {
        [JsonProperty(PropertyName = "geosearch")]
        public List<GeoSearchResult> GeoSearch { get; set; }
    }

    class GeoSearchResult
    {
        [JsonProperty(PropertyName = "pageid")]
        public string PageId { get; set; }

        [JsonProperty(PropertyName = "ns")]
        public string Ns { get; set; }

        [JsonProperty(PropertyName = "title")]
        public string Title { get; set; }

        [JsonProperty(PropertyName = "lat")]
        public decimal Latitude { get; set; }

        [JsonProperty(PropertyName = "lon")]
        public decimal Longitude { get; set; }

        [JsonProperty(PropertyName = "distance")]
        public decimal Distance { get; set; }

        [JsonProperty(PropertyName = "primary")]
        public string Primary { get; set; }
    }

    class YelpAPIClient
    {
        #region Private Members
        private string msConsumerKey = "";
        private string msConsumerSecret = "";
        private string msToken = "";
        private string msTokenSecret = "";

        public class Business
        {
            public string is_claimed { get; set; }

            public string rated { get; set; }

            public string mobile_url { get; set; }

            public string rating_img_url { get; set; }

            public string review_img_url { get; set; }

            public string review_count { get; set; }

            public string name { get; set; }
        }
        #endregion

        #region Public Properties
        public string ConsumerKey
        {
            get { return msConsumerKey; }

            set { msConsumerKey = value; }
        }

        public string ConsumerSecret
        {
            get { return msConsumerSecret; }

            set { msConsumerSecret = value; }
        }

        public string Token
        {
            get { return msToken; }

            set { msToken = value; }
        }

        public string TokenSecret
        {
            get { return msTokenSecret; }

            set { msTokenSecret = value; }
        }
        #endregion

        public YelpAPIClient()
        { }

        public YelpAPIClient(
            string ConsumerKey, 
            string ConsumerSecret, 
            string Token, 
            string TokenSecret)
        {
            this.ConsumerKey = ConsumerKey;
            this.ConsumerSecret = ConsumerSecret;
            this.Token = Token;
            this.TokenSecret = TokenSecret;
        }


        /// <summary>
        /// Host of the API.
        /// </summary>
        private const string API_HOST = "http://api.yelp.com";

        /// <summary>
        /// Relative path for the Search API.
        /// </summary>
        private const string SEARCH_PATH = "/v2/search/";

        /// <summary>
        /// Relative path for the Business API.
        /// </summary>
        private const string BUSINESS_PATH = "/v2/business/";

        /// <summary>
        /// Search limit that dictates the number of businesses returned.
        /// </summary>
        private const int SEARCH_LIMIT = 3;

        /// <summary>
        /// Prepares OAuth authentication and sends the request to the API.
        /// </summary>
        /// <param name="baseURL">The base URL of the API.</param>
        /// <param name="queryParams">The set of query parameters.</param>
        /// <returns>The JSON response from the API.</returns>
        /// <exception>Throws WebException if there is an error from the HTTP request.</exception>
        private JObject PerformRequest(string baseURL, Dictionary<string, string> queryParams = null)
        {
            var query = System.Web.HttpUtility.ParseQueryString(String.Empty);

            if (queryParams == null)
            {
                queryParams = new Dictionary<string, string>();
            }

            foreach (var queryParam in queryParams)
            {
                query[queryParam.Key] = queryParam.Value;
            }

            var uriBuilder = new UriBuilder(baseURL);
            uriBuilder.Query = query.ToString();

            WebRequest request = WebRequest.Create(uriBuilder.ToString());
            request.Method = "GET";

            request.SignRequest(
                new Tokens
                {
                    ConsumerKey = this.ConsumerKey,
                    ConsumerSecret = this.ConsumerSecret,
                    AccessToken = this.Token,
                    AccessTokenSecret = this.TokenSecret
                }
            ).WithEncryption(EncryptionMethod.HMACSHA1).InHeader();

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            var stream = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
            return JObject.Parse(stream.ReadToEnd());
        }

        /// <summary>
        /// Query the Search API by a search term and location.
        /// </summary>
        /// <param name="term">The search term passed to the API.</param>
        /// <param name="location">The search location passed to the API.</param>
        /// <returns>The JSON response from the API.</returns>
        public JObject Search(string term, string location)
        {
            string baseURL = API_HOST + SEARCH_PATH;
            var queryParams = new Dictionary<string, string>()
            {
                { "term", term },
                { "location", location },
                { "limit", SEARCH_LIMIT.ToString() }
            };
            return PerformRequest(baseURL, queryParams);
        }

        /// <summary>
        /// Query the Business API by a business ID.
        /// </summary>
        /// <param name="business_id">The ID of the business to query.</param>
        /// <returns>The JSON response from the API.</returns>
        public JObject GetBusiness(string business_id)
        {
            string baseURL = API_HOST + BUSINESS_PATH + business_id;
            return PerformRequest(baseURL);
        }
    }

    public partial class _Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                this.WHBody.Attributes.Add("onload", "WHInit()");
                this.Title = "Whatz Happn: Version 0.014";
                string sIPAddress = GetIPAddress();
                sIPAddress = "71.125.17.29";  //TEMP for development
                

                if (IsValidIP(sIPAddress) == true)
                {
                    IPInfo ipInfo = GetIPInfo(sIPAddress);

                    GetMap(ipInfo);

                    GetWeather(ipInfo);

                    //GetTwitter(ipInfo);

                    GetWikipedia(ipInfo);

                    GetYelp(ipInfo);
                }
                else
                {
                    //TODO: Inform user we cannot get location.
                }
            }
            catch (Exception ex)
            {
                logException(ex);
            }
        }

        private string GetIPAddress()
        {
            string sIPAddress = "";

            try
            {
                System.Web.HttpContext context = System.Web.HttpContext.Current;
                string ipAddress = context.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

                if (!string.IsNullOrEmpty(ipAddress))
                {
                    string[] addresses = ipAddress.Split(',');
                    if (addresses.Length != 0)
                    {
                        sIPAddress = addresses[0];
                    }
                }
                else
                {
                    sIPAddress = context.Request.ServerVariables["REMOTE_ADDR"];
                }
            }
            catch (Exception ex)
            {
                logException(ex);
            }

            return sIPAddress;
        }

        private bool IsValidIP(string ipAddress)
        {
            bool bReturn = false;

            try
            {
                if (System.Text.RegularExpressions.Regex.IsMatch(ipAddress, "[0-9]{1,3}\\.[0-9]{1,3}\\.[0-9]{1,3}\\.[0-9]{1,3}"))
                {
                    string[] ips = ipAddress.Split('.');
                    if (ips.Length == 4 || ips.Length == 6)
                    {
                        if (System.Int32.Parse(ips[0]) < 256 && System.Int32.Parse(ips[1]) < 256
                            & System.Int32.Parse(ips[2]) < 256 & System.Int32.Parse(ips[3]) < 256)
                        {
                            bReturn = true;
                        }
                        else
                        {
                            bReturn = false;
                        }
                    }
                    else
                    {
                        bReturn = false;
                    }
                }
                else
                {
                    bReturn = false;
                }
            }
            catch (Exception ex)
            {
                logException(ex);
            }

            return bReturn; 
        }

        private IPInfo GetIPInfo(string IPAddress)
        {
            string sURL = "http://ipinfo.io/" + IPAddress;
            using (WebClient client = new WebClient())
            {
                string json = client.DownloadString(sURL);
                IPInfo ipinfo = new JavaScriptSerializer().Deserialize<IPInfo>(json);
                return ipinfo;
            }
        }

        private string GetKeys(string Name)
        {
            string sReturn = "";

            try
            {
                string sPath = HttpContext.Current.Server.MapPath("~/App_Data/" + Name + ".txt");

                if (File.Exists(sPath) == true)
                {
                    using (StreamReader sr = new StreamReader(sPath))
                    {
                        sReturn = sr.ReadToEnd();
                    }
                }
            }
            catch (Exception ex)
            {
                logException(ex);
            }

            return sReturn;
        }

        private void GetMap(IPInfo ipInfo)
        {
            try
            {
                string[] sLocation = ipInfo.loc.ToString().Split(',');
                var sMapSource = "http://maps.google.com?q=" + sLocation[0] + "," + sLocation[1] + "&z=15&output=embed";
                this.googlemap.Attributes.Add("src", sMapSource);

                this.WHCity.InnerHtml = "See whatz happenin in<br />" + ipInfo.city + "!";
            }
            catch (Exception ex)
            {
                logException(ex);
            }
        }

        private void GetWeather(IPInfo ipInfo)
        {
            try
            {
                string sHeaderColor = "TileHeaderBlue";
                string sTileHeight = "whTileHeightSmall";
                double dSeconds = 3.0;
                XPathDocument xDoc = new XPathDocument("http://xml.weather.yahoo.com/forecastrss?p=" + ipInfo.postal);
                XPathNavigator xNavigator;
                XmlNamespaceManager xNameSpace;
                XPathNodeIterator xNodes;
                XPathNavigator xNode;
                

                xNavigator = xDoc.CreateNavigator();

                xNameSpace = new XmlNamespaceManager(xNavigator.NameTable);
                xNameSpace.AddNamespace("yweather", "http://xml.weather.yahoo.com/ns/rss/1.0");
                xNodes = xNavigator.Select("/rss/channel/item/title", xNameSpace);

                while (xNodes.MoveNext())
                {
                    xNode = xNodes.Current;
                    AddContentTile("weather", xNode.InnerXml.ToString(), sHeaderColor, sTileHeight);
                }

                xNodes = xNavigator.Select("/rss/channel/item/yweather:condition", xNameSpace);

                //Basic: Temp and Condition
                while (xNodes.MoveNext())
                {
                    dSeconds += 0.05;
                    xNode = xNodes.Current;
                    AddContentTile("weather", xNode.GetAttribute("temp", xNameSpace.DefaultNamespace) + "f", sHeaderColor, sTileHeight);

                    dSeconds += 0.05;
                    AddContentTile("weather", xNode.GetAttribute("text", xNameSpace.DefaultNamespace), sHeaderColor, sTileHeight);
                }

                //Sunrise and Sunset
                xNodes = xNavigator.Select("/rss/channel/yweather:astronomy", xNameSpace);
                while(xNodes.MoveNext())
                {
                    xNode = xNodes.Current;
                    AddContentTile("weather", "Sunrise: " + xNode.GetAttribute("sunrise", xNameSpace.DefaultNamespace), sHeaderColor, sTileHeight);

                    AddContentTile("weather", "Sunset: " + xNode.GetAttribute("sunset", xNameSpace.DefaultNamespace), sHeaderColor, sTileHeight);
                }

                //extended forcast
                xNodes = xNavigator.Select("/rss/channel/item/yweather:forecast", xNameSpace);
                while (xNodes.MoveNext())
                {
                    dSeconds += 0.05;
                    xNode = xNodes.Current;
                    AddContentTile("weather", xNode.GetAttribute("day", xNameSpace.DefaultNamespace) + ": " + xNode.GetAttribute("text", xNameSpace.DefaultNamespace), sHeaderColor, sTileHeight);
                }

                
            }
            catch (Exception ex)
            {
                logException(ex);
            }
        }

        private void GetTwitter(IPInfo ipInfo)
        {
            try
            {
                //Consumer Key, Consumer Secret, Token, Token Secret
                string sKeys = GetKeys("Twitter");
                string[] sLocation = ipInfo.loc.ToString().Split(',');

                
                 //ITwitter twitter = new TwitterTemplate(consumerKey, consumerSecret, accessToken, accessTokenSecret);



                //IList<Place> results = twitter.GeoOperations.SearchAsync(Convert.ToDouble(sLocation[0]), Convert.ToDouble(sLocation[1])).Result;

                //IList<Place> RETURNS = twitter.GeoOperations.ReverseGeoCodeAsync(Convert.ToDouble(sLocation[0]), Convert.ToDouble(sLocation[1]), null, "10m").Result;

                //System.Threading.Tasks.Task<SearchResults> searchResults = twitter.SearchOperations.SearchAsync("#NYC");

                //var searchResults = twitter.SearchOperations.SearchAsync("#NYC", 10, 20);

                //var returnIT = searchResults.Wait(5000);
                    
                
                //-----------------------------------------------
                Image WeatherIcon = new Image();
                WeatherIcon.CssClass = "HeaderIcon";
                WeatherIcon.ImageUrl = "images/" + "twitter" + ".png";

            }
            catch (Exception ex)
            {
                logException(ex);
            }
        }

        private void GetWikipedia(IPInfo ipInfo)
        {
            try
            {
                string sHeaderColor = "TileHeaderBlue";
                string sTileHeight = "whTileHeightSmall";
                string[] sLocation = ipInfo.loc.ToString().Split(',');
                string sURL = "https://en.wikipedia.org/w/api.php?action=query&list=geosearch&gsradius=10000&gscoord=" + sLocation[0] + "%7c" + sLocation[1] + "&format=json";


                using (WebClient client = new WebClient())
                {
                    var json = client.DownloadString(sURL);
                    WikipediaObject Articles = JsonConvert.DeserializeObject<WikipediaObject>(json);

                    foreach(GeoSearchResult Article in Articles.Query.GeoSearch)
                    {
                        AddContentTile("Wikipedia", Article.Title + " " + "http://en.wikipedia.org/?curid=" + Article.PageId, sHeaderColor, sTileHeight);
                    }
                }
            }
            catch (Exception ex)
            {
                logException(ex);
            }
        }

        private void GetYelp(IPInfo ipInfo)
        {
            try
            {
                //Consumer Key, Consumer Secret, Token, Token Secret	
                string sHeaderColor = "TileHeaderRed";
                string sTileHeight = "whTileHeightSmall";
                string sTerm = "food";
                string sKeys = GetKeys("Yelp");
                string[] KeyList = sKeys.Split(',');
                string sLocation = ipInfo.city + ", " + ipInfo.region;


                YelpAPIClient YelpClient = new YelpAPIClient(KeyList[0], KeyList[1], KeyList[2], KeyList[3]);

                JObject YelpResponse = YelpClient.Search(sTerm, sLocation);

                JArray Businesses = (JArray)YelpResponse.GetValue("businesses");

                foreach(var Business in Businesses)
                {
                    var YelpBusiness = (YelpAPIClient.Business)Business.ToObject(typeof(YelpAPIClient.Business));

                    AddContentTile("Yelp", 
                        YelpBusiness.name + " " +
                        "Rating: " + YelpBusiness.rated + ".  " + 
                        "Number of ratings: " + YelpBusiness.review_count + ".  " + 
                        "Details: " + YelpBusiness.mobile_url, 
                        sHeaderColor, sTileHeight);
                }
            }
            catch (Exception ex)
            {
                logException(ex);
            }
        }


        private void AddContentTile(
            string Icon, 
            string Body, 
            string HeaderColor, 
            string TileHeight)
        {
            try
            {
                HtmlGenericControl WHTileDIV = new HtmlGenericControl("div");
                WHTileDIV.Attributes["class"] = "whTile animated fadeIn " + TileHeight;
                WHTileDIV.ID = "WHTile_" + NewGuid();

                HtmlGenericControl Border3DIV = new HtmlGenericControl("div");
                Border3DIV.Attributes["class"] = "border3";
                Border3DIV.ID = "Border3_" + NewGuid();
                WHTileDIV.Controls.Add(Border3DIV);

                //HtmlGenericControl TileHeaderDIV = new HtmlGenericControl("div");
                //TileHeaderDIV.Attributes["class"] = "TileHeader " + HeaderColor;
                //TileHeaderDIV.ID = "TileHeader_" + NewGuid();
                //Border3DIV.Controls.Add(TileHeaderDIV);

                HtmlGenericControl IconParagraph = new HtmlGenericControl("p");
                IconParagraph.Attributes["class"] = "Icon";
                IconParagraph.ID = "IconParagraph" + NewGuid();
                IconParagraph.InnerHtml = "<img class=\"HeaderIcon\" src=\"images/" + Icon + ".png\" style=\"border-width:0px;\" />";
                //TileHeaderDIV.Controls.Add(IconParagraph);
                WHTileDIV.Controls.Add(IconParagraph);
                
                HtmlGenericControl BodyParagraph = new HtmlGenericControl("p");
                BodyParagraph.Attributes["class"] = "center";
                BodyParagraph.ID = "BodyParagraph_" + NewGuid();
                BodyParagraph.InnerText = Body;
                WHTileDIV.Controls.Add(BodyParagraph);

                this.WHTiles.Controls.Add(WHTileDIV);
            }
            catch (Exception ex)
            {
                logException(ex);
            }
        }

        private string NewGuid()
        {
            return Guid.NewGuid().ToString().Replace("-", "");
        }

        private void logException(Exception ex)
        {
            try
            {
                var st = new StackTrace(ex, true);
                var frame = st.GetFrame(0);
                var line = st.GetFrame(0).GetFileLineNumber();

                //Some .net internals are erroring but not returning a line number
                //if (line > 0)
                //{
                    MethodBase site = ex.TargetSite;
                    string sMethodName = site == null ? null : site.Name;


                    Console.WriteLine("------------------");
                    Console.WriteLine(DateTime.Now.ToString());
                    Console.WriteLine("Line: " + line.ToString());
                    Console.WriteLine("Method: " + sMethodName);
                    Console.WriteLine("Exception: " + ex.Message);
                    Console.Write(ex.StackTrace.ToString());
                    Console.WriteLine("");
                
                //} //line > 0
            }
            catch { }
        }
    }
}