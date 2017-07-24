using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CrmSaturdayOsloWeb.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using System;

namespace CrmSaturdayOsloWeb.Controllers
{
    [Authorize]
    public class SessionsController : Controller
    {
        private readonly crmsatoslo_dbContext Context;

        public SessionsController(crmsatoslo_dbContext context)
        {
            Context = context;    
        }

        // GET: Sessions
        public async Task<IActionResult> Index()
        {
            return View(await Context.Sessions.ToListAsync());
        }

        // GET: Sessions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var session = await Context.Sessions
                .SingleOrDefaultAsync(m => m.SessionId == id);
            if (session == null)
            {
                return NotFound();
            }
            ViewBag.Speakers = Context.SessionSpeakers.Where(s => s.SessionId == id).Select(s => s.Speaker).ToList();
            return View(session);
        }

        // GET: Sessions/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Sessions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("SessionId,Title,Description,Schedule,HandoutsUrl,Track")] Sessions session)
        {
            if (ModelState.IsValid)
            {
                Context.Add(session);
                await Context.SaveChangesAsync();
                return RedirectToAction("Details", new { Id = session.SessionId });
            }
            return View(session);
        }

        // GET: Sessions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sessions = await Context.Sessions.SingleOrDefaultAsync(m => m.SessionId == id);
            if (sessions == null)
            {
                return NotFound();
            }
            return View(sessions);
        }

        // POST: Sessions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("SessionId,Title,Description,Schedule,HandoutsUrl,Track")] Sessions session)
        {
            if (id != session.SessionId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    Context.Update(session);
                    await Context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SessionsExists(session.SessionId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Index");
            }
            return View(session);
        }

        // GET: Sessions/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sessions = await Context.Sessions
                .SingleOrDefaultAsync(m => m.SessionId == id);
            if (sessions == null)
            {
                return NotFound();
            }

            return View(sessions);
        }

        // POST: Sessions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var sessions = await Context.Sessions.SingleOrDefaultAsync(m => m.SessionId == id);
            Context.Sessions.Remove(sessions);
            await Context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        // GET: Sessions/AddSpeakers/5
        public IActionResult AddSpeakers(int sessionId)
        {
            var existingSpeakers = Context.SessionSpeakers.Where(s => s.SessionId == sessionId).Select(s => s.SpeakerId);
            var speakers = Context.Speakers.Where(s => !existingSpeakers.Contains(s.SpeakerId)).ToList();
            ViewBag.SessionId = sessionId;
            return View(speakers);
        }

        // POST: Sessions/AddSpeakers/5
        [HttpPost, ActionName("AddSpeakers")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddSpeakers(IFormCollection form, int sessionId)
        {
            if (form.TryGetValue("speakerCheckBox", out StringValues speakers))
            {
                var speakersToAdd = speakers.Where(s => !s.Equals("false", StringComparison.CurrentCultureIgnoreCase));
                foreach (var speakerId in speakersToAdd)
                {
                    var sessionSpeaker = new SessionSpeakers
                    {
                        Session = await Context.Sessions.FirstAsync(s => s.SessionId == sessionId),
                        Speaker = await Context.Speakers.FirstAsync(s => s.SpeakerId == int.Parse(speakerId))
                    };

                    Context.Add(sessionSpeaker);
                }
                await Context.SaveChangesAsync();
                return RedirectToAction("Details", new { id = sessionId });
            }

            return Ok();
        }

        // POST: Sessions/RemoveSpeaker/5
        [HttpPost, ActionName("RemoveSpeaker")]
        public async Task<IActionResult> RemoveSpeaker(int sessionId, int speakerId)
        {
            var sessionSpeaker = await Context.SessionSpeakers.FirstAsync(s => s.SessionId == sessionId && s.SpeakerId == speakerId);
            Context.Remove(sessionSpeaker);
            await Context.SaveChangesAsync();

            return RedirectToAction("Details", new { id = sessionId });
        }

        private bool SessionsExists(int id)
        {
            return Context.Sessions.Any(e => e.SessionId == id);
        }
    }
}
