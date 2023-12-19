using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using MigraDocTable = MigraDoc.DocumentObjectModel.Tables.Table;
using MigraDocColumn = MigraDoc.DocumentObjectModel.Tables.Column;
using MigraDoc.DocumentObjectModel.Tables;
using MigraDoc.DocumentObjectModel;
using MigraDoc.Rendering;
using PdfSharp.Drawing;
using PdfSharp.Fonts;

namespace THC_CHURCH_WEBSITE.Pages
{
    public class LocalLeaderManagmentModel : PageModel
    {
        private readonly string str2 = "Server=tcp:progpartst10090552.database.windows.net,1433;Initial Catalog=thcchurchgroup;Persist Security Info=False;User ID=momoessop;Password=Mobile22;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
        public List<List<string>> prodRows { get; set; }
        // these variables allow for use in the backend to write and read data reading only methods and variables have been made private 
        public string churchids { get; set; }
        public string churchnames { get; set; }
        public string? Message { get; set; }
        public string test { get; set; }
        public string? phonenumber { get; set; }
        public string Error { get; set; }
        public string TableTitle { get; set; }
        public string? email { get; set; }
        public string? country { get; set; }
        public string? surname { get; set; }
        public string? datestarted { get; set; }
        public string? ProfileId { get; set; }
        public string Worshipname { get; set; }
        public string Leadercount { get; set; }
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
                tablepopulate();
         
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

        private void tablepopulate()
        {

            prodRows = new List<List<string>>();


            SqlConnection conn = new SqlConnection(str2);
            conn.Open();




            string selectquery4 = "SELECT Lid, LName,Lsurname, Pnumber,Email, DateStarted, WorshipName FROM Leaders";

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
            phonenumber = "Leader Number";
            email = " Email Adress";
            surname = "surname";
            Firstname = "Firstname";
            datestarted = "Date Started";
            Worshipname = "Worhsip Center";
            counterload();

        }

        private void counterload()
        {
            int count = 0;
            using (SqlConnection connection = new SqlConnection(str2))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("SELECT COUNT(*) FROM Leaders", connection))
                {
                    count = (int)command.ExecuteScalar();
                    Leadercount = count.ToString();
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
        public async void OnGetAddLeader_click(object sender, EventArgs e)
        {
            Debug.WriteLine("called");
            string fk = Request.Query["ChurchId"];
            string email = Request.Query["email"];
            string fnames = Request.Query["fnames"];
            string snames = Request.Query["snames"];
            string datestarted = Request.Query["date"];

            churchids = fk;
            string phone = Request.Query["phone"];
            ids();
            string[] parts = email.Split(new char[] { '@', '.' });
            string result = parts[0] + parts[1];
            string selectquery4 = $"Select Lid from Leaders where Lid ='{result}';";
            SqlConnection conn = new SqlConnection(str2);
            conn.Open();
            SqlCommand mysql5 = new SqlCommand(selectquery4, conn);
            SqlDataReader reader4 = mysql5.ExecuteReader();
            reader4.Read();
         
            if (reader4.HasRows.Equals(false))
            {
                try
                {
                    reader4.Close();

                    string saveprofile = "INSERT INTO Leaders (Lid, churchid, LName,Lsurname, WorshipName, Email, Pnumber,DateStarted) VALUES (@Lid, @churchid, @LName, @surname,@WorshipName, @Email, @Pnumber, @date)";
                    SqlCommand firstcaputure = new SqlCommand(saveprofile, conn);
                    firstcaputure.Parameters.Add(@"Lid", SqlDbType.VarChar, 100).Value = result.ToString().Trim();
                    firstcaputure.Parameters.Add(@"churchid", SqlDbType.VarChar, 100).Value = fk;
                    firstcaputure.Parameters.Add(@"LName", SqlDbType.VarChar, 100).Value = fnames;
                    firstcaputure.Parameters.Add(@"surname", SqlDbType.VarChar, 100).Value = snames;
                    firstcaputure.Parameters.Add(@"WorshipName", SqlDbType.VarChar, 100).Value = churchnames;
                    firstcaputure.Parameters.Add(@"Email", SqlDbType.VarChar, 100).Value = email;
                    firstcaputure.Parameters.Add(@"Pnumber", SqlDbType.VarChar, 100).Value = phone;
                    firstcaputure.Parameters.Add(@"date", SqlDbType.VarChar, 100).Value = datestarted;

                    firstcaputure.ExecuteScalar();
                    tablepopulate();
                    StatusMessage = "leader details caputored sucessfully";
                }
                catch
                {
                    StatusMessage = "please select a church from the center";


                }


            }
            else
            {
                StatusMessage = " error user already exists";
                Response.Redirect("/LocalLeaderManagment");
                tablepopulate();    
            }
            Debug.WriteLine(result);

        }
        public void OnGetUpdate_Click(object sender, EventArgs e)
        { 

            Debug.WriteLine("MyMethod was called");
            string id = Request.Query["id"];
            string lname = Request.Query["lname"];
            string lsname = Request.Query["lsname"];
            string pnumber = Request.Query["pnum"];
            string lemail = Request.Query["lmail"];
            string sdate = Request.Query["dates"];
            string name = Request.Query["cname"];

            DateTime dt;
            bool isValid = DateTime.TryParseExact(sdate, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt);
            if (id.IsNullOrEmpty().Equals(true))
            {

                StatusMessage = "please enter changes before hitting save";
                tablepopulate();
            }
          else  if (lname.Length < 4)
            {
                StatusMessage = "Leader Name too short";
                tablepopulate();
            }
            else if (lsname.Length < 4)
            {
                StatusMessage = "Leader surname too short";
                tablepopulate();
            }
            else if (pnumber.Length < 7)
            {
                StatusMessage = "Leader phone number too short";
                tablepopulate();
            }
            else if (name.Length < 4)
            {
                StatusMessage = "church name too short";
                tablepopulate();
            }
            else if (Emailchecker(lemail).Equals(false))
            {
                StatusMessage = "please enter a valid email";
                tablepopulate();
            }
            else if(isValid.Equals( false))
            {
                StatusMessage = "please enter a valid date";
                tablepopulate();
                Debug.WriteLine(sdate);
            }
            else
            {
                Debug.WriteLine(id);
                // Create an UPDATE command to update the data in the database
                string connectionString = str2;
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand($"UPDATE Leaders SET LName = @leaderName, Lsurname = @surname, WorshipName = @WorshipName, Email = @Email, Pnumber = @Pnumber, DateStarted = @CenterSize  WHERE Lid = '{id}'", connection))
                    {
                        try
                        {
                            // Set the values of the parameters in the command
                            command.Parameters.AddWithValue("@leaderName", lname);
                            command.Parameters.AddWithValue("@surname", lsname);
                            command.Parameters.AddWithValue("@WorshipName", name);
                            command.Parameters.AddWithValue("@Email", lemail);
                            command.Parameters.AddWithValue("@Pnumber", pnumber);
                            command.Parameters.AddWithValue("@CenterSize", sdate);



                            command.ExecuteNonQuery();
                            tablepopulate();
                            Response.Redirect("/LocalLeaderManagment");
                            StatusMessage = $"{lemail} updated Successfully";
                        }
                        catch (Exception ex) { Debug.WriteLine(ex.ToString()); }
                    }
                }
            }
        }

