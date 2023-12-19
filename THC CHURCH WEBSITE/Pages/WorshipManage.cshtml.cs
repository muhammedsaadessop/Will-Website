using Google.Type;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using MigraDoc.DocumentObjectModel.Tables;
using MigraDoc.DocumentObjectModel;
using MigraDoc.Rendering;

using PdfSharp.Drawing;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.Xml.Linq;
using MigraDocTable = MigraDoc.DocumentObjectModel.Tables.Table;
using MigraDocColumn = MigraDoc.DocumentObjectModel.Tables.Column;
using PdfSharp.Fonts;
using Google.Cloud.Firestore;
using NuGet.Protocol.Plugins;
using PdfSharp.Pdf.Content.Objects;
using Firebase.Auth;
using static Azure.Core.HttpHeader;

namespace THC_CHURCH_WEBSITE.Pages
{ 
    public class WorshipManage : PageModel
    {//connection string do not touch 
        // this code is done by muhammed saad essop st10090552 if theres any isues please contact him first
        // ensure front end names are easy to find as they have to be typed manually here
        private readonly string str2 = "Server=tcp:progpartst10090552.database.windows.net,1433;Initial Catalog=thcchurchgroup;Persist Security Info=False;User ID=momoessop;Password=Mobile22;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
        public List<List<string>> prodRows { get; set; }
        // these variables allow for use in the backend to write and read data reading only methods and variables have been made private 
        public string? Message { get; set; }
        public string churchids { get; set; }
        public string churchnames {  get; set; }
        public string test { get; set; }
        public string? phonenumber { get; set; }
        public string Error { get; set; }
        public string TableTitle { get; set; }
        public string? email { get; set; }
        public string? country { get; set; }
        public string? surname { get; set; }
        public string? centersize { get; set; }
        public string? ProfileId { get; set; }
        public string Worshipname { get; set; }
        public string pcount { get; set; }
        public string cCount { get; set; }
        public string? Firstname { get; set; }
        [TempData]
        public string StatusMessage { get; set; }
        public string Email { get; set; }
        public void OnGet()
        {
            if (User.Identity.Name.IsNullOrEmpty().Equals(true))
            {
                Response.Redirect("/Index");
            }
            else
            {
                if (User.Identity.Name.IsNullOrEmpty().Equals(true))
                {
                    Response.Redirect("/Index");
                }
                else
                {
                    tablepopulate();
                  
                }
            }

        }

        public bool Emailchecker(string email)
        {
            try
            {
                var format = new System.Net.Mail.MailAddress(email);
                return true;
            }
            catch
            {
                return false;
            }
        }
        //populates the tables as per default data state
        private void tablepopulate()
        {

            prodRows = new List<List<string>>();

         
            SqlConnection conn = new SqlConnection(str2);
            conn.Open();
     



            string selectquery4 = "SELECT Pid, PastorName,Psurname, Pnumber,Email, CenterSize, Country, WorshipName FROM Pastors";

            SqlCommand mysql5 = new SqlCommand(selectquery4, conn);
            SqlDataReader reader4 = mysql5.ExecuteReader();



            while (reader4.Read())
            {
                List<string> row = new List<string>();
                for (int i = 0; i < reader4.FieldCount; i++)
                {
                    row.Add(reader4[i].ToString());
                }
                prodRows.Add(row);
            }
            conn.Close();


            reader4.Close();
            // column headings 
            ProfileId = "ID";
            phonenumber = "Pastors Number";
            email = " Email Adress";
            surname = "surname";
            Firstname = "Firstname";
            centersize = "Center Size";
            TableTitle = "Country";
            country = "Country";
            Worshipname = "Worhsip Center";
            counterload();

        }

        private void counterload()
        {
            int count = 0;
            using (SqlConnection connection = new SqlConnection(str2))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("SELECT COUNT(*) FROM Pastors", connection))
                {
                    count = (int)command.ExecuteScalar();
                    pcount = count.ToString();
                }
            }

