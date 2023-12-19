using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using NuGet.Protocol.Plugins;
using System.Data;
using System.Diagnostics;

namespace THC_CHURCH_WEBSITE.Pages
{
    public class AboutModel : PageModel
    {
        [TempData]
        public string StatusMessage { get; set; }
        private readonly string str2 = "Server=tcp:progpartst10090552.database.windows.net,1433;Initial Catalog=thcchurchgroup;Persist Security Info=False;User ID=momoessop;Password=Mobile22;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
        public void OnGet()
        {
        }

       
    }
}
