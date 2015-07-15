using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ElucidatClient.Models;
using Newtonsoft.Json;

namespace Demo {
    class Program {
        static void Main(string[] args) {
            var publicKey = ConfigurationManager.AppSettings["publicKey"];
            var secretKey = ConfigurationManager.AppSettings["secretKey"];
            var baseUrl = ConfigurationManager.AppSettings["baseUrl"];

            var projectCode = ConfigurationManager.AppSettings["projectCode"];
            var releaseCode = ConfigurationManager.AppSettings["releaseCode"];
            var authorEmail = ConfigurationManager.AppSettings["authorEmail"];

            var client = new ElucidatClient.Elucidat(publicKey, secretKey, true, baseUrl);

            var projects = client.GetProjects();

            Console.WriteLine(JsonConvert.SerializeObject(projects, Formatting.Indented));
            Console.ReadLine();

            Console.WriteLine(JsonConvert.SerializeObject(client.GetReleases(projectCode), Formatting.Indented));
            Console.ReadLine();

            Console.WriteLine(JsonConvert.SerializeObject(client.GetRelease(releaseCode), Formatting.Indented));
            Console.ReadLine();

            Console.WriteLine(JsonConvert.SerializeObject(client.CreateReleaseByReleaseCode(releaseCode, "online-private"))); 
            Console.ReadLine();

            Console.WriteLine(JsonConvert.SerializeObject(client.CreateReleaseByProjectCode(projectCode, "online-private")));
            Console.ReadLine();

            Console.WriteLine(JsonConvert.SerializeObject(client.GetLaunchLink(releaseCode), Formatting.Indented));
            Console.ReadLine();

            Console.WriteLine(JsonConvert.SerializeObject(client.GetPollResults(releaseCode), Formatting.Indented));
            Console.ReadLine();

            Console.WriteLine(JsonConvert.SerializeObject(client.GetAccount(), Formatting.Indented));
            Console.ReadLine();

            Console.WriteLine(JsonConvert.SerializeObject(client.UpdateAccount(new AccountDetailModel {
                FirstName = "Joe",
                LastName = "Bloggs",
                CompanyEmail =  "joe.bloggs@example.com",
                Telephone = "01234 567890",
                Address1 = "1 Example rd.",
                Postcode = "EX4 7PL",
                Country = "GB"
            }), Formatting.Indented));
            Console.ReadLine();

            Console.WriteLine(JsonConvert.SerializeObject(client.GetAuthors(), Formatting.Indented));
            Console.ReadLine();

            Console.WriteLine(JsonConvert.SerializeObject(client.CreateAuthor(new AuthorDetailModel {
                FirstName = "Joe",
                LastName = "Bloggs",
                Email = "joe.bloggs@example.com"
            }), Formatting.Indented));
            Console.ReadLine();

            Console.WriteLine(JsonConvert.SerializeObject(client.ChangeAuthorRole(authorEmail, "editor"))); 
            Console.ReadLine();

            Console.WriteLine(JsonConvert.SerializeObject(client.DeleteAuthor("joe.bloggs@example.com")));
            Console.ReadLine();

            Console.WriteLine(JsonConvert.SerializeObject(client.GetEventSubscriptions()));
            Console.ReadLine();

            Console.WriteLine(JsonConvert.SerializeObject(client.SubscribeEvent("release_course", "http://test.com/release_course")));
            Console.ReadLine();

            Console.WriteLine(JsonConvert.SerializeObject(client.UnsubscribeEvent("release_course")));
            Console.ReadLine();
        }
    }
}
