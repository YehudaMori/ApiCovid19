using Covid_19Data.Models;
using Covid19Api.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net;

namespace Covid19Api.Controllers
{
    [Route("api/[controller]/[Action]")]
    [ApiController]
    public class InformationController : ControllerBase
    {
        string url = "https://api.covid19api.com/world";

        [HttpGet]
        public ActionResult<Rootobject> GetMaxNewConfirmd()
        {
            List<DataCovid>? rootobject = GetModel(url);

            var r = rootobject.Where(c => c.NewConfirmed == rootobject.Max(c => c.NewConfirmed))
                .Select(c => new { NumberOfNewConfirmd = c.NewConfirmed, Date = c.Date })
                .ToList();

            return Ok(r);
        }

        [HttpGet]
        public ActionResult<Rootobject> GetMaxNewDeaths()
        {
            var rootobject = GetModel(url);

            var r = rootobject.Where(c => c.NewDeaths == rootobject.Max(c => c.NewDeaths))
                .Select(c => new { NumberOfNewDeaths = c.NewDeaths, Date = c.Date })
                .ToList();

            return Ok(r);
        }

        [HttpGet]
        public ActionResult<Rootobject> GetMonthWithMostDeaths()
        {
            var rootobject = GetModel(url);

            var r = rootobject.OrderBy(c => c.Date).GroupBy(c => c.Date.Month)
                .Select(c => new { Month = c.Key, Sum = c.Sum(c => c.NewDeaths) })
                .OrderByDescending(c => c.Sum)
                .Take(1)
                .ToList();

            return Ok(r);
        }

        [HttpGet]
        public ActionResult<Rootobject> GetMonthWithMostConfirmd()
        {
            var rootobject = GetModel(url);

            var r = rootobject.OrderBy(c => c.Date).GroupBy(c => c.Date.Month)
                .Select(c => new { Month = c.Key, Sum = c.Sum(c => c.NewConfirmed) })
                .OrderByDescending(c => c.Sum)
                .Take(1)
                .ToList();

            return Ok(r);
        }

        [HttpGet]
        public ActionResult<Rootobject> GetCountrysMonthWithoutCovid()
        {
            WebClient client = new WebClient();

            string url = $"https://api.covid19api.com/summary";
            var json = client.DownloadString(url);
            Rootobject Root = JsonConvert.DeserializeObject<Rootobject>(json);

            var Countrys = Root.Countries.Where(c => c.NewConfirmed == 0).Select(c => new { CountryName = c.Country }).OrderBy(c => c.CountryName).ToList();
            var rootobject = new List<CovidData>();
            List<string> CountryList = new List<string>();
            foreach (var item in Countrys)
            {
                Thread.Sleep(400);
                url = $"https://api.covid19api.com/country/{item.CountryName}/status/confirmed?from=2021-{DateTime.Now.AddMonths(-1).Month}-{DateTime.Now.Day}T00:00:00Z&to=2021-{DateTime.Now.Month}-{DateTime.Now.Day}T00:00:00Z";

                try
                {
                    json = client.DownloadString(url);
                    rootobject = JsonConvert.DeserializeObject<List<CovidData>>(json).ToList();

                    if (MonthWithoutCovid(rootobject))
                        CountryList.Add(item.CountryName);
                }
                catch { }

            }

            return Ok(CountryList);
        }

        private static List<DataCovid>? GetModel(string url)
        {
            WebClient client = new WebClient();
            var json = client.DownloadString(url);

            return JsonConvert.DeserializeObject<List<DataCovid>>(json);
        }

        private bool MonthWithoutCovid(List<CovidData> rootobject)
        {
            var c = rootobject.FirstOrDefault(c => c.Date == DateTime.Now.AddDays(-30).Date);
            var b = rootobject.FirstOrDefault(j => j.Date == DateTime.Now.Date);

            if (b != null)
                if (c.Cases == b.Cases)
                    return true;
            return false;
        }
    }
}
