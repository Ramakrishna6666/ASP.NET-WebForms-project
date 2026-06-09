using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace FIlms
{
    /// <summary>
    /// Register Page - Web Forms
    /// Cloud Migration Note: This Web Forms page should be migrated to ASP.NET Core Razor Pages
    /// for optimal cloud deployment on Azure Container Apps with Linux containers.
    /// Razor Pages provide similar page-based model without ViewState overhead.
    /// Consider using ASP.NET Core Identity for user registration in cloud environments.
    /// </summary>
    public partial class Register : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Cloud-ready: Ensure stateless operation for horizontal scaling
        }

        protected void Button1_Click(object sender, EventArgs e)
        {

        }

        protected void Calendar1_SelectionChanged(object sender, EventArgs e)
        {

        }

        protected void calDate_SelectionChanged(object sender, EventArgs e)
        {

        }

       

       
    }
}
