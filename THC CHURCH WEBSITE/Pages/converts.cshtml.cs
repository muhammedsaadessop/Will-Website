using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Diagnostics;

using Microsoft.IdentityModel.Tokens;
using MigraDoc.DocumentObjectModel.Tables;
using MigraDoc.DocumentObjectModel;
using MigraDoc.Rendering;
using PdfSharp.Drawing;
using MigraDocTable = MigraDoc.DocumentObjectModel.Tables.Table;
using MigraDocColumn = MigraDoc.DocumentObjectModel.Tables.Column;
using PdfSharp.Fonts;

namespace THC_CHURCH_WEBSITE.Pages
{
    public class convertsModel : PageModel
    {
        private readonly string str2 = "Server=tcp:progpartst10090552.database.windows.net,1433;Initial Catalog=thcchurchgroup;Persist Security Info=False;User ID=momoessop;Password=Mobile22;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
        public List<List<string>> prodRows { get; set; }
        // these variables allow for use in the backend to write and read data reading only methods and variables have been made private \
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
        public string? Gender { get; set; }
        public string? ProfileId { get; set; }
        public string Worshipname { get; set; }
        public string Marriagestatus { get; set; }
        public string Convertscount { get; set; }
        public string cCount { get; set; }
        public string? Firstname { get; set; }
        [TempData]
        public string StatusMessage { get; set; }
        public string Email { get; set; }
        public void OnGet()
        {
            if (GlobalFontSettings.FontResolver == null)
            {
                GlobalFontSettings.FontResolver = new MyFontResolver();
            }
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




            string selectquery4 = "SELECT Cid, CName,Csurname, Pnumber,Email, Gender, WorshipName,Maritals FROM Converts";

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
            phonenumber = "Convert Number";
            email = " Email Adress";
            surname = "surname";
            Firstname = "Firstname";
            Gender = "gender";
            Worshipname = "Worhsip Center";
            Marriagestatus = "Marriage Status";
            counterload();

        }

