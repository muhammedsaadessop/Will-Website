using Google.Cloud.Firestore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Diagnostics;

namespace THC_CHURCH_WEBSITE.Areas.Identity.Pages.Account.Manage
{
    public class syncModel : PageModel
    {
        private readonly string? str2 = "Server=tcp:progpartst10090552.database.windows.net,1433;Initial Catalog=thcchurchgroup;Persist Security Info=False;User ID=momoessop;Password=Mobile22;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
        [TempData]
        public string StatusMessage { get; set; }
        public void OnGet()
        {
        }
        public async Task OnGetProfile_Click(object sender, EventArgs e)
        {
           StatusMessage = "PASTOR PROFILE SYNCED";
            Debug.WriteLine("CALLED");
            string message = User.Identity.Name;
          var  Email = message?.ToString();

            string[] parts = Email.Split(new char[] { '@', '.' });
            string result = parts[0] + parts[1];
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", "thchurch-661c7-firebase-adminsdk-bfoyu-7460d48e93.json");
            FirestoreDb db = FirestoreDb.Create("thchurch-661c7");
            DocumentReference docRef = db.Collection("pastors").Document(result.ToLower());
            DocumentSnapshot snap = await docRef.GetSnapshotAsync();

            if (snap.Exists)
            {
                Dictionary<string, object> userDetails = snap.GetValue<Dictionary<string, object>>("userDetails");

                SqlConnection conn = new SqlConnection(str2);
                conn.Open();
                var firstname = userDetails["firstname"];
                var surname = userDetails["surname"];
                var worhshipname = userDetails["church"];
                var phone = userDetails["phone"];
                var size = userDetails["centersize"];
                var country = userDetails["country"];

                string updateprofile = $"UPDATE Pastors SET  PastorName = @PastorName, Psurname = @surname, WorshipName = @WorshipName, Pnumber = @Pnumber, CenterSize = @CenterSize, Country = @place WHERE Pid = '{result}'";



                SqlCommand firstcaputure = new SqlCommand(updateprofile, conn);
                firstcaputure.Parameters.Add(@"PastorName", SqlDbType.VarChar, 100).Value = firstname;
                firstcaputure.Parameters.Add(@"surname", SqlDbType.VarChar, 100).Value = surname;
                firstcaputure.Parameters.Add(@"WorshipName", SqlDbType.VarChar, 100).Value = worhshipname;
                firstcaputure.Parameters.Add(@"Pnumber", SqlDbType.VarChar, 100).Value = phone;
                firstcaputure.Parameters.Add(@"CenterSize", SqlDbType.VarChar, 100).Value = size;
                firstcaputure.Parameters.Add(@"place", SqlDbType.VarChar, 100).Value = country;
                firstcaputure.ExecuteScalar();

                firstcaputure.ExecuteScalar();

            }

        }
        

    }
}
