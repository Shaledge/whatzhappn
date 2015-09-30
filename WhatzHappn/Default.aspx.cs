using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Xml;
using System.Xml.XPath;

namespace WhatzHappn
{
    public partial class _Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                string sIPAddress = GetIPAddress();
                string sZIPCode = "10001";

                if (IsValidIP(sIPAddress) == true)
                {
                    GetWeather(sZIPCode);
                }
                else
                {
                    //Inform user we cannot get location.
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

            return true; // bReturn; //TEMP TODO: remove for production
        }

        private void GetWeather(string ZIPCode)
        {
            try
            {
                string sTitle = "";
                XPathDocument xDoc = new XPathDocument("http://xml.weather.yahoo.com/forecastrss?p=" + ZIPCode);
                XPathNavigator xNavigator;
                XmlNamespaceManager xNameSpace;
                XPathNodeIterator xNodes;
                XPathNavigator xNode;
                
                Image WeatherIcon = new Image();
                WeatherIcon.CssClass = "ButtonIcon";
                WeatherIcon.ImageUrl = "images/" + "weather" + ".png";
                
                HtmlGenericControl SectionDIV = new HtmlGenericControl("div");
                SectionDIV.Attributes["class"] = "header";
                SectionDIV.ID = "Weather";
                //SectionDIV.di
                HtmlGenericControl ContentDIV = new HtmlGenericControl("div");
                SectionDIV.Attributes["class"] = "content";
                SectionDIV.ID = "WeatherContent";

                SectionDIV.Controls.Add(ContentDIV);


                SectionDIV.Style.Add("display", "block");

                xNavigator = xDoc.CreateNavigator();

                xNameSpace = new XmlNamespaceManager(xNavigator.NameTable);
                xNameSpace.AddNamespace("yweather", "http://xml.weather.yahoo.com/ns/rss/1.0");
                xNodes = xNavigator.Select("/rss/channel/item/title", xNameSpace);

                while (xNodes.MoveNext())
                {
                    xNode = xNodes.Current;
                    //sTitle = xNode.InnerXml.ToString();

                    AddContentTile(SectionDIV, WeatherIcon, "", xNode.InnerXml.ToString());
                }

                //TEMP
                this.WHBody.Controls.Add(SectionDIV);






                xNodes = xNavigator.Select("/rss/channel/item/yweather:condition", xNameSpace);

                //Basic: Temp and Condition
                while (xNodes.MoveNext())
                {
                    xNode = xNodes.Current;
                    sTitle = xNode.GetAttribute("temp", xNameSpace.DefaultNamespace) + "f";

                    sTitle = xNode.GetAttribute("text", xNameSpace.DefaultNamespace);
                }


                //Sunrise and Sunset
                xNodes = xNavigator.Select("/rss/channel/yweather:astronomy", xNameSpace);
                while(xNodes.MoveNext())
                {
                    xNode = xNodes.Current;
                    sTitle = "Sunrise: " + xNode.GetAttribute("sunrise", xNameSpace.DefaultNamespace);

                    sTitle = "Sunset: " + xNode.GetAttribute("sunset", xNameSpace.DefaultNamespace);
                }

                //extended forcast
                xNodes = xNavigator.Select("/rss/channel/item/yweather:forecast", xNameSpace);
                while (xNodes.MoveNext())
                {
                    xNode = xNodes.Current;
                    sTitle = xNode.GetAttribute("day", xNameSpace.DefaultNamespace) + ": " + xNode.GetAttribute("text", xNameSpace.DefaultNamespace);

                }
            }
            catch (Exception ex)
            {
                logException(ex);
            }
        }

        private void AddContentTile(HtmlGenericControl ParentDIV, Image Icon, string Title, string Body)
        {
            try
            {
                HtmlGenericControl WHTileDIV = new HtmlGenericControl("div");
                WHTileDIV.Attributes["class"] = "whTile";
                WHTileDIV.ID = "ContentTile" + NewGuid();

                HtmlGenericControl Border3DIV = new HtmlGenericControl("div");
                Border3DIV.Attributes["class"] = "border3";
                Border3DIV.ID = "Border3" + NewGuid();

                WHTileDIV.Controls.Add(Border3DIV);

                HtmlGenericControl Border2DIV = new HtmlGenericControl("div");
                Border2DIV.Attributes["class"] = "border3";
                Border2DIV.ID = "Border3" + NewGuid();

                Border3DIV.Controls.Add(Border2DIV);

                if (Icon != null)
                {
                    HtmlGenericControl IconParagraph = new HtmlGenericControl("p");
                    IconParagraph.Attributes["class"] = "monopolyLogo";
                    IconParagraph.ID = "IconParagraph" + NewGuid();

                    IconParagraph.Controls.Add(Icon);
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
                BodyParagraph.ID = "BodyParagraph" + NewGuid();
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