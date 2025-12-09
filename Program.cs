using HtmlAgilityPack;
using Newtonsoft.Json;
using System.IO;
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

            //// Load HTML
            //var html = new HtmlDocument();
            //html.LoadHtml(body);

            //// XPaths
            //var xpaths = new[]
            //{
            //    "//*[@id=\"root\"]/",
            //    "//*[@id=\"root\"]/div/div[1]/div[2]/div[2]/div[2]/div[2]/div/table/tbody/tr/td[1]/span",
            //    "//*[@id=\"root\"]/div/div[1]/div[2]/div[2]/div[2]/div[2]/div/table/tbody/tr/td[1]/span"
            //};

            //var results = new List<string>();

            //foreach (var xp in xpaths)
            //{
            //    var node = html.DocumentNode.SelectSingleNode(xp);
            //    if (node != null)
            //        results.Add(node.InnerText.Trim());
            //    else
            //        results.Add("[NOT FOUND]");
            //}

            Console.WriteLine(body.ToString());

            string path = "html-output.txt";

            File.WriteAllText(path, body.ToString());

            //// Print results
            //foreach (var item in results)
            //{
            //    Console.WriteLine(item);
            //}

            break;
        }
    }
}
