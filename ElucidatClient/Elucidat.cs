using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using ElucidatClient.Models;
using ElucidatClient.Support;
using Newtonsoft.Json;

namespace ElucidatClient {
    public class Elucidat {

        private readonly string publicKey;
        private readonly string secretKey;
        private readonly bool simulationMode;
        private readonly Uri baseUrl;

        private readonly JsonMediaTypeFormatter[] jsonFormatters = {
            new JsonMediaTypeFormatter {
                SerializerSettings = new JsonSerializerSettings {
                    ContractResolver = new UnderscoreContractResolver(),
                }
            }
        };

        /// <summary>
        /// Create and configure API client service
        /// </summary>
        /// <param name="publicKey"></param>
        /// <param name="secretKey"></param>
        /// <param name="simulationMode"></param>
        /// <param name="baseUrl"></param>
        public Elucidat(string publicKey, string secretKey, bool simulationMode, string baseUrl) {
            this.publicKey = publicKey;
            this.secretKey = secretKey;
            this.simulationMode = simulationMode;
            this.baseUrl = new Uri(baseUrl);
        }

        /// <summary>
        /// Call a GET API method returning a structure of type T without URL parameters
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url"></param>
        /// <returns></returns>
        private T Get<T>(string url) {
            return Get<T>(url, new Dictionary<string, string>());
        }

        /// <summary>
        /// Call a GET API method returning a structure of type T with URL parameters
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url"></param>
        /// <param name="fields"></param>
        /// <returns></returns>
        public T Get<T>(string url, IDictionary<string, string> fields) {
            if (simulationMode)
                fields = fields.Concat(new Dictionary<string, string> { { "simulation_mode", "simulation" } }).ToDictionary(x => x.Key, x => x.Value);

            return CallGet<T>(AuthHeaders(GetNonce(url)), fields, url);
        }

        /// <summary>
        /// Internal get-based API call. Used for both actual API call and obtaining nonces.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="headers"></param>
        /// <param name="fields"></param>
        /// <param name="url"></param>
        /// <returns></returns>
        private T CallGet<T>(IDictionary<string, string> headers, IDictionary<string, string> fields, string url) {
            var signedHeaders = headers.Concat(new Dictionary<string, string> { { "oauth_signature", Sign(headers.Concat(fields), url, "GET") } });

            using (var client = new HttpClient()) {
                client.BaseAddress = baseUrl;
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", BuildBaseString(signedHeaders, ",")); // non-standard authorization header
                client.DefaultRequestHeaders.Add("Expect", "");

                var response = client.GetAsync(url + "?" + BuildBaseString(fields, "&")).Result;

                if (!response.IsSuccessStatusCode) {
                    var body = response.Content.ReadAsStringAsync().Result;
                    throw new Exception(body);
                }

                return response.Content.ReadAsAsync<T>(jsonFormatters).Result;
            }
        }

        /// <summary>
        /// Call a POST-based API method with parameters
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url"></param>
        /// <param name="fields"></param>
        /// <returns></returns>
        public T Post<T>(string url, IDictionary<string, string> fields) {
            if (simulationMode)
                fields = fields.Concat(new Dictionary<string, string> { { "simulation_mode", "simulation" } }).ToDictionary(x => x.Key, x => x.Value);

            return CallPost<T>(AuthHeaders(GetNonce(url)), fields, url);
        }

        /// <summary>
        /// Internal POST call. Used only for actual API method invocations.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="headers"></param>
        /// <param name="fields"></param>
        /// <param name="url"></param>
        /// <returns></returns>
        private T CallPost<T>(IDictionary<string, string> headers, IDictionary<string, string> fields, string url) {
            var signedHeaders = headers.Concat(new Dictionary<string, string> { { "oauth_signature", Sign(headers.Concat(fields), url, "POST") } });

            using (var client = new HttpClient()) {
                client.BaseAddress = baseUrl;
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", BuildBaseString(signedHeaders, ",")); // non-standard authorization header
                client.DefaultRequestHeaders.Add("Expect", "");

                // We cannot use FormUrlEncodedContent here, because it doesn't encode parameters in the same way as the PHP code on
                // the recieving end. See also Support/StringExtensions.cs.
                var content = new StringContent(BuildBaseString(fields, "&"));
                content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");
                var response = client.PostAsync(url, content).Result;

                if (!response.IsSuccessStatusCode) {
                    var body = response.Content.ReadAsStringAsync().Result;
                    throw new Exception(body);
                }

                return response.Content.ReadAsAsync<T>(jsonFormatters).Result;
            }

        }

