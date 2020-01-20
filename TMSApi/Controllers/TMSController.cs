using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TMSEntities;

namespace TMSApi.Controllers
{
    [Route("api/[action]")]
    [ApiController]
    public class TMSController : ControllerBase
    {
        private readonly TMSContext _tmsContext;

        public TMSController(TMSContext context)
        {
            _tmsContext = context;
        }

        public ActionResult GetAllListings()
        {
            return new JsonResult(_tmsContext.Listings.ToList());
        }

        public ActionResult GetAllListingsByTerm(string term)
        {
            return new JsonResult(_tmsContext.Listings.Where(x => x.Term.LookupLabel == term));
        }

        public ActionResult GetAllListingsBySubjectCourse(string subject, string courseNumber)
        {
            return new JsonResult(_tmsContext.Listings.Where(x => x.Subject == subject && x.CourseNumber == courseNumber));
        }

        public async Task<ActionResult> ScrapeTMS()
        {
            var scraper = new TMSScraper();
            var result = await scraper.Execute(_tmsContext);
            return StatusCode(200);
        }
    }
}