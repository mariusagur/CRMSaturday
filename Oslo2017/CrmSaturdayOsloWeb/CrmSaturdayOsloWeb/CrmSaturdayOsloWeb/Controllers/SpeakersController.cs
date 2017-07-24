using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CrmSaturdayOsloWeb.Models;
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.AspNetCore.Authorization;

namespace CrmSaturdayOsloWeb.Controllers
{
    [Authorize]
    public class SpeakersController : Controller
    {
        private readonly crmsatoslo_dbContext context;

        public SpeakersController(crmsatoslo_dbContext context)
        {
            this.context = context;
        }

        [AllowAnonymous]
        // GET: Speakers
        public async Task<IActionResult> Index()
        {
            var speakers = await context.Speakers.OrderBy(s => s.LastName).ToListAsync();
            return View(speakers);
        }

        [AllowAnonymous]
        // GET: Speakers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var speakers = await context.Speakers
                .SingleOrDefaultAsync(m => m.SpeakerId == id);
            if (speakers == null)
            {
                return NotFound();
            }

            return View(speakers);
        }

        // GET: Speakers/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Speakers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("SpeakerId,FirstName,LastName,Company,BlogUrl,Titles,Bio,TwitterHandle")] Speakers speaker)
        {
            if (ModelState.IsValid)
            {
                speaker.TwitterHandle = !string.IsNullOrWhiteSpace(speaker.TwitterHandle) && speaker.TwitterHandle.StartsWith("@") ?
                        speaker.TwitterHandle : $"@{speaker.TwitterHandle}";
                context.Add(speaker);
                await context.SaveChangesAsync();
                return RedirectToAction("Details", new { id = speaker.SpeakerId });
            }
            return View(speaker);
        }

        // GET: Speakers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var speakers = await context.Speakers.SingleOrDefaultAsync(m => m.SpeakerId == id);
            if (speakers == null)
            {
                return NotFound();
            }
            return View(speakers);
        }

        // POST: Speakers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("SpeakerId,FirstName,MiddleName,LastName,Company,BlogUrl,Titles,Bio,TwitterHandle")] Speakers speaker)
        {
            if (id != speaker.SpeakerId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existing = await context.Speakers.AsNoTracking().FirstAsync(s => s.SpeakerId == speaker.SpeakerId);
                    speaker.ProfilePicture = existing.ProfilePicture;
                    speaker.ProfilePictureExtension = existing.ProfilePictureExtension;
                    speaker.TwitterHandle = speaker.TwitterHandle.StartsWith("@") ? 
                        speaker.TwitterHandle : $"@{speaker.TwitterHandle}";
                    context.Update(speaker);
                    await context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SpeakersExists(speaker.SpeakerId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Details", new { id = speaker.SpeakerId });
            }
            return View(speaker);
        }

        // GET: Speakers/UploadImage/5
        public async Task<IActionResult> UploadImage(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var speakers = await context.Speakers.SingleOrDefaultAsync(m => m.SpeakerId == id);
            if (speakers == null)
            {
                return NotFound();
            }
            return View(speakers);
        }

        // GET: Speakers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var speakers = await context.Speakers
                .SingleOrDefaultAsync(m => m.SpeakerId == id);
            if (speakers == null)
            {
                return NotFound();
            }

            return View(speakers);
        }

        // POST: Speakers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var speakers = await context.Speakers.SingleOrDefaultAsync(m => m.SpeakerId == id);
            context.Speakers.Remove(speakers);
            await context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        [HttpPost, ActionName("FileUpload")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> FileUpload(int speakerId, IFormFile file)
        {
            if (file != null)
            {
                var pic = System.IO.Path.GetFileName(file.FileName);
                var extension = file.FileName.Split('.').Last();

                using (var ms = new MemoryStream())
                {
                    await file.CopyToAsync(ms);
                    var speaker = await context.Speakers.SingleOrDefaultAsync(s => s.SpeakerId == speakerId);

                    speaker.ProfilePicture = ms.ToArray();
                    speaker.ProfilePictureExtension = extension;

                    context.Update(speaker);
                    context.SaveChanges();
                }
            }
            // after successfully uploading redirect the user
            return RedirectToAction("details", new { id = speakerId });
        }

        private bool SpeakersExists(int id)
        {
            return context.Speakers.Any(e => e.SpeakerId == id);
        }
    }
}