            int count2 = 0;
            using (SqlConnection connection = new SqlConnection(str2))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("SELECT COUNT(*) FROM Churchs", connection))
                {
                    count2 = (int)command.ExecuteScalar();
                    cCount = count2.ToString();
                }
            }

        }
        
        public void OnGetFilter_Click(object sender, EventArgs e)
        {
            ProfileId = "ID";
            phonenumber = "Pastors Number";
            email = " Email Adress";
            surname = "surname";
            Firstname = "Firstname";
            centersize = "Center Size";
            TableTitle = "Country";
            country = "Country";
            Worshipname = "Worhsip Center";
            string entry = Request.Query["filters"];
            string spinner = Request.Query["filterspin"];
            prodRows = new List<List<string>>();
            Debug.WriteLine("MyMethod was called");
            // rest of the method code
            SqlConnection conn = new SqlConnection(str2);
              if(entry.IsNullOrEmpty().Equals(true) && spinner.IsNullOrEmpty().Equals(true))
            {
                tablepopulate();
            }
           else if (entry.IsNullOrEmpty().Equals(true) && spinner.IsNullOrEmpty().Equals(false))
            {
                StatusMessage = " please enter a filter value";
                tablepopulate();
            }
            else if (entry.IsNullOrEmpty().Equals(false) && spinner.IsNullOrEmpty().Equals(true))
            {
                StatusMessage = " please enter a spinner value";
                tablepopulate();
            }
       
            else

            {
                try { 
                string selectquery2 = $"SELECT Pid, PastorName,Psurname, Pnumber,Email, CenterSize, Country, WorshipName from Pastors where {spinner}  COLLATE Latin1_General_CI_AS  LIKE '%{entry}%' ";
                SqlCommand mysql2 = new SqlCommand(selectquery2, conn);
                conn.Open();
                using (SqlDataReader reader2 = mysql2.ExecuteReader())
                {
                    while (reader2.Read())
                    {
                        List<string> row = new List<string>();
                        for (int i = 0; i < reader2.FieldCount; i++)
                        {
                            row.Add(reader2[i].ToString());
                        }
                        prodRows.Add(row);
                    }
                    Debug.WriteLine(spinner);
                    Debug.WriteLine(entry);
                    Debug.WriteLine(selectquery2);
                    conn.Close();
                        counterload();
                }
            }catch(Exception ex) { Debug.WriteLine(ex.Message); }

            }


        }
        public void OnGetAdd_Click(object sender, EventArgs e)
        {

            Debug.WriteLine("MyMethod was called");

            tablepopulate();
        }
        public async void OnGetDelete_click(object sender, EventArgs e)
        {
            string Pid = Request.Query["deletentry"];


            string selectquery4 = $"Select Pid from Pastors where Pid ='{Pid}';";
            SqlConnection conn = new SqlConnection(str2);
            conn.Open();
            SqlCommand mysql5 = new SqlCommand(selectquery4, conn);
            SqlDataReader reader4 = mysql5.ExecuteReader();
            reader4.Read();
            if (reader4.HasRows.Equals(true))
            {
                reader4.Close();

                string delete = $"delete from Pastors where Pid = '{Pid}'";
                SqlCommand firstcaputure = new SqlCommand(delete, conn);

                firstcaputure.ExecuteScalar();
                tablepopulate();

                StatusMessage = $"Pastor {Pid} deleted sucessfully";

                conn.Close();
                Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", "thchurch-661c7-firebase-adminsdk-bfoyu-7460d48e93.json");
                FirestoreDb db = FirestoreDb.Create("thchurch-661c7");
                DocumentReference docRef = db.Collection("pastors").Document(Pid.ToLower());

                await docRef.DeleteAsync();
            }
            else
            {
                tablepopulate();
                StatusMessage = $"Pastor {Pid} doesnt exist or has been deleted already";
                conn.Close();
                Response.Redirect("/WorshipManage");
            }


        }

        public async void OnGetUpdate_Click(object sender, EventArgs e)
        {

            Debug.WriteLine("MyMethod was called");
            string id = Request.Query["test"];
            string pname = Request.Query["test2"];
            string psurname = Request.Query["test3"];
            string pnumber = Request.Query["test4"];
            string emails = Request.Query["test5"];
            string size = Request.Query["test6"];
            string countrys = Request.Query["test7"];
            string name = Request.Query["test8"];
         
        
            if(id.IsNullOrEmpty().Equals(true))
            {

                StatusMessage = "please enter changes before hitting save";
                tablepopulate();
            }
           else if (pname.Length < 5)
            {
                StatusMessage = "pastor Name too short";
                tablepopulate();
            }
            else if (psurname.Length < 4)
            {
                StatusMessage = "pastor surname too short";
                tablepopulate();
            }
            else if (pnumber.Length < 7)
            {
                StatusMessage = "pastor phone number too short";
                tablepopulate();
            }
            else if (name.Length < 4)
            {
                StatusMessage = "church name too short";
                tablepopulate();
            }
            else if (Emailchecker(emails).Equals(false))
            {
                StatusMessage = "please enter a valid email";
                tablepopulate();
            }
            else if (countrys.Length < 2)
            {
                StatusMessage = "please enter a valid country ";
                tablepopulate();
            }
            else
            {
                Debug.WriteLine(id);
                // Create an UPDATE command to update the data in the database
                string connectionString = str2;
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand($"UPDATE Pastors SET PastorName = @PastorName, Psurname = @surname, WorshipName = @WorshipName, Email = @Email, Pnumber = @Pnumber, CenterSize = @CenterSize, Country = @place WHERE Pid = '{id}'", connection))
                    {
                        // Set the values of the parameters in the command
                        command.Parameters.AddWithValue("@PastorName", pname);
                        command.Parameters.AddWithValue("@surname", psurname);
                        command.Parameters.AddWithValue("@WorshipName", name);
                        command.Parameters.AddWithValue("@Email", emails);
                        command.Parameters.AddWithValue("@Pnumber", pnumber);
                        command.Parameters.AddWithValue("@CenterSize", size);
                        command.Parameters.AddWithValue("@place", countrys);


                        command.ExecuteNonQuery();
                        tablepopulate();
                        StatusMessage = $"{emails} Details updated sucessfully ";
                        tablepopulate();
                        Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", "thchurch-661c7-firebase-adminsdk-bfoyu-7460d48e93.json");
                        FirestoreDb db = FirestoreDb.Create("thchurch-661c7");
                        DocumentReference docRef = db.Collection("pastors").Document(id.ToLower()); 


                        await docRef.SetAsync(new { userDetails = new { firstname = pname, surname = psurname, phone = pnumber, email = emails, centersize = size, church = name, country = countrys, churchid = id,admin = "false" } }, SetOptions.MergeAll);
                    }
                }
            }
        }

        public void OnGetSortId_Click(object sender, EventArgs e)
        {
            ProfileId = "ID";
            phonenumber = "Pastors Number";
            email = " Email Adress";
            surname = "surname";
            Firstname = "Firstname";
            centersize = "Center Size";
            TableTitle = "Country";
            country = "Country";
            Worshipname = "Worhsip Center";

            prodRows = new List<List<string>>();
            Debug.WriteLine("MyMethod was called");
            // rest of the method code
            SqlConnection conn = new SqlConnection(str2);
            string selectquery2 = $"SELECT Pid, PastorName,Psurname, Pnumber,Email, CenterSize, Country, WorshipName FROM Pastors order by Pid";
            SqlCommand mysql2 = new SqlCommand(selectquery2, conn);
            conn.Open();
            using (SqlDataReader reader2 = mysql2.ExecuteReader())
            {
                while (reader2.Read())
                {
                    List<string> row = new List<string>();
                    for (int i = 0; i < reader2.FieldCount; i++)
                    {
                        row.Add(reader2[i].ToString());
                    }
                    prodRows.Add(row);
                }
                counterload();
                conn.Close();
            }
        }
        public void OnGetSortSurname_Click(object sender, EventArgs e)
        {
            ProfileId = "ID";
            phonenumber = "Pastors Number";
            email = " Email Adress";
            surname = "surname";
            Firstname = "Firstname";
            centersize = "Center Size";
            TableTitle = "Country";
            country = "Country";
            Worshipname = "Worhsip Center";

            prodRows = new List<List<string>>();
            Debug.WriteLine("MyMethod was called");
            // rest of the method code
            SqlConnection conn = new SqlConnection(str2);
            string selectquery2 = $"SELECT Pid, PastorName,Psurname, Pnumber,Email, CenterSize, Country, WorshipName FROM Pastors order by Psurname";
            SqlCommand mysql2 = new SqlCommand(selectquery2, conn);
            conn.Open();
            using (SqlDataReader reader2 = mysql2.ExecuteReader())
            {
                while (reader2.Read())
                {
                    List<string> row = new List<string>();
                    for (int i = 0; i < reader2.FieldCount; i++)
                    {
                        row.Add(reader2[i].ToString());
                    }
                    prodRows.Add(row);
                }
                counterload();
                conn.Close();
            }
        }

        public void OnGetSortFname_Click(object sender, EventArgs e)
        {
            ProfileId = "ID";
            phonenumber = "Pastors Number";
            email = " Email Adress";
            surname = "surname";
            Firstname = "Firstname";
            centersize = "Center Size";
            TableTitle = "Country";
            country = "Country";
            Worshipname = "Worhsip Center";

            prodRows = new List<List<string>>();
            Debug.WriteLine("MyMethod was called");
            // rest of the method code
            SqlConnection conn = new SqlConnection(str2);
            string selectquery2 = $"SELECT Pid, PastorName,Psurname, Pnumber,Email, CenterSize, Country, WorshipName FROM Pastors order by PastorName";
            SqlCommand mysql2 = new SqlCommand(selectquery2, conn);
            conn.Open();
            using (SqlDataReader reader2 = mysql2.ExecuteReader())
            {
                while (reader2.Read())
                {
                    List<string> row = new List<string>();
                    for (int i = 0; i < reader2.FieldCount; i++)
                    {
                        row.Add(reader2[i].ToString());
                    }
                    prodRows.Add(row);
                }
                counterload();
                conn.Close();
            }
        }


        public void OnGetSortNumber_Click(object sender, EventArgs e)
        {
            ProfileId = "ID";
            phonenumber = "Pastors Number";
            email = " Email Adress";
            surname = "surname";
            Firstname = "Firstname";
            centersize = "Center Size";
            TableTitle = "Country";
            country = "Country";
            Worshipname = "Worhsip Center";

            prodRows = new List<List<string>>();
            Debug.WriteLine("MyMethod was called");
            // rest of the method code
            SqlConnection conn = new SqlConnection(str2);
            string selectquery2 = $"SELECT Pid, PastorName,Psurname, Pnumber,Email, CenterSize, Country, WorshipName FROM Pastors order by Pnumber";
            SqlCommand mysql2 = new SqlCommand(selectquery2, conn);
            conn.Open();
            using (SqlDataReader reader2 = mysql2.ExecuteReader())
            {
                while (reader2.Read())
                {
                    List<string> row = new List<string>();
                    for (int i = 0; i < reader2.FieldCount; i++)
                    {
                        row.Add(reader2[i].ToString());
                    }
                    prodRows.Add(row);
                }
                counterload();
                conn.Close();
            }
        }
        public void OnGetSortSocial_Click(object sender, EventArgs e)
        {
            ProfileId = "ID";
            phonenumber = "Pastors Number";
            email = " Email Adress";
            surname = "surname";
            Firstname = "Firstname";
            centersize = "Center Size";
            TableTitle = "Country";
            country = "Country";
            Worshipname = "Worhsip Center";

            prodRows = new List<List<string>>();
            Debug.WriteLine("MyMethod was called");
            // rest of the method code
            SqlConnection conn = new SqlConnection(str2);
            string selectquery2 = $"SELECT Pid, PastorName,Psurname, Pnumber,Email, CenterSize, Country, WorshipName FROM Pastors order by Email";
            SqlCommand mysql2 = new SqlCommand(selectquery2, conn);
            conn.Open();
            using (SqlDataReader reader2 = mysql2.ExecuteReader())
            {
                while (reader2.Read())
                {
                    List<string> row = new List<string>();
                    for (int i = 0; i < reader2.FieldCount; i++)
                    {
                        row.Add(reader2[i].ToString());
                    }
                    prodRows.Add(row);
                }
                counterload();
                conn.Close();
            }
        }
        public void OnGetSortMember_Click(object sender, EventArgs e)
        {
            ProfileId = "ID";
            phonenumber = "Pastors Number";
            email = " Email Adress";
            surname = "surname";
            Firstname = "Firstname";
            centersize = "Center Size";
            TableTitle = "Country";
            country = "Country";
            Worshipname = "Worhsip Center";

            prodRows = new List<List<string>>();
            Debug.WriteLine("MyMethod was called");
            // rest of the method code
            SqlConnection conn = new SqlConnection(str2);
            string selectquery2 = $"SELECT Pid, PastorName,Psurname, Pnumber,Email, CenterSize, Country, WorshipName FROM Pastors order by CenterSize";
            SqlCommand mysql2 = new SqlCommand(selectquery2, conn);
            conn.Open();
            using (SqlDataReader reader2 = mysql2.ExecuteReader())
            {
                while (reader2.Read())
                {
                    List<string> row = new List<string>();
                    for (int i = 0; i < reader2.FieldCount; i++)
                    {
                        row.Add(reader2[i].ToString());
                    }
                    prodRows.Add(row);
                }
                counterload();
                conn.Close();
            }
        }
        public void OnGetSortCountry_Click(object sender, EventArgs e)
        {
            ProfileId = "ID";
            phonenumber = "Pastors Number";
            email = " Email Adress";
            surname = "surname";
            Firstname = "Firstname";
            centersize = "Center Size";
            TableTitle = "Country";
            country = "Country";
            Worshipname = "Worhsip Center";

            prodRows = new List<List<string>>();
            Debug.WriteLine("MyMethod was called");
            // rest of the method code
            SqlConnection conn = new SqlConnection(str2);
            string selectquery2 = $"SELECT Pid, PastorName,Psurname, Pnumber,Email, CenterSize, Country, WorshipName FROM Pastors order by Country";
            SqlCommand mysql2 = new SqlCommand(selectquery2, conn);
            conn.Open();
            using (SqlDataReader reader2 = mysql2.ExecuteReader())
            {
                while (reader2.Read())
                {
                    List<string> row = new List<string>();
                    for (int i = 0; i < reader2.FieldCount; i++)
                    {
                        row.Add(reader2[i].ToString());
                    }
                    prodRows.Add(row);
                }
                counterload();
                conn.Close();
            }
        }
        public void OnGetSortCenter_Click(object sender, EventArgs e)
        {
            ProfileId = "ID";
            phonenumber = "Pastors Number";
            email = " Email Adress";
            surname = "surname";
            Firstname = "Firstname";
            centersize = "Center Size";
            TableTitle = "Country";
            country = "Country";
            Worshipname = "Worhsip Center";

            prodRows = new List<List<string>>();
            Debug.WriteLine("MyMethod was called");
            // rest of the method code
            SqlConnection conn = new SqlConnection(str2);
            string selectquery2 = $"SELECT Pid, PastorName,Psurname, Pnumber,Email, CenterSize, Country, WorshipName FROM Pastors order by WorshipName";
            SqlCommand mysql2 = new SqlCommand(selectquery2, conn);
            conn.Open();
            using (SqlDataReader reader2 = mysql2.ExecuteReader())
            {
                while (reader2.Read())
                {
                    List<string> row = new List<string>();
                    for (int i = 0; i < reader2.FieldCount; i++)
                    {
                        row.Add(reader2[i].ToString());
                    }
                    prodRows.Add(row);
                }
                counterload();
                conn.Close();
            }
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

        public async void OnGetAddPastor_click(object sender, EventArgs e)
        {
            string fk = Request.Query["ChurchId"];
            string emails = Request.Query["email"];
            string fnames = Request.Query["fnames"];
            string snames = Request.Query["snames"];
            string size = Request.Query["size"];
            string countrys = Request.Query["country"];
            string phones = Request.Query["phone"];
            churchids = fk;
            ids();
            string[] parts = emails.Split(new char[] { '@', '.' });
            string result = parts[0] + parts[1];
            string selectquery4 = $"Select Pid from Pastors where Pid ='{result}';";
            SqlConnection conn = new SqlConnection(str2);
            conn.Open();
            SqlCommand mysql5 = new SqlCommand(selectquery4, conn);
            SqlDataReader reader4 = mysql5.ExecuteReader();
            reader4.Read();
            if (reader4.HasRows.Equals(false))
            {
                reader4.Close();

                string saveprofile = "INSERT INTO Pastors (Pid, churchid, PastorName,Psurname, WorshipName, Email, Pnumber, CenterSize,Country) VALUES (@Pid, @churchid, @PastorName, @surname,@WorshipName, @Email, @Pnumber, @CenterSize,@place)";
                SqlCommand firstcaputure = new SqlCommand(saveprofile, conn);
                firstcaputure.Parameters.Add(@"Pid", SqlDbType.VarChar, 100).Value = result.ToString().Trim();
                firstcaputure.Parameters.Add(@"churchid", SqlDbType.VarChar, 100).Value = fk;
                firstcaputure.Parameters.Add(@"PastorName", SqlDbType.VarChar, 100).Value = fnames;
                firstcaputure.Parameters.Add(@"surname", SqlDbType.VarChar, 100).Value = snames;
                firstcaputure.Parameters.Add(@"WorshipName", SqlDbType.VarChar, 100).Value = churchnames;
                firstcaputure.Parameters.Add(@"Email", SqlDbType.VarChar, 100).Value = emails;
                firstcaputure.Parameters.Add(@"Pnumber", SqlDbType.VarChar, 100).Value = phones;
                firstcaputure.Parameters.Add(@"CenterSize", SqlDbType.VarChar, 100).Value = size;
                firstcaputure.Parameters.Add(@"place", SqlDbType.VarChar, 100).Value = countrys;
                firstcaputure.ExecuteScalar();
                tablepopulate();
             

                StatusMessage = $"{emails} details caputored sucessfully";
                Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", "thchurch-661c7-firebase-adminsdk-bfoyu-7460d48e93.json");
                FirestoreDb db = FirestoreDb.Create("thchurch-661c7");
                DocumentReference docRef = db.Collection("pastors").Document(result.ToLower());


                await docRef.SetAsync(new { userDetails = new { firstname = fnames, surname = snames, phone = phones, email = emails, centersize = size, church = churchnames, country = countrys, churchid = fk ,admin = "false"} }, SetOptions.MergeAll);
            }
            else
            {
              
                tablepopulate();
                Response.Redirect("/WorshipManage");
                StatusMessage = $" error {emails} already exists";
            }
            Debug.WriteLine(result);

        }



        public async void OnGetDownload_Click(object sender, EventArgs e)
        {
            string getter = str2;
            string selectMemberssDATA = "SELECT * FROM Pastors";

            Document PageSetup = new Document();
            Section newpart = PageSetup.AddSection();


            Paragraph heading = newpart.AddParagraph("Harvest Tabernacle Church Pastor Report:");
            heading.Format.Font.Size = 14;
            heading.Format.Font.Name = "Times New Roman";
            heading.Format.SpaceAfter = 10;
            heading.Format.Alignment = ParagraphAlignment.Center;
            heading.Format.Alignment = ParagraphAlignment.Justify;
            heading.Format.Font.Italic = true;

            MigraDocTable reports = newpart.AddTable();

            PageSetup.DefaultPageSetup.PageFormat = PageFormat.A4;
            PageSetup.DefaultPageSetup.TopMargin = Unit.FromCentimeter(2);
            PageSetup.DefaultPageSetup.BottomMargin = Unit.FromCentimeter(2);
            PageSetup.DefaultPageSetup.LeftMargin = Unit.FromMillimeter(3);
            PageSetup.DefaultPageSetup.RightMargin = Unit.FromCentimeter(3);
            XGraphics gfx = XGraphics.CreateMeasureContext(new XSize(1000, 1000), XGraphicsUnit.Point, XPageDirection.Downwards);
            XFont font = new XFont("Arial", 8);


            using (SqlConnection Memberstable = new SqlConnection(getter))
            {
                Memberstable.Open();
                SqlCommand selectquery = new SqlCommand(selectMemberssDATA, Memberstable);
                SqlDataReader data = selectquery.ExecuteReader();
                reports.Rows.Height = Unit.FromPoint(12);
                reports.Rows.VerticalAlignment = VerticalAlignment.Center;
                reports.Format.Font.Size = 6;


                List<double> maxColumnWidths = new List<double>();
                for (int i = 0; i < data.FieldCount; i++)
                {
                    maxColumnWidths.Add(0);
                }

                while (data.Read())
                {
                    for (int i = 0; i < data.FieldCount; i++)
                    {
                        string cellText = data.GetValue(i).ToString();
                        double cellTextWidth = gfx.MeasureString(cellText, font).Width;
                        maxColumnWidths[i] = Math.Max(maxColumnWidths[i], cellTextWidth);
                    }
                }


                data.Close();
                data = selectquery.ExecuteReader();


                for (int i = 0; i < data.FieldCount; i++)
                {
                    MigraDocColumn column = reports.AddColumn();
                    column.Width = Unit.FromPoint(maxColumnWidths[i]);
                    column.Format.Alignment = ParagraphAlignment.Center;
                }


                Row headerRow = reports.AddRow();
                for (int i = 0; i < data.FieldCount; i++)
                {
                    Cell cell = headerRow.Cells[i];
                    cell.AddParagraph(data.GetName(i));
                    cell.Format.Font.Bold = true;
                    cell.Shading.Color = Colors.LightGray;
                    cell.Borders.Width = 0.5;
                    cell.Format.Font.Size = 5;
                }


                while (data.Read())
                {
                    Row row = reports.AddRow();
                    for (int i = 0; i < data.FieldCount; i++)
                    {
                        Cell cell = row.Cells[i];
                        string cellText = data.GetValue(i).ToString();
                        int maxLineLength = 40;
                        if (cellText.Length > maxLineLength)
                        {

                            List<string> lines = SplitTextIntoLines(cellText, maxLineLength);
                            foreach (string line in lines)
                            {
                                cell.AddParagraph(line);
                            }
                        }
                        else
                        {
                            cell.AddParagraph(cellText);
                        }
                        cell.Format.Alignment = ParagraphAlignment.Center;
                        cell.Borders.Width = 0.5;
                    }
                }
            }




            PdfDocumentRenderer renderer = new PdfDocumentRenderer(true);
            renderer.Document = PageSetup;
            renderer.RenderDocument();


            MemoryStream stream = new MemoryStream();
            renderer.PdfDocument.Save(stream, false);
            Response.Clear();
            Response.ContentType = "application/pdf";
            Response.Headers["Content-Disposition"] = "attachment; filename=PastorList.pdf";
            await Response.Body.WriteAsync(stream.ToArray(), 0, (int)stream.Length);

            await Response.CompleteAsync();
        }
        private List<string> SplitTextIntoLines(string text, int maxLineLength)
        {
            List<string> lines = new List<string>();
            int currentIndex = 0;
            while (currentIndex < text.Length)
            {
                int lineEndIndex = Math.Min(currentIndex + maxLineLength, text.Length);
                lines.Add(text.Substring(currentIndex, lineEndIndex - currentIndex));
                currentIndex = lineEndIndex;
            }
            return lines;
        }
    }

}

