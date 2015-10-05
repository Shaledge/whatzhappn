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

//Twitter



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

    public partial class _Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                this.Title = "Whatz Happn: Version 0.007";
                string sIPAddress = GetIPAddress();
                sIPAddress = "71.125.17.29";  //TEMP for development
                

                if (IsValidIP(sIPAddress) == true)
                {
                    IPInfo ipInfo = GetIPInfo(sIPAddress);

                    GetMap(ipInfo);

                    GetWeather(ipInfo);

                    GetTwitter(ipInfo);

                    GetWikipedia(ipInfo);
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
            string URL = "http://ipinfo.io/" + IPAddress;
            using (WebClient client = new WebClient())
            {
                string json = client.DownloadString(URL);
                IPInfo ipinfo = new JavaScriptSerializer().Deserialize<IPInfo>(json);
                return ipinfo;
            }
        }

        private string Getkeys(string Name)
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
                string sMapSource = "http://maps.google.com?q=" + sLocation[0] + "," + sLocation[1] + "&z=15&output=embed";
                this.googlemap.Attributes.Add("src", sMapSource);
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
                
                Image WeatherIcon = new Image();
                WeatherIcon.CssClass = "HeaderIcon";
                WeatherIcon.ImageUrl = "images/" + "weather" + ".png";
                
                HtmlGenericControl SectionDIV = new HtmlGenericControl("div");
                SectionDIV.Attributes["class"] = "header";
                SectionDIV.ID = "WeatherContent";
                

                xNavigator = xDoc.CreateNavigator();

                xNameSpace = new XmlNamespaceManager(xNavigator.NameTable);
                xNameSpace.AddNamespace("yweather", "http://xml.weather.yahoo.com/ns/rss/1.0");
                xNodes = xNavigator.Select("/rss/channel/item/title", xNameSpace);

                while (xNodes.MoveNext())
                {
                    xNode = xNodes.Current;

                    AddContentTile(SectionDIV, WeatherIcon, "", xNode.InnerXml.ToString(), sHeaderColor, sTileHeight);
                }

                xNodes = xNavigator.Select("/rss/channel/item/yweather:condition", xNameSpace);

                //Basic: Temp and Condition
                while (xNodes.MoveNext())
                {
                    dSeconds += 0.05;
                    xNode = xNodes.Current;
                    AddContentTile(SectionDIV, WeatherIcon, "", xNode.GetAttribute("temp", xNameSpace.DefaultNamespace) + "f", sHeaderColor, sTileHeight);

                    dSeconds += 0.05;
                    AddContentTile(SectionDIV, WeatherIcon, "", xNode.GetAttribute("text", xNameSpace.DefaultNamespace), sHeaderColor, sTileHeight);
                }


                //Sunrise and Sunset
                xNodes = xNavigator.Select("/rss/channel/yweather:astronomy", xNameSpace);
                while(xNodes.MoveNext())
                {
                    xNode = xNodes.Current;
                    AddContentTile(SectionDIV, WeatherIcon, "", "Sunrise: " + xNode.GetAttribute("sunrise", xNameSpace.DefaultNamespace), sHeaderColor, sTileHeight);

                    AddContentTile(SectionDIV, WeatherIcon, "", "Sunset: " + xNode.GetAttribute("sunset", xNameSpace.DefaultNamespace), sHeaderColor, sTileHeight);
                }


                //extended forcast
                xNodes = xNavigator.Select("/rss/channel/item/yweather:forecast", xNameSpace);
                while (xNodes.MoveNext())
                {
                    dSeconds += 0.05;
                    xNode = xNodes.Current;
                    AddContentTile(SectionDIV, WeatherIcon, "", xNode.GetAttribute("day", xNameSpace.DefaultNamespace) + ": " + xNode.GetAttribute("text", xNameSpace.DefaultNamespace), sHeaderColor, sTileHeight);
                }

                //Add everything to the body
                this.WHBody.Controls.Add(SectionDIV);
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
                string sKeys = Getkeys("Twitter");

                string consumerKey = "..."; // The application's consumer key
                string consumerSecret = "..."; // The application's consumer secret
                string accessToken = "..."; // The access token granted after OAuth authorization
                string accessTokenSecret = "..."; // The access token secret granted after OAuth authorization
               /* ITwitter twitter = new TwitterTemplate(consumerKey, consumerSecret, accessToken, accessTokenSecret);*/

                //ISearchOperations search = new ISearchOperations();


      

                Image WeatherIcon = new Image();
                WeatherIcon.CssClass = "HeaderIcon";
                WeatherIcon.ImageUrl = "images/" + "twitter" + ".png";

                HtmlGenericControl SectionDIV = new HtmlGenericControl("div");
                SectionDIV.Attributes["class"] = "header";
                SectionDIV.ID = "TwitterContent";




                //Add everything to the body
                this.WHBody.Controls.Add(SectionDIV);
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
                string[] sLocation = ipInfo.loc.ToString().Split(',');
                string sURL = "https://en.wikipedia.org/w/api.php?action=query&list=geosearch&gsradius=10000&gscoord=" + sLocation[0] + "%7c" + sLocation[1] + "&format=json";

                

            }
            catch (Exception ex)
            {
                logException(ex);
            }
        }

        private void AddContentTile(
            HtmlGenericControl ParentDIV, 
            Image Icon, 
            string Title, 
            string Body, 
            string HeaderColor, 
            string TileHeight)
        {
            try
            {
                //,            double Seconds
                HtmlGenericControl WHTileDIV = new HtmlGenericControl("div");
                WHTileDIV.Attributes["class"] = "whTile animated fadeIn " + TileHeight;
                //WHTileDIV.Attributes["class"].Parameter.Value = Seconds

                WHTileDIV.ID = "ContentTile_" + NewGuid();

                HtmlGenericControl Border3DIV = new HtmlGenericControl("div");
                Border3DIV.Attributes["class"] = "border3";
                Border3DIV.ID = "Border3_" + NewGuid();

                WHTileDIV.Controls.Add(Border3DIV);

                HtmlGenericControl Border2DIV = new HtmlGenericControl("div");
                Border2DIV.Attributes["class"] = "TileHeader " + HeaderColor;
                Border2DIV.ID = "TileHeader_" + NewGuid();

                Border3DIV.Controls.Add(Border2DIV);

                if (Icon != null)
                {
                    HtmlGenericControl IconParagraph = new HtmlGenericControl("p");
                    IconParagraph.Attributes["class"] = "Icon";
                    IconParagraph.ID = "IconParagraph" + NewGuid();

                    IconParagraph.Controls.Add(Icon);
                    Border2DIV.Controls.Add(IconParagraph);
                }
                else
                {
                    HtmlGenericControl TitleParagraph = new HtmlGenericControl("p");
                    TitleParagraph.Attributes["class"] = "center";
                    TitleParagraph.ID = "IconParagraph" + NewGuid();
                    TitleParagraph.InnerText = Title;

                    Border2DIV.Controls.Add(TitleParagraph);
                }

                HtmlGenericControl BodyParagraph = new HtmlGenericControl("p");
                BodyParagraph.Attributes["class"] = "center";
                BodyParagraph.ID = "BodyParagraph_" + NewGuid();
                BodyParagraph.InnerText = Body;

                WHTileDIV.Controls.Add(BodyParagraph);

                ParentDIV.Controls.Add(WHTileDIV);
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
            Console.WriteLine(ex.Message);
        }
    }
}