        /// <summary>
        /// Generate an HMAC signature for an API request
        /// </summary>
        /// <param name="parameters"></param>
        /// <param name="url"></param>
        /// <param name="httpMethod"></param>
        /// <returns></returns>
        private string Sign(IEnumerable<KeyValuePair<string, string>> parameters, string url, string httpMethod) {
            var baseString = httpMethod + "&" + new Uri(baseUrl, url) + "&" + BuildBaseString(parameters.OrderBy(x => x.Key), "&");

            var hmac = new HMACSHA1(Encoding.ASCII.GetBytes(secretKey.RawUrlEncode()));
            hmac.Initialize();

            return Convert.ToBase64String(hmac.ComputeHash(Encoding.ASCII.GetBytes(baseString)));
        }

        /// <summary>
        /// Create a query-string like single string from a parameter dictionary.
        /// </summary>
        /// <param name="parameters"></param>
        /// <param name="delimiter"></param>
        /// <returns></returns>
        private string BuildBaseString(IEnumerable<KeyValuePair<string, string>> parameters, string delimiter) {
            return String.Join(delimiter, parameters.Select(x => x.Key.RawUrlEncode() + "=" + x.Value.RawUrlEncode()));
        }

        /// <summary>
        /// Create a parameter dictionary from an object
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        private IDictionary<string, string> BuildParameterDictionary(object o) {
            var dict = new Dictionary<string, string>();

            foreach (var propertyInfo in o.GetType().FindMembers(MemberTypes.Property, BindingFlags.Instance | BindingFlags.Public, null, null)) {
                dict.Add(propertyInfo.Name.TransformPropertyName(), Convert.ToString(((PropertyInfo) propertyInfo).GetValue(o)));
            }

            return dict;
        } 

        /// <summary>
        /// Base authentication headers required for every API request.
        /// </summary>
        /// <param name="nonce"></param>
        /// <returns></returns>
        private IDictionary<string, string> AuthHeaders(string nonce) {
            var headers = new Dictionary<string, string> {
                {"oauth_consumer_key", publicKey},
                {"oauth_signature_method", "HMAC-SHA1"},
                {"oauth_timestamp", Math.Floor(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds).ToString()},
                {"oauth_version", "1.0"}
            };

            if (nonce != null)
                headers.Add("oauth_nonce", nonce);

            return headers;
        }

        /// <summary>
        /// Retrieve a security nonce from the API to authenticate the next method call.
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        private string GetNonce(string url) {
            return CallGet<NonceModel>(AuthHeaders(null), new Dictionary<string, string>(), url).Nonce;
        }

        /// <summary>
        /// Retrieve a list of projects
        /// </summary>
        /// <returns></returns>
        public IList<ProjectModel> GetProjects() {
            return Get<IList<ProjectModel>>("projects");
        }

        /// <summary>
        /// Retrieve account information
        /// </summary>
        /// <returns></returns>
        public AccountModel GetAccount() {
            return Get<AccountModel>("account");
        }

        /// <summary>
        /// Update account information
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public MessageModel UpdateAccount(AccountDetailModel data) {
            return Post<MessageModel>("account/update", BuildParameterDictionary(data));
        }

        /// <summary>
        /// Retrieve a list of authors
        /// </summary>
        /// <returns></returns>
        public IList<AuthorModel> GetAuthors() {
            return Get<IList<AuthorModel>>("authors");
        }

