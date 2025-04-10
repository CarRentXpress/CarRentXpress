using System.Text.RegularExpressions;
using AngleSharp;
using CarRentXpress.Application.Services;
using CarRentXpress.Application.Services.Interfaces;
using CarRentXpress.DTOs;
using Microsoft.Extensions.DependencyInjection;

namespace CarRentXpress.Scraper
{
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
                        string modelAndBrandInThePage = article.QuerySelector("h2")?.TextContent?.Trim() ?? "Not available";
                        var imgElement = article.QuerySelector("img");

                        string imgSrc = imgElement?.GetAttribute("data-original");

                        if (string.IsNullOrWhiteSpace(imgSrc))
                        {
                            imgSrc = imgElement?.GetAttribute("src");
                        }

                        string imgUrl = string.Empty;
                        if (!string.IsNullOrWhiteSpace(imgSrc))
                        {
                            if (imgSrc.StartsWith("http", StringComparison.OrdinalIgnoreCase))
                                imgUrl = imgSrc;
                            else
                            {
                                if (!imgSrc.StartsWith("/"))
                                    imgSrc = "/" + imgSrc;
                                imgUrl = "https://www.carjet.com" + imgSrc;
                            }
                        }

                        string pricePerDayText = article.QuerySelector("em.price-day-euros")?.TextContent?.Trim() ?? "0";
                        decimal pricePerDay = decimal.Parse(Regex.Replace(pricePerDayText, "[£€]", "").Trim());


                        string[] parts = modelAndBrandInThePage.Split(' ', StringSplitOptions.RemoveEmptyEntries).ToArray();

                        string brand = string.Empty;
                        string model = string.Empty;
                        if (parts.Length > 0)
                        {
                            brand = parts[0];
                            model = parts.Length > 1 ? string.Join(" ", parts.Skip(1)) : string.Empty;
                        }

                        Console.WriteLine($"Found Car - Brand: {brand}, Model {model}, Price: {pricePerDay}, Image: {imgUrl}");


                    }
                }
            }
        }
    }
}