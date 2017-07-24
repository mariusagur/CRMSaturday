using CrmSaturdayOsloWeb.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace CrmSaturdayOsloWeb.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly crmsatoslo_dbContext context;
        public HomeController(crmsatoslo_dbContext context)
        {
            this.context = context;
        }
        [AllowAnonymous]
        public IActionResult Index()
        {
            var sessions = context.Sessions.Where(s => s.Schedule.HasValue && s.Track > 0).ToList();

            var schedule = new List<ScheduleDTO>();
            sessions.ForEach(s =>
            {
                var model = new ScheduleDTO(s)
                {
                    Speakers = context.SessionSpeakers
                    .Where(ss => ss.Session.Equals(s))
                    .Select(ss => ss.Speaker).ToArray()
                };
                schedule.Add(model);
            });

            return View(schedule);
        }

        [AllowAnonymous]
        public IActionResult About()
        {
            ViewData["Message"] = "About CRMSaturday.";

            return View();
        }

        [AllowAnonymous]
        public IActionResult Contact()
        {
            ViewData["Message"] = "Contact information.";

            return View();
        }

        [AllowAnonymous]
        public IActionResult Error()
        {
            return View();
        }
    }
}
