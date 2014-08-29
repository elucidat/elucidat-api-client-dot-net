using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace ExampleClient {
    public class Elucidat {
        private readonly string publicKey;
        private readonly string privateKey;
        private readonly string baseUrl;

        /// <summary>
        /// Create and configure client service
        /// </summary>
        /// <param name="baseUrl">base URL of the elucidat API ( usually https://api.elucidat.com )</param>
        /// <param name="publicKey">your public key</param>
        /// <param name="privateKey">your private key</param>
        public Elucidat(string baseUrl, string publicKey, string privateKey) {
            this.baseUrl = baseUrl;
            this.privateKey = privateKey;
            this.publicKey = publicKey;
        }

        /// <summary>
        /// Call any API method
        /// </summary>
        /// <typeparam name="T">type of the returned API data</typeparam>
        /// <param name="method">relative URL of the method to call, including query string parameters if any</param>
        /// <returns>API data of type T</returns>
        private T Get<T>(string method) { 
            // expiry date
            var expiry = DateTime.UtcNow.AddMinutes(15).ToString("yyyy/MM/dd HH:mm:ss+00:00");

            // initialize encryption
            var hmac = new HMACSHA256(Encoding.ASCII.GetBytes(privateKey));
            hmac.Initialize();

            // build checkstring
            var checkstring = baseUrl + "/" + method + "GET" + expiry;
            // sign it
            var signature = BitConverter.ToString(hmac.ComputeHash(Encoding.ASCII.GetBytes(checkstring))).Replace("-", "").ToLower();

            // initialize HTTP client
            using (var client = new HttpClient()) {
                // configure request
                client.BaseAddress = new Uri(baseUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("X-Public", publicKey);
                client.DefaultRequestHeaders.Add("X-Expires", expiry);               
                client.DefaultRequestHeaders.Add("X-Signature", signature);

                // make the API call
                var response = client.GetAsync(method).Result;

                // throw exception on HTTP error
                response.EnsureSuccessStatusCode();

                // return respose
                return response.Content.ReadAsAsync<T>().Result;
            }
        }

        /// <summary>
        /// Get account details
        /// </summary>
        /// <returns>account details</returns>
        public Account GetAccount() {
            return Get<Account>("account");
        }

        public class Account {
            public string company_name;
            public string company_email;
            public string subscription_level;
            public DateTime created;
            public int course_views_this_month;
            public string course_views_allowed_per_month;
            public int total_course_views;
            public int project_passes;
            public int project_fails;
            public int page_views_this_month;
            public int total_page_views;
            public int projects_allowed_in_account;
            public int projects_used_in_account;
            public int authors_allowed_in_account;
            public int authors_used_in_account;
        }

        /// <summary>
        /// List projects
        /// </summary>
        /// <returns>a list of projects</returns>
        public IList<Project> GetProjects() {
            return Get<IList<Project>>("projects");
        }

        public class Project {
            public string project_code;
            public string name;
            public DateTime created;
            public int project_views;
            public int project_views_this_month;
            public int project_passes;
            public int project_fails;
            public string theme;
            public int theme_version_number;
        }

        /// <summary>
        /// List releases for a given project
        /// </summary>
        /// <param name="projectCode"></param>
        /// <returns>a list of releases</returns>
        public IList<Release> GetReleases(string projectCode) {
            return Get<IList<Release>>(String.Format("projects/{0}/releases", projectCode));
        }

        public class Release {
            public string release_code;
            public DateTime created;
            public string status;
            public string description;
            public int release_views;
            public int release_views_this_month;
            public int release_passes;
            public int release_fails;
            public DateTime? release_last_view;
            public int version_number;
            public string release_mode;
        }

        /// <summary>
        /// Extended details for a given release
        /// </summary>
        /// <param name="releaseCode"></param>
        /// <returns></returns>
        public ReleaseDetails GetRelease(string releaseCode) {
            return Get<ReleaseDetails>(String.Format("releases/{0}", releaseCode));
        }

        public class ReleaseDetails : Release {
            public ProjectDetails project;
        }

        public class ProjectDetails : Project {
            public string project_key;
        }

        /// <summary>
        /// Get poll results for a given release
        /// </summary>
        /// <param name="releaseCode"></param>
        /// <returns></returns>
        public IList<IDictionary<string, string>> GetPollResults(string releaseCode) {
            return Get<IList<IDictionary<string, string>>>(String.Format("releases/{0}/answers", releaseCode));
        }

        /// <summary>
        /// Get a launch link for the given release
        /// </summary>
        /// <param name="releaseCode"></param>
        /// <returns></returns>
        public TypedLink GetLaunchLink(string releaseCode) {
            return Get<TypedLink>(String.Format("releases/{0}/launch", releaseCode));
        }

        public class TypedLink : Link {
            public string link_type;
        }

        /// <summary>
        /// Get a deep link into the Elucidat administration area.
        /// </summary>
        /// <param name="type">type of link</param>
        /// <param name="email">user email</param>
        /// <returns>A url</returns>
        public Link GetDeepLink(string type, string email) {
            return Get<Link>(String.Format("deeplink/{0}?email_address={1}", type, HttpUtility.UrlEncode(email)));
        }

        public class Link {
            public string url;
        }
    }
}
