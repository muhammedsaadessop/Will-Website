// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.Common;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Google.Cloud.Firestore;
using Google.Type;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;

namespace THC_CHURCH_WEBSITE.Areas.Identity.Pages.Account.Manage
{
    public class IndexModel : PageModel
    {
        private readonly string? str2 = "Server=tcp:progpartst10090552.database.windows.net,1433;Initial Catalog=thcchurchgroup;Persist Security Info=False;User ID=momoessop;Password=Mobile22;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
  
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        public string churchids { get; set; }
        public string churchnames { get; set; }
        public IndexModel(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [TempData]
        public string StatusMessage { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class InputModel
        {
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Phone]
            [Display(Name = "Phone number")]
            public string PhoneNumber { get; set; }

            [Display(Name = "First Name")]
            public string FirstName { get; set; }

            [Display(Name = "Surname")]
            public string Surname { get; set; }

            [Display(Name = "Center Size")]
            public string Centerssize { get; set; }

            [Display(Name = "Worship Name")]
            public string Worshipname { get; set; }
            [Display(Name = "Country")]
            public string Country { get; set; }
            [Display(Name = "Church ID")]
            public string ChurchID { get; set; }
        }

        private async Task LoadAsync(IdentityUser user)
        {
            var userName = await _userManager.GetUserNameAsync(user);
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
          
            Username = userName;
            string Email;
            SqlConnection conn = new SqlConnection(str2);
            conn.Open();
            string message = User.Identity.Name;
            Email = message?.ToString();

            string[] parts = Email.Split(new char[] { '@', '.' });
            string result = parts[0] + parts[1];
            string selectquery4 = $"Select * from Pastors where Pid ='{result}';";
            SqlCommand mysql5 = new SqlCommand(selectquery4, conn);
            SqlDataReader reader4 = mysql5.ExecuteReader();
            reader4.Read();
            if (reader4.HasRows.Equals(true))
            {
                Input = new InputModel
                {
                    Worshipname = reader4["WorshipName"].ToString(),
                    FirstName = reader4["PastorName"].ToString(),
                    Surname = reader4["Psurname"].ToString(),
                    Centerssize = reader4["CenterSize"].ToString(),
                    Country = reader4["Country"].ToString(),
                    ChurchID = reader4["churchid"].ToString(),
                    PhoneNumber = phoneNumber
                };
                reader4.Close();
            }
            else
            {
                Input = new InputModel
                {
                   
                    PhoneNumber = phoneNumber
                };
            }
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }
         
            await LoadAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }

            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            Profile();
            if (Input.PhoneNumber != phoneNumber)
            {
                var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, Input.PhoneNumber);
                if (!setPhoneResult.Succeeded)
                {
                    StatusMessage = "Unexpected error when trying to set phone number.";
                    return RedirectToPage();
                }
            }
         
            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Your profile has been updated";
            
