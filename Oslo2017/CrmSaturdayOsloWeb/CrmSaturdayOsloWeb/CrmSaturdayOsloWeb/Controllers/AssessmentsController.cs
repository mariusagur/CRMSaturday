using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CrmSaturdayOsloWeb.Models;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Text.RegularExpressions;

namespace CrmSaturdayOsloWeb.Controllers
{
    [Authorize]
    public class AssessmentsController : Controller
    {
        private readonly crmsatoslo_dbContext Context;
        private readonly Regex EmailMatch = new Regex(@"\b[A-Za-z0-9.-_%+]+@[A-Za-z0-9.-]+[A-Za-z]{2,}\b");

        public AssessmentsController(crmsatoslo_dbContext context)
        {
            this.Context = context;    
        }

        // GET: Assessments
        public async Task<IActionResult> Index()
        {
            var crmsatoslo_dbContext = Context.Assessments.Include(a => a.Session);
            return View(await crmsatoslo_dbContext.ToListAsync());
        }

        // GET: Assessments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var assessments = await Context.Assessments
                .Include(a => a.Session)
                .SingleOrDefaultAsync(m => m.AssessmentId == id);
            if (assessments == null)
            {
                return NotFound();
            }

            return View(assessments);
        }

        // GET: Assessments/Create
        [AllowAnonymous]
        public IActionResult Create(int sessionId)
        {
            var assessment = new Assessments
            {
                SessionId = sessionId,
                Session = Context.Sessions.AsNoTracking().First(s => s.SessionId == sessionId)
            };
            return View(assessment);
        }

        // POST: Assessments/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Attendee,SessionId,SpeakerFeedback,SessionFeedback,Rating")] Assessments assessment)
        {
            if (!EmailMatch.IsMatch(assessment.Attendee))
            {
                return BadRequest("Email address is invalid");
            }
            if (ModelState.IsValid)
            {
                var existing = await Context.Assessments.AsNoTracking().FirstOrDefaultAsync(s => 
                    s.SessionId == assessment.SessionId && 
                    s.Attendee.Equals(assessment.Attendee, StringComparison.CurrentCultureIgnoreCase));
                if (existing != null)
                {
                    assessment.AssessmentId = existing.AssessmentId;
                    Context.Update(assessment);
                }
                else
                {
                    Context.Add(assessment);
                }
                await Context.SaveChangesAsync();
                return RedirectToAction(actionName: "Index", controllerName: "Home");
            }
            ViewData["SessionId"] = new SelectList(Context.Sessions, "SessionId", "Description", assessment.SessionId);
            return View(assessment);
        }

        // GET: Assessments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var assessments = await Context.Assessments.SingleOrDefaultAsync(m => m.AssessmentId == id);
            if (assessments == null)
            {
                return NotFound();
            }
            ViewData["SessionId"] = new SelectList(Context.Sessions, "SessionId", "Description", assessments.SessionId);
            return View(assessments);
        }

        // POST: Assessments/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("AssessmentId,Attendee,SessionId,SpeakerFeedback,SessionFeedback,Rating")] Assessments assessment)
        {
            if (id != assessment.AssessmentId)
            {
                return NotFound();
            }
            if (!EmailMatch.IsMatch(assessment.Attendee))
            {
                return BadRequest("Email address is invalid");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    Context.Update(assessment);
                    await Context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AssessmentsExists(assessment.AssessmentId))
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
            ViewData["SessionId"] = new SelectList(Context.Sessions, "SessionId", "Description", assessment.SessionId);
            return View(assessment);
        }

        // GET: Assessments/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var assessments = await Context.Assessments
                .Include(a => a.Session)
                .SingleOrDefaultAsync(m => m.AssessmentId == id);
            if (assessments == null)
            {
                return NotFound();
            }

            return View(assessments);
        }

        // POST: Assessments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var assessments = await Context.Assessments.SingleOrDefaultAsync(m => m.AssessmentId == id);
            Context.Assessments.Remove(assessments);
            await Context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        private bool AssessmentsExists(int id)
        {
            return Context.Assessments.Any(e => e.AssessmentId == id);
        }
    }
}
