using System.Text.RegularExpressions;
using AngleSharp;

namespace CarRentXpress.Scraper;

class Program
{
    static async Task Main()
    {
        //Site name - Carjet
        //Adjust location, pick and return date
        //Then change the url
        
        //Settings right now:
        //Pick-up Location: London, Gatwick Airport (LGW)
        //Pick-up date: 11th April 2025
        //At: 10:30
        //Return Date: 23rd April 2025
        //At: 10:30
        
        string url = "https://www.carjet.com/do/list/en?s=e122874d-fe6e-48a8-ac49-8ddefc4bee88&b=09b54ec3-0b69-4032-8d9f-8cf0d9f6619c";
 
        using (HttpClient client = new HttpClient())
        {
            HttpResponseMessage response = await client.GetAsync(url);
 
            string htmlContent = await response.Content.ReadAsStringAsync();
 
            var config = Configuration.Default.WithDefaultLoader();
            var context = BrowsingContext.New(config);
            var document = await context.OpenAsync(req => req.Content(htmlContent));
 
            var articles = document.QuerySelectorAll("article[data-order]");
            if (articles != null)
            {
                foreach (var article in articles)
                {
                    var order = article.GetAttribute("data-order");
                    Console.WriteLine($"Article with data-order: {order}");

                    string model = article.QuerySelector("h2")?.TextContent ?? "Not available";
                    
                    var imgElement = article.QuerySelector("img");
                    var imgSrc = imgElement?.GetAttribute("data-original") 
                                 ?? imgElement?.GetAttribute("src");
                    string imgUrl = imgSrc != null && imgSrc.StartsWith("http")
                        ? imgSrc
                        : "https://www.carjet.com" + imgSrc;

                    string pricePerDayText = article.QuerySelector("em.price-day-euros")?.TextContent.Trim() ?? "Not available";
                    decimal pricePerDay = decimal.Parse(Regex.Replace(pricePerDayText, "[£€]", "").Trim());

                    Console.WriteLine($"Model: {model}");
                    Console.WriteLine($"Image URL: {imgUrl}");
                    Console.WriteLine($"Price: {pricePerDay}");
                }
            }
           
        }
    }
}