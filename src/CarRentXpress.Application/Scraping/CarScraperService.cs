using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AngleSharp;
using CarRentXpress.Application.Services.Contracts;
using CarRentXpress.DTOs;

namespace CarRentXpress.Application.Scraping
{
    public class CarScraperService
    {
        private readonly ICarService _carService;

        public CarScraperService(ICarService carService)
        {
            _carService = carService;
        }

        public async Task ScrapeAndPersistCarsAsync()
        {
            var scrapedCars = new List<CarDto>();
             //Site name - Carjet
            //Adjust location, pick and return date
            //Then change the url

            //Settings right now:
            //Pick-up Location: London, Gatwick Airport (LGW)
            //Pick-up date: 11th April 2025
            //At: 10:30
            //Return Date: 23rd April 2025
            //At: 10:30
            string url =
                "https://www.carjet.com/do/list/en?s=f2c3f048-89e9-4a5a-b31f-56f13164e251&b=09b54ec3-0b69-4032-8d9f-8cf0d9f6619c";
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
                    Random rnd = new Random();
                    foreach (var article in articles)
                    {
                        int year = rnd.Next(2010, 2024);
                        string seatText = article.QuerySelector("ul.features li")?.GetAttribute("value") ?? "0";
                        int seatCount = int.Parse(seatText);
                        
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

                        Console.WriteLine($"Found Car - Brand: {brand}, Model {model}, Price: {pricePerDay}, Image: {imgUrl}, Seats Count: {seatCount}, Year: {year}");

                        var carDto = new CarDto
                        {
                            Id = Guid.NewGuid().ToString(),
                            Brand = brand,
                            Model = model,
                            Year = year,
                            Seats = seatCount,
                            PricePerDay = pricePerDay,
                            ImgUrl = imgUrl,
                        };

                        scrapedCars.Add(carDto);
                    }
                }
            }

            if (scrapedCars.Count > 0)
            {
                await _carService.AddCarsAsync(scrapedCars);
                Console.WriteLine($"{scrapedCars.Count} cars were added to the database.");
            }
            else
            {
                Console.WriteLine("No cars were scraped.");
            }
        }
    }
}
