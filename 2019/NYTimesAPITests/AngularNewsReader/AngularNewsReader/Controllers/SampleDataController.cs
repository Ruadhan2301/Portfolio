using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace AngularNewsReader.Controllers
{
    [Route("api/[controller]")]
    public class SampleDataController : Controller
    {
        private static string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        [HttpGet("[action]")]
        public IEnumerable<WeatherForecast> WeatherForecasts()
        {
            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                DateFormatted = DateTime.Now.AddDays(index).ToString("d"),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            });
        }

        public class WeatherForecast
        {
            public string DateFormatted { get; set; }
            public int TemperatureC { get; set; }
            public string Summary { get; set; }

            public int TemperatureF
            {
                get
                {
                    return 32 + (int)(TemperatureC / 0.5556);
                }
            }
        }

        public class TimesArticle
        {
            public string _id { get; set; }
            public string _abstract { get; set; }
            public object blog { get; set; }
            public object byline { get; set; }
            public string document_type { get; set; }
            public object headline { get; set; }
            public object keywords { get; set; }
            public string lead_paragraph { get; set; }
            public object multimedia { get; set; }
            public string news_desk { get; set; }
            public string print_page { get; set; }
            public string pub_date { get; set; }
            public string section_name { get; set; }
            public string snippet { get; set; }
            public string source { get; set; }
            public string type_of_material { get; set; }
            public string uri { get; set; }
            public string web_url { get; set; }
            public string word_count { get; set; }


            /*public int Thumbnail
            {
                get
                {
                    return multimedia.thumbnail;
                }
            }*/
        }

    }
}
