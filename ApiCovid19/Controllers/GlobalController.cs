using Covid_19Data.Models;
using Covid19Api.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net;

namespace Covid_19Data.Controllers
{
    [Route("api/[controller]/[Action]")]
    [ApiController]
    public class GlobalController : ControllerBase
    {
        List<DataCovid> rootobject = new List<DataCovid>();
        string url = "https://api.covid19api.com/world";

        [HttpGet]
        public ActionResult<Rootobject> GetTotalDeathsLastSixsMonth()
        {

            rootobject = GetModels(url);

            var ro = rootobject.OrderBy(c => c.Date).ToList();
            var r = rootobject
                .Where(c => c.Date.Day == 1 || c.Date.DayOfYear == DateTime.Now.DayOfYear)
                .OrderBy(c => c.Date)
                .Select(c => new { NewDeaths = c.NewDeaths, Month = c.Date.Month })
                .ToList();

            return Ok(r);
        }

        [HttpGet]
        public ActionResult<Rootobject> GetTotalConfirmdLastSixsMonth()
        {

            rootobject = GetModels(url);

            var ro = rootobject.OrderBy(c => c.Date).ToList();
            var r = rootobject
                .Where(c => c.Date.Day == 1 || c.Date.DayOfYear == DateTime.Now.DayOfYear)
                .OrderBy(c => c.Date)
                .Select(c => new { NewConfirmd = c.NewConfirmed, Month = c.Date.Month })
                .ToList();

            return Ok(r);
        }

        [HttpGet]
        public ActionResult<Rootobject> GetTotalDeathd()
        {
            rootobject = GetModels(url);

            var r = rootobject.Select(r => new { TotalDeats = r.TotalDeaths, Date = r.Date })
                .OrderBy(c => c.Date)
                .TakeLast(1)
                .ToList();

            return Ok(r);
        }

        [HttpGet]
        public ActionResult<Rootobject> GetTotalConfirmd()
        {
            rootobject = GetModels(url);

            var r = rootobject.Select(r => new { TotalConfirmd = r.TotalConfirmed, Date = r.Date })
                .OrderBy(c => c.Date)
                .TakeLast(1)
                .ToList();

            return Ok(r);
        }

        private static List<DataCovid> GetModels(string url)
        {
            WebClient client = new WebClient();

            List<DataCovid>? rootobject;
            var json = client.DownloadString(url);
            rootobject = JsonConvert.DeserializeObject<List<DataCovid>>(json);
            return rootobject;
        }
    }
}
