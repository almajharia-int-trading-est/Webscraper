using HtmlAgilityPack;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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


            // Parse FlareSolverr JSON
            dynamic result = JsonConvert.DeserializeObject(body);

            // Extract actual HTML
            string htmlContent = result.solution.response;

            // Load HTML
            var html = new HtmlDocument();
            html.LoadHtml(htmlContent);



            //Console.WriteLine(html.DocumentNode.OuterHtml);

            // Product Title
            string title = html.DocumentNode.SelectSingleNode("//title")?.InnerText?.Trim();
            Console.WriteLine("Title: " + title);

            // Meta Description
            string description =
                html.DocumentNode.SelectSingleNode("//meta[@name='description']")
                ?.GetAttributeValue("content", "")
                ?.Trim();
            Console.WriteLine("Description: " + description);

            // Image
            string image =
                html.DocumentNode.SelectSingleNode("//meta[@property='og:image']")
                ?.GetAttributeValue("content", "");
            Console.WriteLine("Image: " + image);

            // Product SKU
            string sku =
                html.DocumentNode.SelectSingleNode("//script[@type='application/ld+json']")
                ?.InnerText;

            if (sku != null)
            {
                try
                {
                    var structured = JObject.Parse(sku);
                    Console.WriteLine("SKU: " + structured["sku"]);
                    Console.WriteLine("MPN: " + structured["mpn"]);
                }
                catch { }
            }




            string path = "html-output.txt";

            File.WriteAllText(path, html.DocumentNode.OuterHtml);

            //// Print results
            //foreach (var item in results)
            //{
            //    Console.WriteLine(item);
            //}

            break;
        }
    }
}
