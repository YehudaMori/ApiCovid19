using Covid_19Data.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net;


namespace Covid19Api.Controllers
{
    [Route("api/[controller]/[Action]")]
    [ApiController]
    public class CountriesController : ControllerBase
    {
        string url = $"https://api.covid19api.com/summary";

        [HttpGet]
        public ActionResult<Rootobject> CountriesByNewPositivs()
        {
            try
            {
                Rootobject? Root = GetModel(url);

                var SummaryCountrys = Root.Countries.
                    OrderByDescending(c => c.NewConfirmed)
                    .Select(c => new { Cuntry = c.Country, NewConfirmd = c.NewConfirmed })
                    .ToList();

                return Ok(SummaryCountrys);
            }
            catch
            {
                return NotFound();
            }

        }

        [HttpGet]
        public ActionResult<Rootobject> CountriesByNewDeaths()
        {
            try
            {
                Rootobject? Root = GetModel(url);

                var SummaryCountrys = Root.Countries.
                    OrderByDescending(c => c.NewDeaths)
                    .Select(c => new { Cuntry = c.Country, NewDeaths = c.NewDeaths })
                    .ToList();

                return Ok(SummaryCountrys);
            }
            catch
            {
                return NotFound();
            }
        }

        [HttpGet]
        public ActionResult<Rootobject> CountryByNewPositivs()
        {
            try
            {
                Rootobject? Root = GetModel(url);

                var israel = Root.Countries.OrderBy(c => c.Country).ToList();
                var Country = Root.Countries.
                    OrderByDescending(c => c.NewConfirmed)
                    .Select(c => new { Cuntry = c.Country, NewConfirmd = c.NewConfirmed })
                    .Take(1)
                    .ToList();

                return Ok(Country);
            }
            catch
            {
                return NotFound();
            }

        }

        [HttpGet]
        public ActionResult<Rootobject> CountryByNewDeath()
        {
            try
            {

                Rootobject? Root = GetModel(url);

                var israel = Root.Countries.OrderBy(c => c.Country).ToList();
                var Country = Root.Countries.
                    OrderByDescending(c => c.NewConfirmed)
                    .Select(c => new { Cuntry = c.Country, NewDeaths = c.NewDeaths })
                    .Take(1)
                    .ToList();

                return Ok(Country);
            }
            catch
            {
                return NotFound();
            }

        }

        [HttpGet]
        public ActionResult<CovidData> GetTotalByCountries(string country)
        {
            List<CovidData> rootobject = new List<CovidData>();
            try
            {
                var client = new WebClient();
                string url = $"https://api.covid19api.com/total/country/{country}/status/confirmed?from=10.10.2021&to={DateTime.Now}";
                var json = client.DownloadString(url);
                rootobject = JsonConvert.DeserializeObject<List<CovidData>>(json);

                var SummaryCountrys = rootobject
                    .Where(c => c.Date.Day == 1 || c.Date.DayOfYear == DateTime.Now.DayOfYear)
                    .TakeLast(7)
                    .ToList();

                return Ok(SummaryCountrys);
            }
            catch (Exception e)
            {
                rootobject = new List<CovidData>();
                return NotFound(e.Message);
            }

        }

        [HttpGet]
        public ActionResult<Rootobject> GetTotalDeathByCountries()
        {
            try
            {
                Rootobject? Root = GetModel(url);

                var TotalDeathByCountry = Root.Countries
                    .Select(c => new { CountryName = c.Country, TotalDeaths = c.TotalDeaths })
                    .OrderByDescending(c => c.TotalDeaths)
                    .ToList();

                return Ok(TotalDeathByCountry);
            }
            catch
            {
                return NotFound();
            }

        }

        [HttpGet]
        public ActionResult<Rootobject> GetTotalConfirmdByCountries()
        {
            try
            {
                Rootobject? Root = GetModel(url);

                var TotalDeathByCountry = Root.Countries
                    .Select(c => new { CountryName = c.Country, TotalConfirmd = c.TotalConfirmed })
                    .OrderByDescending(c => c.TotalConfirmd)
                    .ToList();

                return Ok(TotalDeathByCountry);
            }
            catch
            {
                return NotFound();
            }

        }

        private static Rootobject? GetModel(string url)
        {
            var client = new WebClient();
            var json = client.DownloadString(url);
           
            return JsonConvert.DeserializeObject<Rootobject>(json);
        }
    }
}
