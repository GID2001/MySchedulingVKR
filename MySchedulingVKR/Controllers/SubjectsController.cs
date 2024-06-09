using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MySchedulingVKR;
using MySchedulingVKR.Models;

namespace MySchedulingVKR.Controllers
{
    public class SubjectsController : Controller
    {
        private readonly DbSchedulingContext _context;

        public SubjectsController(DbSchedulingContext context)
        {
            _context = context;
        }

        // GET: Subjects
        public async Task<IActionResult> Index()
        {
            return View(await _context.Subjects.ToListAsync());
        }

        // GET: Subjects/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var subject = await _context.Subjects
                .FirstOrDefaultAsync(m => m.Id == id);
            if (subject == null)
            {
                return NotFound();
            }

            return View(subject);
        }

        // GET: Subjects/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Subjects/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name")] Subject subject)
        {
            int organId = Convert.ToInt32(TempData["OrganizationId"]);
            OrganizationSubject organizationSubject = new OrganizationSubject();

            if (ModelState.IsValid)
            {
                var existingSubject = await _context.Subjects.FirstOrDefaultAsync(o => o.Name == subject.Name);
                
                if (existingSubject != null)
                {
                    organizationSubject.SubjectId = existingSubject.Id;
                    organizationSubject.OrganizationId = organId;
                    _context.OrganizationSubjects.Add(organizationSubject);
                    await _context.SaveChangesAsync();

                    return RedirectToAction("Menu", "Organizations", new { id = organId });
                }
                else
                {
                    _context.Subjects.Add(subject);
                    await _context.SaveChangesAsync();

                    organizationSubject.SubjectId = subject.Id;
                    organizationSubject.OrganizationId = organId;
                    _context.OrganizationSubjects.Add(organizationSubject);
                    await _context.SaveChangesAsync();

                    return RedirectToAction("Menu", "Organizations", new { id = organId });
                }
            }

            return View(subject);
        }

        // GET: Subjects/Edit/5        
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var subject = await _context.Subjects.FindAsync(id);

            if (subject == null)
            {
                return NotFound();
            }
            return View(subject);
        }

        // POST: Subjects/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name")] Subject subject)
        {
            if (id != subject.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(subject);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SubjectExists(subject.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                int organizationId = Convert.ToInt32(TempData["OrganizationId"]);
                return RedirectToAction("Menu", "Organizations", new { id =  organizationId});
            }
            return View(subject);
        }

        // GET: Subjects/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var subject = await _context.Subjects
                .FirstOrDefaultAsync(m => m.Id == id);
            if (subject == null)
            {
                return NotFound();
            }

            return View(subject);
        }

        // POST: Subjects/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var subject = await _context.Subjects.FindAsync(id);
            if (subject != null)
            {
                _context.Subjects.Remove(subject);
            }

            await _context.SaveChangesAsync();
            int organizationId = Convert.ToInt32(TempData["OrganizationId"]);
            return RedirectToAction("Menu", "Organizations", new { id = organizationId });
        }

        private bool SubjectExists(int id)
        {
            return _context.Subjects.Any(e => e.Id == id);
        }
    }
}
