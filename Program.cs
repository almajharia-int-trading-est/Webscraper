using Newtonsoft.Json;
using System.Net.Http;
using System.Text;

class Program
{
    static async Task Main(string[] args)
    {
        var proxies = File.ReadAllLines("proxies.txt").Where(x => !string.IsNullOrWhiteSpace(x)).ToList();

        foreach (var proxy in proxies)
        {
            Console.WriteLine($"Using proxy: {proxy}");

            using var http = new HttpClient();
            var requestBody = new
            {
                cmd = "request.get",
                url = "https://www.thermofisher.com/order/catalog/product/E1041",
                session = "my-session",
                proxy = new
                {
                    url = $"{proxy}"
                }
            };

            var json = JsonConvert.SerializeObject(requestBody);
            var response = await http.PostAsync("http://localhost:8191/v1",
                new StringContent(json, Encoding.UTF8, "application/json"));

            var body = await response.Content.ReadAsStringAsync();
            Console.WriteLine(body);
        }
    }
}
