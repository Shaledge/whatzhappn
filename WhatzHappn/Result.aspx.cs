using System;
using System.Collections.Generic;

using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WhatzHappn
{
    public partial class Result : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {

            }
            catch (Exception ex)
            {
                logException(ex);
            }
        }

        private void logException(Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
}