        private void counterload()
        {
            int count = 0;
            using (SqlConnection connection = new SqlConnection(str2))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("SELECT COUNT(*) FROM Converts", connection))
                {
                    count = (int)command.ExecuteScalar();
                    Convertscount = count.ToString();
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
        public async void OnGetAddConverts_click(object sender, EventArgs e)
        {
            Debug.WriteLine("called");
            string fk = Request.Query["ChurchId"];
            string emails = Request.Query["email"];
            string fnames = Request.Query["fnames"];
            string snames = Request.Query["snames"];
            string Genders = Request.Query["gender"];
      
            string mstatus = Request.Query["mstatus"];
            string phone = Request.Query["phone"];
            churchids = fk;
            ids();
            string[] parts = emails.Split(new char[] { '@', '.' });
            string result = parts[0] + parts[1];
            string selectquery4 = $"Select Cid from Converts where Cid ='{result}';";
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

                    string saveprofile = "INSERT INTO Converts (Cid, churchid, CName,Csurname, WorshipName, Email, Pnumber,Gender,Maritals) VALUES (@Cid, @churchid, @Cname, @surname,@WorshipName, @Email, @Pnumber, @date,@mari)";
                    SqlCommand firstcaputure = new SqlCommand(saveprofile, conn);
                    firstcaputure.Parameters.Add(@"Cid", SqlDbType.VarChar, 100).Value = result.ToString().Trim();
                    firstcaputure.Parameters.Add(@"churchid", SqlDbType.VarChar, 100).Value = fk;
                    firstcaputure.Parameters.Add(@"Cname", SqlDbType.VarChar, 100).Value = fnames;
                    firstcaputure.Parameters.Add(@"surname", SqlDbType.VarChar, 100).Value = snames;
                    firstcaputure.Parameters.Add(@"WorshipName", SqlDbType.VarChar, 100).Value = churchnames;
                    firstcaputure.Parameters.Add(@"Email", SqlDbType.VarChar, 100).Value = emails;
                    firstcaputure.Parameters.Add(@"Pnumber", SqlDbType.VarChar, 100).Value = phone;
                    firstcaputure.Parameters.Add(@"date", SqlDbType.VarChar, 100).Value = Genders;
                    firstcaputure.Parameters.Add(@"mari", SqlDbType.VarChar, 100).Value = mstatus.Trim();

                    firstcaputure.ExecuteScalar();
                    tablepopulate();
                    StatusMessage = $"{emails} details caputored sucessfully";
                    Response.Redirect("/converts");
                }
                catch
                {
                    StatusMessage = "please select a church from the center";

                    Response.Redirect("/converts");
                }


            }
            else
            {
                StatusMessage = $"error {emails} already exists";
                Response.Redirect("/converts");
                tablepopulate();
            }
            Debug.WriteLine(result);

        }
        public void OnGetUpdate_Click(object sender, EventArgs e)
        {

            Debug.WriteLine("MyMethod was called");
            string id = Request.Query["id"];
            string Cname = Request.Query["Cname"];
            string msname = Request.Query["msname"];
            string pnumber = Request.Query["pnum"];
            string memail = Request.Query["mmail"];
            string genders = Request.Query["new"];
            string name = Request.Query["mname"];
            string mstatus = Request.Query["maritals"];


            if (id.IsNullOrEmpty().Equals(true))
            {

                StatusMessage = "please enter changes before hitting save";
                tablepopulate();
            }
            else if (Cname.Length < 5)
            {
                StatusMessage = "Church Name too short";
                tablepopulate();
            }
            else if (msname.Length < 4)
            {
                StatusMessage = "Converts surname too short";
                tablepopulate();
            }
            else if (pnumber.Length < 7)
            {
                StatusMessage = "Converts phone number too short";
                tablepopulate();
            }
            else if (name.Length < 4)
            {
                StatusMessage = "converts name too short";
                tablepopulate();
            }
            else if (Emailchecker(memail).Equals(false))
            {
                StatusMessage = "please enter a valid email";
                tablepopulate();
            }
            else if (genders.Length < 4)
            {
                StatusMessage = "gender too short";
                tablepopulate();

            }
            else if (mstatus.Length < 5)
            {
                StatusMessage = "status too short";
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

                    using (SqlCommand command = new SqlCommand($"UPDATE Converts SET CName = @ConvertsName, Csurname = @surname, WorshipName = @WorshipName, Email = @Email, Pnumber = @Pnumber, Gender = @gender, @stats = Maritals  WHERE Cid = '{id}'", connection))
                    {
                        try
                        {
                            // Set the values of the parameters in the command
                            command.Parameters.AddWithValue("@ConvertsName", name);
                            command.Parameters.AddWithValue("@surname", msname);
                            command.Parameters.AddWithValue("@WorshipName", Cname);
                            command.Parameters.AddWithValue("@Email", memail);
                            command.Parameters.AddWithValue("@Pnumber", pnumber);
                            command.Parameters.AddWithValue("@gender", genders);
                            command.Parameters.AddWithValue("@stats",mstatus);



                            command.ExecuteNonQuery();
                            tablepopulate();
                            Response.Redirect("/converts");
                            StatusMessage = $"{memail} has been updated";
                        }
                        catch (Exception ex) { Debug.WriteLine(ex.ToString()); }
                    }
                }
            }
        }

        public async void OnGetDelete_click(object sender, EventArgs e)
        {
            string Cid = Request.Query["deletentry"];


            string selectquery4 = $"Select Cid from Converts where Cid ='{Cid}';";
            SqlConnection conn = new SqlConnection(str2);
            conn.Open();
            SqlCommand mysql5 = new SqlCommand(selectquery4, conn);
            SqlDataReader reader4 = mysql5.ExecuteReader();
            reader4.Read();
            if (reader4.HasRows.Equals(true))
            {
                reader4.Close();

                string delete = $"delete from Converts where Cid = '{Cid}'";
                SqlCommand firstcaputure = new SqlCommand(delete, conn);

                firstcaputure.ExecuteScalar();
                tablepopulate();

                StatusMessage = $"Converts {Cid} deleted sucessfully";

                conn.Close();
                Response.Redirect("/converts");
            }
            else
            {
                tablepopulate();
                StatusMessage = $"Converts {Cid} doesnt exist or has been deleted already";
                conn.Close();
                Response.Redirect("/converts");
            }


        }

        public void OnGetFilter_Click(object sender, EventArgs e)
        {
            ProfileId = "ID";
            phonenumber = "Converts Number";
            email = " Email Adress";
            surname = "surname";
            Firstname = "Firstname";
            Gender = "Gender";
            Worshipname = "Worhsip Center";
            Marriagestatus = "Marriage status";
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
                    string selectquery2 = $"SELECT Cid, Cname,Csurname, Pnumber,Email, Gender, WorshipName ,Maritals from Converts where {spinner}  COLLATE Latin1_General_CI_AS  LIKE '%{entry}%'  ";
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

        public void OnGetSortGender_Click(object sender, EventArgs e)
        {
            ProfileId = "ID";
            phonenumber = "Converts Number";
            email = " Email Adress";
            surname = "surname";
            Firstname = "Firstname";
            Gender = "Gender";
            Worshipname = "Worhsip Center";
            Marriagestatus = "Marriage status";

            prodRows = new List<List<string>>();
            Debug.WriteLine("MyMethod was called");
            // rest of the method code
            SqlConnection conn = new SqlConnection(str2);
            string selectquery2 = "SELECT Cid, Cname,Csurname, Pnumber,Email, Gender, WorshipName,Maritals FROM Converts order by Gender";
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
            phonenumber = "Convert Number";
            email = " Email Adress";
            surname = "surname";
            Firstname = "Firstname";
            Gender = "gender";
            Worshipname = "Worhsip Center";
            Marriagestatus = "Marriage Status";


            prodRows = new List<List<string>>();
            Debug.WriteLine("MyMethod was called");
            // rest of the method code
            SqlConnection conn = new SqlConnection(str2);
            string selectquery2 = $"SELECT Cid, Cname,Csurname, Pnumber,Email, Gender, WorshipName,Maritals FROM Converts order by Cid";
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
            phonenumber = "Convert Number";
            email = " Email Adress";
            surname = "surname";
            Firstname = "Firstname";
            Gender = "gender";
            Worshipname = "Worhsip Center";
            Marriagestatus = "Marriage Status";


            prodRows = new List<List<string>>();
            Debug.WriteLine("MyMethod was called");
            // rest of the method code
            SqlConnection conn = new SqlConnection(str2);
            string selectquery2 = $"SELECT Cid, Cname,Csurname, Pnumber,Email, Gender, WorshipName,Maritals FROM Converts order by Csurname";
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
            phonenumber = "Convert Number";
            email = " Email Adress";
            surname = "surname";
            Firstname = "Firstname";
            Gender = "gender";
            Worshipname = "Worhsip Center";
            Marriagestatus = "Marriage Status";

            prodRows = new List<List<string>>();
            Debug.WriteLine("MyMethod was called");
            // rest of the method code
            SqlConnection conn = new SqlConnection(str2);
            string selectquery2 = $"SELECT Cid, Cname,Csurname, Pnumber,Email, Gender, WorshipName,Maritals FROM Converts order by Cname";
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
            phonenumber = "Convert Number";
            email = " Email Adress";
            surname = "surname";
            Firstname = "Firstname";
            Gender = "gender";
            Worshipname = "Worhsip Center";
            Marriagestatus = "Marriage Status";

            prodRows = new List<List<string>>();
            Debug.WriteLine("MyMethod was called");
            // rest of the method code
            SqlConnection conn = new SqlConnection(str2);
            string selectquery2 = $"SELECT Cid, Cname,Csurname, Pnumber,Email, Gender, WorshipName,Maritals FROM Converts order by Pnumber";
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
            phonenumber = "Convert Number";
            email = " Email Adress";
            surname = "surname";
            Firstname = "Firstname";
            Gender = "gender";
            Worshipname = "Worhsip Center";
            Marriagestatus = "Marriage Status";


            prodRows = new List<List<string>>();
            Debug.WriteLine("MyMethod was called");
            // rest of the method code
            SqlConnection conn = new SqlConnection(str2);
            string selectquery2 = $"SELECT Cid, Cname,Csurname, Pnumber,Email, Gender, WorshipName,Maritals FROM Converts order by Email";
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
            phonenumber = "Convert Number";
            email = " Email Adress";
            surname = "surname";
            Firstname = "Firstname";
            Gender = "gender";
            Worshipname = "Worhsip Center";
            Marriagestatus = "Marriage Status";

            prodRows = new List<List<string>>();
            Debug.WriteLine("MyMethod was called");
       
            SqlConnection conn = new SqlConnection(str2);
            string selectquery2 = $"SELECT Cid, Cname,Csurname, Pnumber,Email, Gender, WorshipName,Maritals FROM Converts order by WorshipName";
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

        public void OnGetSortMaritals_Click(object sender, EventArgs e)
        {

            ProfileId = "ID";
            phonenumber = "Convert Number";
            email = " Email Adress";
            surname = "surname";
            Firstname = "Firstname";
            Gender = "gender";
            Worshipname = "Worhsip Center";
            Marriagestatus = "Marriage Status";

            prodRows = new List<List<string>>();
            Debug.WriteLine("MyMethod was called");

            SqlConnection conn = new SqlConnection(str2);
            string selectquery2 = $"SELECT Cid, Cname,Csurname, Pnumber,Email, Gender, WorshipName,Maritals FROM Converts order by Maritals";
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
            string selectMemberssDATA = "SELECT * FROM Converts";

            Document PageSetup = new Document();
            Section newpart = PageSetup.AddSection();


            Paragraph heading = newpart.AddParagraph("Harvest Tabernacle Church Convert Report:");
            heading.Format.Font.Size = 14;
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
            XFont font = new XFont("Times New Roman", 9);


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
            Response.Headers["Content-Disposition"] = "attachment; filename=ConvertReport.pdf";
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
