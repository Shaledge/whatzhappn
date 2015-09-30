using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WhatzHappn
{
    public partial class _Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (IsValidIP(GetIPAddress()) == true)
                {
                    //TODO: get info  
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

        private string GetIPAddress()
        {
            System.Web.HttpContext context = System.Web.HttpContext.Current;
            string ipAddress = context.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

            if (!string.IsNullOrEmpty(ipAddress))
            {
                string[] addresses = ipAddress.Split(',');
                if (addresses.Length != 0)
                {
                    return addresses[0];
                }
            }

            return context.Request.ServerVariables["REMOTE_ADDR"];
        }

        private void logException(Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
}