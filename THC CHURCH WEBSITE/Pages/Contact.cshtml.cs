using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Diagnostics;

namespace THC_CHURCH_WEBSITE.Pages
{
    public class ContactModel : PageModel
    {
        public void OnGet()
        {
        }
        [TempData]
        public string StatusMessage { get; set; }
        private readonly string str2 = "Server=tcp:progpartst10090552.database.windows.net,1433;Initial Catalog=thcchurchgroup;Persist Security Info=False;User ID=momoessop;Password=Mobile22;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";


        public void OnGetMailing_Click(object sender, EventArgs e)
        {
            string MailID;


            string email = Request.Query["InputEmail1"];
            string churchId = Request.Query["sel1"].ToString().Split(',')[0];
            string churchName = Request.Query["sel1"].ToString().Split(',')[1];


            string[] parts = email.Split(new char[] { '@', '.' });
            string result = parts[0] + parts[1];


            string selectquery4 = $"Select MailID from Mailing where MailID ='{result}';";
            SqlConnection conn2 = new SqlConnection(str2);
            conn2.Open();
            SqlCommand mysql5 = new SqlCommand(selectquery4, conn2);
            SqlDataReader reader4 = mysql5.ExecuteReader();
            reader4.Read();
            if (reader4.HasRows.Equals(true))
            {
                StatusMessage = "ERROR USER ALREADY EXISTS";
            }

            else
            {




                using (SqlConnection conn = new SqlConnection(str2))
                {
                    // Open the connection
                    conn.Open();
                    string query = "INSERT INTO Mailing (MailID, churchid, Mfirstname, Mlastname, Mreason,mailaddress,churchname) VALUES (@MailID, @churchid, @Mfirstname, @Mlastname, @Mreason,@mail,@name)";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        try
                        {

                            cmd.Parameters.Add(@"MailID", SqlDbType.VarChar, 100).Value = result.ToString().Trim();
                            cmd.Parameters.Add(@"churchid", SqlDbType.VarChar, 100).Value = churchId;
                            cmd.Parameters.Add(@"Mfirstname", SqlDbType.VarChar, 100).Value = Request.Query["InputFirstName"].ToString().Trim();
                            cmd.Parameters.Add(@"Mlastname", SqlDbType.VarChar, 100).Value = Request.Query["InputLastName"].ToString().Trim();
                            cmd.Parameters.Add(@"Mreason", SqlDbType.VarChar, 100).Value = Request.Query["InputReason"].ToString().Trim();
                            cmd.Parameters.Add(@"mail", SqlDbType.VarChar, 100).Value = email;
                            cmd.Parameters.Add(@"name", SqlDbType.VarChar, 100).Value = churchName;


                            cmd.ExecuteNonQuery();
                        }
                        catch (Exception ex)
                        {
                            // Display an error message or log the exception details
                            Debug.WriteLine(ex);

                        }

                    }
                }
            }



        }

        public List<string> GetChurchNames()
        {
            // Create an empty list to store the church names
            List<string> churchNames = new List<string>();

            // Create a connection object using the connection string
            SqlConnection conn = new SqlConnection(str2);

            // Create a SQL query to select all the church names from the Church table
            string query = "SELECT ChurchId, ChurchName FROM Churchs";

            // Create a command object using the connection and the query
            SqlCommand cmd = new SqlCommand(query, conn);

            // Open the connection
            conn.Open();

            // Execute the query and get a data reader object
            SqlDataReader reader = cmd.ExecuteReader();

            // Loop through the rows in the data reader
            while (reader.Read())
            {
                // Get the church name from the current row
                string churchId = reader["ChurchId"].ToString();
                string churchName = reader["ChurchName"].ToString();

                // Add the church name to the list
                churchNames.Add(churchId + ", " + churchName);
            }

            // Close the data reader
            reader.Close();

            // Close the connection
            conn.Close();

            // Return the list of church names
            return churchNames;

        }
    }
}
