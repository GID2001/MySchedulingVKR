using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MySchedulingVKR;
using MySchedulingVKR.Models;
using static MySchedulingVKR.Models.Lesson;

namespace MySchedulingVKR.Controllers
{
    public class LessonsController : Controller
    {
        private readonly DbSchedulingContext _context;

        public LessonsController(DbSchedulingContext context)
        {
            _context = context;
        }

        //Список занятий
        // GET: Lessons
        public async Task<IActionResult> Index()
        {
            return View(await _context.Lessons.ToListAsync());
        }

        //Информация о занятии
        // GET: Lessons/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lesson = await _context.Lessons
                .FirstOrDefaultAsync(m => m.Id == id);
            if (lesson == null)
            {
                return NotFound();
            }

            return View(lesson);
        }
        
        //Создание занятия
        // GET: Lessons/Create
        public IActionResult Create()
        {
            ViewBag.DayOfWeeks = Enum.GetValues(typeof(DayOfWeekEnum))
                           .Cast<DayOfWeekEnum>()
                           .Select(d => new SelectListItem
                           {
                               Value = d.ToString(),
                               Text = d.ToString()
                           })
                           .ToList();
            return View();
        }
        // POST: Lessons/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,DayOfTheWeek,Time,Date")] Lesson lesson)
        {
            int TeacherId = Convert.ToInt32(TempData["TeacherId"]);

            if (ModelState.IsValid)
            {
                var teacherAccess = new TeacherAccess();
                teacherAccess.TeacherId = TeacherId;
                teacherAccess.LessonId = lesson.Id;
                teacherAccess.Access = true;

                lesson.TeacherAccesses.Add(teacherAccess);
                
                _context.Add(lesson);
                _context.Add(teacherAccess);

                await _context.SaveChangesAsync();

                return RedirectToAction("Menu", "Teachers", new { id = TeacherId });
            }

            return View(lesson);
        }

        //Редактирование занятия
        // GET: Lessons/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lesson = await _context.Lessons.FindAsync(id);
            if (lesson == null)
            {
                return NotFound();
            }
            return View(lesson);
        }
        // POST: Lessons/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,DayOfTheWeek,Time,Date")] Lesson lesson)
        {
            int TeacherId = Convert.ToInt32(TempData["TeacherId"]);
            if (id != lesson.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(lesson);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LessonExists(lesson.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Menu", "Teachers", new { id = TeacherId });
            }
            return View(lesson);
        }

        //Удаление занятия
        // GET: Lessons/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lesson = await _context.Lessons
                .FirstOrDefaultAsync(m => m.Id == id);
            if (lesson == null)
            {
                return NotFound();
            }

            return View(lesson);
        }
        // POST: Lessons/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            int TeacherId = Convert.ToInt32(TempData["TeacherId"]);
            var lesson = await _context.Lessons.FindAsync(id);
            if (lesson != null)
            {
                _context.Lessons.Remove(lesson);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Menu", "Teachers", new { id = TeacherId });
        }

        private bool LessonExists(int id)
        {
            return _context.Lessons.Any(e => e.Id == id);
        }
    }
}