        /// <summary>
        /// Create a new author account
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public MessageModel CreateAuthor(AuthorDetailModel data) {
            return Post<MessageModel>("authors/create", BuildParameterDictionary(data));
        }

        /// <summary>
        /// Delete an author account
        /// </summary>
        /// <param name="authorEmail"></param>
        /// <returns></returns>
        public MessageModel DeleteAuthor(string authorEmail) {
            return Post<MessageModel>("authors/delete", new Dictionary<string, string> {
                {"email", authorEmail}
            });
        }

        /// <summary>
        /// Set an author's role
        /// </summary>
        /// <param name="authorEmail"></param>
        /// <param name="role"></param>
        /// <returns></returns>
        public MessageModel ChangeAuthorRole(string authorEmail, string role) {
            return Post<MessageModel>("authors/role", new Dictionary<string, string> {
                {"email", authorEmail},
                {"role", role}
            });
        }

        /// <summary>
        /// Retrieve a list of releases for a given project
        /// </summary>
        /// <param name="projectCode"></param>
        /// <returns></returns>
        public IList<ReleaseModel> GetReleases(string projectCode) {
            return Get<IList<ReleaseModel>>("releases", new Dictionary<string, string> { { "project_code", projectCode } });
        }

        /// <summary>
        /// Retrieve the details of a single release
        /// </summary>
        /// <param name="releaseCode"></param>
        /// <returns></returns>
        public ReleaseModel GetRelease(string releaseCode) {
            return Get<ReleaseModel>("releases/details", new Dictionary<string, string> { { "release_code", releaseCode } });
        }

        /// <summary>
        /// Re-release an existing release
        /// </summary>
        /// <param name="releaseCode"></param>
        /// <param name="releaseMode"></param>
        /// <returns></returns>
        public MessageModel CreateReleaseByReleaseCode(string releaseCode, string releaseMode) {
            return Post<MessageModel>("releases/create", new Dictionary<string, string> {
                {"release_code", releaseCode},
                {"release_mode", releaseMode}
            });
        }

        public MessageModel CreateReleaseByProjectCode(string projectCode, string releaseMode) {
            return Post<MessageModel>("releases/create", new Dictionary<string, string> {
                {"project_code", projectCode},
                {"release_mode", releaseMode}
            });
        }

        /// <summary>
        /// Retrieve a link which can be used to launch the give release.
        /// </summary>
        /// <param name="releaseCode"></param>
        /// <returns></returns>
        public LinkModel GetLaunchLink(string releaseCode) {
            return Get<LinkModel>("releases/launch", new Dictionary<string, string> { { "release_code", releaseCode } });
        }

        /// <summary>
        /// Get poll results for a given release
        /// </summary>
        /// <param name="releaseCode"></param>
        /// <returns></returns>
        public IList<PollResultsModel> GetPollResults(string releaseCode) {
            return Get<IList<PollResultsModel>>("releases/answers", new Dictionary<string, string> { { "release_code", releaseCode } });
        }

        /// <summary>
        /// Retrieve a list of events for which notification callbacks have been configured
        /// </summary>
        /// <returns></returns>
        public IDictionary<string, string> GetEventSubscriptions() {
            return Get<Dictionary<string, string>>("event");
        }

        /// <summary>
        /// Configure a notification callback url for a given event
        /// </summary>
        /// <param name="eventName"></param>
        /// <param name="url"></param>
        /// <returns></returns>
        public MessageModel SubscribeEvent(string eventName, string url) {
            return Post<MessageModel>("event/subscribe", new Dictionary<string, string> {
                {"event", eventName},
                {"callback_url", url}
            });
        }

        /// <summary>
        /// Remove the notification url for a given event
        /// </summary>
        /// <param name="eventName"></param>
        /// <returns></returns>
        public MessageModel UnsubscribeEvent(string eventName) {
            return Post<MessageModel>("event/unsubscribe", new Dictionary<string, string> { { "event", eventName } });
        }
    }
}