        public async void OnGetDelete_click(object sender, EventArgs e)
        {
            string Lid = Request.Query["deletentry"];


            string selectquery4 = $"Select Lid from Leaders where Lid ='{Lid}';";
            SqlConnection conn = new SqlConnection(str2);
            conn.Open();
            SqlCommand mysql5 = new SqlCommand(selectquery4, conn);
            SqlDataReader reader4 = mysql5.ExecuteReader();
            reader4.Read();
            if (reader4.HasRows.Equals(true))
            {
                reader4.Close();

                string delete = $"delete from Leaders where Lid = '{Lid}'";
                SqlCommand firstcaputure = new SqlCommand(delete, conn);

                firstcaputure.ExecuteScalar();
                tablepopulate();

                StatusMessage = $"leader {Lid} deleted sucessfully";

                conn.Close();
                Response.Redirect("/LocalLeaderManagment");
            }
            else
            {
                tablepopulate();
                StatusMessage = $"Leader {Lid} doesnt exist or has been deleted already";
                conn.Close();
                Response.Redirect("/LocalLeaderManagment");
            }


        }

        public void OnGetFilter_Click(object sender, EventArgs e)
        {
            ProfileId = "ID";
            phonenumber = "Leader Number";
            email = " Email Adress";
            surname = "surname";
            Firstname = "Firstname";
            datestarted = "Date Started";
            Worshipname = "Worhsip Center";
            string entry = Request.Query["filters"];
            string spinner = Request.Query["filterspin"];
            prodRows = new List<List<string>>();
            Debug.WriteLine("MyMethod was called");
            // rest of the method code
            SqlConnection conn = new SqlConnection(str2);
            if (entry.IsNullOrEmpty().Equals(true) && spinner.IsNullOrEmpty().Equals(true))
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
                try
                {
                    string selectquery2 = $"SELECT Lid, LName, Lsurname, Pnumber, Email, DateStarted, WorshipName from Leaders where {spinner} COLLATE Latin1_General_CI_AS  LIKE '%{entry}%' ";

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
                }
                catch (Exception ex) { Debug.WriteLine(ex.Message); }

            }


          
        }

        public void OnGetSortDate_Click(object sender, EventArgs e)
        {
            ProfileId = "ID";
            phonenumber = "Leader Number";
            email = " Email Adress";
            surname = "surname";
            Firstname = "Firstname";
            datestarted = "Date Started";
            Worshipname = "Worhsip Center";

            prodRows = new List<List<string>>();
            Debug.WriteLine("MyMethod was called");
            // rest of the method code
            SqlConnection conn = new SqlConnection(str2);
            string selectquery2 = "SELECT Lid, LName,Lsurname, Pnumber,Email, DateStarted, WorshipName FROM Leaders order by DateStarted";
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

        public void OnGetSortId_Click(object sender, EventArgs e)
        {

            ProfileId = "ID";
            phonenumber = "Leader Number";
            email = " Email Adress";
            surname = "surname";
            Firstname = "Firstname";
            datestarted = "Date Started";
            Worshipname = "Worhsip Center";


            prodRows = new List<List<string>>();
            Debug.WriteLine("MyMethod was called");
            // rest of the method code
            SqlConnection conn = new SqlConnection(str2);
            string selectquery2 = $"SELECT Lid, LName,Lsurname, Pnumber,Email, DateStarted, WorshipName FROM Leaders order by Lid";
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
            phonenumber = "Leader Number";
            email = " Email Adress";
            surname = "surname";
            Firstname = "Firstname";
            datestarted = "Date Started";
            Worshipname = "Worhsip Center";


            prodRows = new List<List<string>>();
            Debug.WriteLine("MyMethod was called");
            // rest of the method code
            SqlConnection conn = new SqlConnection(str2);
            string selectquery2 = $"SELECT Lid, LName,Lsurname, Pnumber,Email, DateStarted, WorshipName FROM Leaders order by Lsurname";
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
            phonenumber = "Leader Number";
            email = " Email Adress";
            surname = "surname";
            Firstname = "Firstname";
            datestarted = "Date Started";
            Worshipname = "Worhsip Center";

            prodRows = new List<List<string>>();
            Debug.WriteLine("MyMethod was called");
            // rest of the method code
            SqlConnection conn = new SqlConnection(str2);
            string selectquery2 = $"SELECT Lid, LName,Lsurname, Pnumber,Email, DateStarted, WorshipName FROM Leaders order by LName";
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
            phonenumber = "Leader Number";
            email = " Email Adress";
            surname = "surname";
            Firstname = "Firstname";
            datestarted = "Date Started";
            Worshipname = "Worhsip Center";


            prodRows = new List<List<string>>();
            Debug.WriteLine("MyMethod was called");
            // rest of the method code
            SqlConnection conn = new SqlConnection(str2);
            string selectquery2 = $"SELECT Lid, LName,Lsurname, Pnumber,Email, DateStarted, WorshipName FROM Leaders order by Pnumber";
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
        public void OnGetSortEmail_Click(object sender, EventArgs e)
        {

            ProfileId = "ID";
            phonenumber = "Leader Number";
            email = " Email Adress";
            surname = "surname";
            Firstname = "Firstname";
            datestarted = "Date Started";
            Worshipname = "Worhsip Center";


            prodRows = new List<List<string>>();
            Debug.WriteLine("MyMethod was called");
            // rest of the method code
            SqlConnection conn = new SqlConnection(str2);
            string selectquery2 = $"SELECT Lid, LName,Lsurname, Pnumber,Email, DateStarted, WorshipName FROM Leaders order by Email";
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
            phonenumber = "Leader Number";
            email = " Email Adress";
            surname = "surname";
            Firstname = "Firstname";
            datestarted = "Date Started";
            Worshipname = "Worhsip Center";


            prodRows = new List<List<string>>();
            Debug.WriteLine("MyMethod was called");
            // rest of the method code
            SqlConnection conn = new SqlConnection(str2);
            string selectquery2 = $"SELECT Lid, LName,Lsurname, Pnumber,Email, DateStarted, WorshipName FROM Leaders order by WorshipName";
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

        public async void OnGetDownload_Click(object sender, EventArgs e)
        {
            string getter = str2;
            string selectMemberssDATA = "SELECT * FROM Leaders";

            Document PageSetup = new Document();
            Section newpart = PageSetup.AddSection();


            Paragraph heading = newpart.AddParagraph("Harvest Tabernacle Church Leader Report:");
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
            XFont font = new XFont("Arial", 9);


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
            Response.Headers["Content-Disposition"] = "attachment; filename=LeaderReport.pdf";
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

