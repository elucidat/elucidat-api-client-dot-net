using Newtonsoft.Json;
using System;
using System.Configuration;

namespace ExampleClient {
    class Program {
        static void Main(string[] args) {
            var publicKey = ConfigurationManager.AppSettings["publicKey"];
            var privateKey = ConfigurationManager.AppSettings["privateKey"];
            var emailAddress = ConfigurationManager.AppSettings["userEmail"];
            var apiUrl = "https://api.elucidat.com";

            var client = new Elucidat(
                apiUrl,
                publicKey,
                privateKey
            );

            // Account details
            var account = client.GetAccount();
            Console.Out.WriteLine(JsonConvert.SerializeObject(account, Formatting.Indented)); // print result
            Console.ReadLine(); // pause until enter key pressed

            // Project list
            var projects = client.GetProjects();
            Console.Out.WriteLine(JsonConvert.SerializeObject(projects, Formatting.Indented));
            Console.ReadLine();

            // Release list
            var releases = client.GetReleases(projects[0].project_code);
            Console.Out.WriteLine(JsonConvert.SerializeObject(releases, Formatting.Indented));
            Console.ReadLine();

            // Release details
            var details = client.GetRelease(releases[0].release_code);
            Console.Out.WriteLine(JsonConvert.SerializeObject(details, Formatting.Indented));
            Console.ReadLine();

            // Poll results
            var results = client.GetPollResults(details.release_code);
            Console.Out.WriteLine(JsonConvert.SerializeObject(results, Formatting.Indented));
            Console.ReadLine();

            // Launch link
            var launch = client.GetLaunchLink(details.release_code);
            Console.Out.WriteLine(JsonConvert.SerializeObject(launch, Formatting.Indented));
            Console.ReadLine();

            // Dashboard link
            var dashboard = client.GetDeepLink("dashboard", emailAddress);
            Console.Out.WriteLine(JsonConvert.SerializeObject(dashboard, Formatting.Indented));
            Console.ReadLine();

            // Themes link
            var themes = client.GetDeepLink("themes", emailAddress);
            Console.Out.WriteLine(JsonConvert.SerializeObject(themes, Formatting.Indented));
            Console.ReadLine();

            // Projects link
            var projectsLink = client.GetDeepLink("projects", emailAddress);
            Console.Out.WriteLine(JsonConvert.SerializeObject(projectsLink, Formatting.Indented));
            Console.ReadLine();

            // Project edit link
            var edit = client.GetDeepLink("projects/edit/" + projects[0].project_code, emailAddress);
            Console.Out.WriteLine(JsonConvert.SerializeObject(edit, Formatting.Indented));
            Console.ReadLine();

            // Project preview link
            var preview = client.GetDeepLink("projects/preview/" + projects[0].project_code, emailAddress);
            Console.Out.WriteLine(JsonConvert.SerializeObject(preview, Formatting.Indented));
            Console.ReadLine();

            // Release link
            var release = client.GetDeepLink("release/" + projects[0].project_code, emailAddress);
            Console.Out.WriteLine(JsonConvert.SerializeObject(release, Formatting.Indented));
            Console.ReadLine();

            // All releases link
            var allReleases = client.GetDeepLink("release", emailAddress);
            Console.Out.WriteLine(JsonConvert.SerializeObject(allReleases, Formatting.Indented));
            Console.ReadLine();

            // Account details link
            var accountLink = client.GetDeepLink("account_details", emailAddress);
            Console.Out.WriteLine(JsonConvert.SerializeObject(accountLink, Formatting.Indented));
            Console.ReadLine();
        }
    }
}
