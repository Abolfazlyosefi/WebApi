using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace GitHubApiApp
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            const string url = "https://api.github.com/orgs/dotnet/repos";

            try
            {
                var repositories = await FetchRepositoriesAsync(url);

                Console.WriteLine("=== Repositories under .NET Foundation ===\n");

                foreach (var repo in repositories)
                {
                    Console.WriteLine($"Name: {repo.Name}");
                    Console.WriteLine($"Homepage: {repo.Homepage ?? "No homepage"}");
                    Console.WriteLine($"GitHub: {repo.HtmlUrl}");
                    Console.WriteLine($"Description: {repo.Description ?? "No description"}");
                    Console.WriteLine($"Watchers: {repo.Watchers:N0}");
                    Console.WriteLine($"Last push: {repo.PushedAt:yyyy-MM-dd HH:mm:ss}\n");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        private static async Task<List<Repository>> FetchRepositoriesAsync(string url)
        {
            using var httpClient = new HttpClient();

            // GitHub API requires a User-Agent header
            httpClient.DefaultRequestHeaders.Add("User-Agent", "StudentApp");

            var response = await httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
                throw new Exception($"Failed to fetch data: {response.StatusCode}");

            var json = await response.Content.ReadAsStringAsync();

            return JsonSerializer.Deserialize<List<Repository>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        }

        public class Repository
        {
            [JsonPropertyName("name")]
            public string Name { get; set; }

            [JsonPropertyName("description")]
            public string Description { get; set; }

            [JsonPropertyName("html_url")]
            public string HtmlUrl { get; set; }

            [JsonPropertyName("homepage")]
            public string Homepage { get; set; }

            [JsonPropertyName("watchers")]
            public int Watchers { get; set; }

            [JsonPropertyName("pushed_at")]
            public DateTime PushedAt { get; set; }


        }
    }
}
