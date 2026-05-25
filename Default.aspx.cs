using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace FIlms
{
    public partial class _Default : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Example: If file paths were needed, use ConfigurationHelper instead of hardcoded paths
            // BEFORE (hardcoded - causes containerization blocker):
            // string dataPath = "C:\\Data\\Files";
            
            // AFTER (externalized configuration - containerization-ready):
            // string dataPath = ConfigurationHelper.GetFilePath("DataFilePath", "/app/data/files");
            
            // This ensures paths work in Linux containers and can be configured via environment variables
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            Response.Redirect("https://moodle.unwe.bg");
        }
    }
}
