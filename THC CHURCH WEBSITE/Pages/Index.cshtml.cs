using FireSharp;
using FireSharp.Config;
using FireSharp.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using Google.Cloud.Firestore;
using Firebase.Database;
using Firebase.Database.Query;
using FirebaseClient = Firebase.Database.FirebaseClient;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using System.Security.Cryptography;
using System.Security.Cryptography;
using Google.Apis.Auth.OAuth2;
using Firebase.Auth.Providers;
using Microsoft.IdentityModel.Tokens;
using Google.Type;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Diagnostics;
using LiteDB;
using System.Data;
using System.Text;
using System.Composition;
using System.Data.SqlClient;
using System.IO;
using NuGet.Common;



namespace THC_CHURCH_WEBSITE.Pages
{
    public class IndexModel : PageModel
    {
        private readonly string str2 = "Server=tcp:progpartst10090552.database.windows.net,1433;Initial Catalog=thcchurchgroup;Persist Security Info=False;User ID=momoessop;Password=Mobile22;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
        public List<List<string>> prodRows { get; set; }

        public string? Firstname { get; set; }
        [TempData]
        public string StatusMessage { get; set; }
        public List<Dictionary<string, object>> Events { get; set; }
       

        public string DailyVerse { get; set; }


        private readonly ILogger<IndexModel> _logger;

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }


        public async Task OnGetAsync()
        {
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", "thchurch-661c7-firebase-adminsdk-bfoyu-7460d48e93.json");
            FirestoreDb db = FirestoreDb.Create("thchurch-661c7");
            CollectionReference eventsRef = db.Collection("events");
            QuerySnapshot snap = await eventsRef.GetSnapshotAsync();
            Events = new List<Dictionary<string, object>>();
            if (snap.Documents != null)
            {
                foreach (DocumentSnapshot doc in snap.Documents)
                {
                    if (doc.Exists)
                    {
                        Dictionary<string, object> eventData = doc.ToDictionary();
                        Events.Add(eventData);
                    }
                }
            }


            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", "thchurch-661c7-firebase-adminsdk-bfoyu-7460d48e93.json");
            FirestoreDb db1 = FirestoreDb.Create("thchurch-661c7");

            // Create a document called verseoftheday under churches
            DocumentReference vRef = db1.Collection("churchs").Document("verseoftheday");

            DocumentSnapshot snapshot = await vRef.GetSnapshotAsync();

            if (snapshot.Exists)
            {
                Dictionary<string, object> documentData = snapshot.ToDictionary();
                List<object> verseObj= (List<object>)documentData["Verses"];

                // Convert each object in the list to a string
                List<string> verses = verseObj.ConvertAll(obj => obj.ToString());

                // Select a random verse
                Random r = new Random();
                DailyVerse = verses[r.Next(verses.Count)];
            }
            else
            {
                DailyVerse = "No verse of the day available.";
            }






        }
    }















    public class FacebookPosts
    {
        private static readonly HttpClient client = new HttpClient();

        private const string pageId = "106495838217015";
        private static string userAccessToken = "EAAVrv8Cffn4BOwza1DZCfu29ZBw3mMCuZBXO93JQeLuzXZCWZCOjCEGk99gnrFZBYUgSbcjXP0DfCh3lBegQ0MgzqDtyCOQcSHZBtIafgGoYR86sgZBaf3F6UZBV5qMj2OqlPHqbJfVbONjWraeuBclgvSe2Y49yLbK2xz3ra0Dk9dvK7cRK47TrZCCNzndtnfZCN4ZD";



        public static async Task<JArray> GetFacebookPostsAsync()
        {
            // Use the long-lived page access token to make the API call
            var rawdata = await client.GetAsync($"https://graph.facebook.com/v17.0/106495838217015/feed?fields=id,from,created_time,message,full_picture,likes.summary(true),comments.summary(true),shares&access_token={userAccessToken}");
            var data = await rawdata.Content.ReadAsStringAsync();
            var newsfeed = JObject.Parse(data)["data"] as JArray;

            // Create a new JArray to store non-deleted posts
            JArray activeposts = new JArray();

            // Iterate through the posts
            foreach (var update in newsfeed)
            {
                // Check if the post data is not null
                if (update["id"] != null && update["from"] != null && update["created_time"] != null && update["message"] != null)
                {
                    // Add the post to the nonDeletedPosts array
                    activeposts.Add(update);
                }
            }

            Debug.WriteLine(rawdata.ToString());
            Debug.WriteLine(data);
            Debug.WriteLine(activeposts.ToString());

            return activeposts;
        }

    }


    public class RedirectToDefaultPageMiddleware
    {
        private readonly RequestDelegate _next;

        public RedirectToDefaultPageMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {

            if (context.Request.Path == "/")
            {
                if (context.User.Identity.Name.IsNullOrEmpty().Equals(false))
                {
                    // Redirect to the default page for logged-in users
                    context.Response.Redirect("/Home");
                }
                else
                {
                    // Redirect to the home page for non-logged-in users
                    context.Response.Redirect("/Index");
                }
            }
            else
            {
                await _next(context);
            }
        }
    }
}