            return RedirectToPage();
        }

        public void ids()
        {
            // rest of the method code
            SqlConnection conn = new SqlConnection(str2);
            conn.Open();
            string selectquery2 = $"SELECT churchname FROM Churchs where churchid ='{churchids}'";
            SqlCommand mysql2 = new SqlCommand(selectquery2, conn);

            SqlDataReader reader4 = mysql2.ExecuteReader();
            reader4.Read();
            if (reader4.HasRows.Equals(true))
            {
                churchnames = reader4["churchname"].ToString();
            }
            reader4.Close();


        }
        public async void Profile()
        {
           churchids = Input.ChurchID.ToString().Trim();
            string Email;
            SqlConnection conn = new SqlConnection(str2);
            conn.Open();
            string message = User.Identity.Name;
            Email = message?.ToString();
            ids();
            string[] parts = Email.Split(new char[] { '@', '.' });
            string result = parts[0] + parts[1];
            string selectquery4 = $"Select Pid from Pastors where Pid ='{result}';";
            SqlCommand mysql5 = new SqlCommand(selectquery4, conn);
            SqlDataReader reader4 = mysql5.ExecuteReader();
            reader4.Read();
            if (reader4.HasRows.Equals(false))
            {
                reader4.Close();
               
                string saveprofile = "INSERT INTO Pastors (Pid, churchid, PastorName,Psurname, WorshipName, Email, Pnumber, CenterSize,Country) VALUES (@Pid, @churchid, @PastorName, @surname,@WorshipName, @Email, @Pnumber, @CenterSize,@place)";
                SqlCommand firstcaputure = new SqlCommand(saveprofile, conn);
                firstcaputure.Parameters.Add(@"Pid", SqlDbType.VarChar, 100).Value = result.ToString().Trim();
                firstcaputure.Parameters.Add(@"churchid", SqlDbType.VarChar, 100).Value = Input.ChurchID.ToString().Trim();
                firstcaputure.Parameters.Add(@"PastorName", SqlDbType.VarChar, 100).Value = Input.FirstName.ToString().Trim();
                firstcaputure.Parameters.Add(@"surname", SqlDbType.VarChar, 100).Value = Input.Surname.ToString().Trim();
                firstcaputure.Parameters.Add(@"WorshipName", SqlDbType.VarChar, 100).Value = churchnames.ToString();   
                firstcaputure.Parameters.Add(@"Email", SqlDbType.VarChar, 100).Value = Email.ToString().Trim();
                firstcaputure.Parameters.Add(@"Pnumber", SqlDbType.VarChar, 100).Value = Input.PhoneNumber.ToString().Trim();
                firstcaputure.Parameters.Add(@"CenterSize", SqlDbType.VarChar, 100).Value = Input.Centerssize.ToString().Trim();
                firstcaputure.Parameters.Add(@"place", SqlDbType.VarChar, 100).Value = Input.Country.ToString().Trim();
                firstcaputure.ExecuteScalar();

             

                    Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", "thchurch-661c7-firebase-adminsdk-bfoyu-7460d48e93.json");

                    
                    FirestoreDb db = FirestoreDb.Create("thchurch-661c7");

                    var userId = result;

                    
                    DocumentReference docRef = db.Collection("pastors").Document(userId.ToLower());


                await docRef.SetAsync(new { userDetails = new { firstname = Input.FirstName, surname = Input.Surname, phone = Input.PhoneNumber, email = Email, centersize = Input.Centerssize, church = churchnames, country = Input.Country, churchid = Input.ChurchID ,admin ="True"} }, SetOptions.MergeAll);





            }
            else
            {
                reader4.Close();
                Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", "thchurch-661c7-firebase-adminsdk-bfoyu-7460d48e93.json");
                FirestoreDb db = FirestoreDb.Create("thchurch-661c7");

                var userId = result;


                DocumentReference docRef = db.Collection("pastors").Document(userId.ToLower());

                await docRef.SetAsync(new { userDetails = new { firstname = Input.FirstName, surname = Input.Surname, phone = Input.PhoneNumber, email = Email, centersize = Input.Centerssize, church = churchnames ,country = Input.Country,churchid = Input.ChurchID,admin = "true"} }, SetOptions.MergeAll);
      
                string updateprofile = $"UPDATE Pastors SET  PastorName = @PastorName, Psurname = @surname, WorshipName = @WorshipName, Email = @Email, Pnumber = @Pnumber, CenterSize = @CenterSize, Country = @place WHERE Pid = '{result}'";
    
             

                SqlCommand firstcaputure = new SqlCommand(updateprofile, conn);
                firstcaputure.Parameters.Add(@"PastorName", SqlDbType.VarChar, 100).Value = Input.FirstName.ToString().Trim();
                firstcaputure.Parameters.Add(@"surname", SqlDbType.VarChar, 100).Value = Input.Surname.ToString().Trim();
                firstcaputure.Parameters.Add(@"WorshipName", SqlDbType.VarChar, 100).Value = churchnames.ToString().Trim();
                firstcaputure.Parameters.Add(@"Email", SqlDbType.VarChar, 100).Value = Email.ToString().Trim();
                firstcaputure.Parameters.Add(@"Pnumber", SqlDbType.VarChar, 100).Value = Input.PhoneNumber.ToString().Trim();
                firstcaputure.Parameters.Add(@"CenterSize", SqlDbType.VarChar, 100).Value = Input.Centerssize.ToString().Trim();
                firstcaputure.Parameters.Add(@"place", SqlDbType.VarChar, 100).Value = Input.Country.ToString().Trim();
                firstcaputure.ExecuteScalar();

                firstcaputure.ExecuteScalar();
            }

        }
    }
}
