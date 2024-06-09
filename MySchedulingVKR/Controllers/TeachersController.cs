using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MySchedulingVKR;
using MySchedulingVKR.Models;
using MySchedulingVKR.ViewModels;

namespace MySchedulingVKR.Controllers
{
    public class TeachersController : Controller
    {
        private readonly DbSchedulingContext _context;

        public TeachersController(DbSchedulingContext context)
        {
            _context = context;
        }

        //Авторизация преподавателя
        // GET: Organizations/Authorization
        public IActionResult Authorization()
        {
            return View();
        }
        // POST: Organizations/Authorization
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Authorization([Bind("Login,Password")] Teacher teacher)
        {

            var user = await _context.Teachers.FirstOrDefaultAsync(o => o.Login == teacher.Login && o.Password == teacher.Password);

            // Проверка введенных данных пользователя
            if (user != null)
            {
                return RedirectToAction("Menu", "Teachers", new { id = user.Id });
            }
            else
            {
                ViewBag.ErrorMessage = "Неверный логин или пароль";
                return View(user);
            }
        }

        //Меню преподавателя
        //GET: Teachers/MainPage
        public async Task<IActionResult> Menu(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var viewModel = new TeacherViewLesson();
            viewModel.Teachers = await _context.Teachers
                .Include(t => t.TeacherAccesses)
                    .ThenInclude(t => t.Lesson)
                .Include(o => o.Organization)
                .Include(t => t.Subject)
                .AsNoTracking()
                .ToListAsync();

            if (id != null)
            {
                ViewData["Id"] = id;

                TempData["TeacherId"] = id;

                Teacher teacher = viewModel.Teachers.Where(
                    o => o.Id == id).Single();

                viewModel.Teacher = teacher;

                viewModel.Lessons = teacher.TeacherAccesses.Select(o => o.Lesson);

                viewModel.TeachersAccesses = teacher.TeacherAccesses;
            }

            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> ScheduleTeachers(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var schedule = await _context.Teachers
                .Include(t => t.Subject)
                .Include(t => t.ScheduleTeachers)
                    .ThenInclude(t => t.Schedule)
                    .ThenInclude(s => s.Lesson)
                .Include(t => t.ScheduleTeachers)
                    .ThenInclude(t => t.Schedule)
                .FirstOrDefaultAsync(t => t.Id == id);
                
            return View(schedule);
        }

        //Список преподавателей
        // GET: Teachers
        public async Task<IActionResult> Index()
        {
            var dbSchedulingContext = _context.Teachers.Include(t => t.Organization).Include(t => t.Subject);
            return View(await dbSchedulingContext.ToListAsync());
        }

        //Информация о преподавателе
        // GET: Teachers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var teacher = await _context.Teachers
                .Include(t => t.Organization)
                .Include(t => t.Subject)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (teacher == null)
            {
                return NotFound();
            }

            return View(teacher);
        }

        //Создание преподавателя
        // GET: Teachers/Create
        [HttpGet]
        public IActionResult Create()
        {
            int organizationId = Convert.ToInt32(TempData["OrganizationId"]);
            Teacher teacher = new Teacher();
            teacher.OrganizationId = organizationId;

            var organization = _context.Organizations
                                          .Include(o => o.OrganizationSubjects)
                                              .ThenInclude(o => o.Subject)
                                          .FirstOrDefault(o => o.Id == organizationId);
            if (organization != null)
            {
                ViewData["SubjectName"] = new SelectList(organization.OrganizationSubjects.Select(o => o.Subject), "Id", "Name");
            }

            return View(teacher);
        }
        // POST: Teachers/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Login,Password,Name,Surname,SubjectId, OrganizationId")] Teacher teacher)
        {
            int organizationId = (int)teacher.OrganizationId;

            var existingTeacher = await _context.Teachers.FirstOrDefaultAsync(o => o.Login == teacher.Login);

            if (existingTeacher != null)
            {
                ModelState.AddModelError("Login", "Логин уже используется.");
                return View(teacher);
            }
            else if (ModelState.IsValid)
            {
                _context.Add(teacher);
                await _context.SaveChangesAsync();
                return RedirectToAction("Menu", "Organizations", new { id = organizationId });
            }

            var organization = _context.Organizations
                                          .Include(o => o.OrganizationSubjects)
                                              .ThenInclude(o => o.Subject)
                                          .FirstOrDefault(o => o.Id == organizationId);
            if (organization != null)
            {
                ViewData["SubjectName"] = new SelectList(organization.OrganizationSubjects.Select(o => o.Subject), "Id", "Name");
            }

            return View(teacher);
        }

        //Редактирование преподавателя
        // GET: Teachers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            int organizationId = Convert.ToInt32(TempData["OrganizationId"]);
            var organization = _context.Organizations
                                          .Include(o => o.OrganizationSubjects)
                                              .ThenInclude(o => o.Subject)
                                          .FirstOrDefault(o => o.Id == organizationId);
            if (organization != null)
            {
                ViewData["SubjectName"] = new SelectList(organization.OrganizationSubjects.Select(o => o.Subject), "Id", "Name");
            }

            if (id == null)
            {
                return NotFound();
            }

            var teacher = await _context.Teachers.FindAsync(id);

            if (teacher == null)
            {
                return NotFound();
            }

            return View(teacher);
        }
        // POST: Teachers/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Login,Password,Name,Surname,SubjectId, OrganizationId")] Teacher teacher)
        {
            var existingTeacher = await _context.Teachers.FirstOrDefaultAsync(o => o.Login == teacher.Login && o.Id != id);
            if (existingTeacher != null)
            {
                ModelState.AddModelError("Login", "Логин уже используется.");
                return View(teacher);
            }

            else if (id != teacher.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(teacher);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TeacherExists(teacher.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Menu", "Organizations", new { id = teacher.OrganizationId });
            }
            var organization = _context.Organizations
                                          .Include(o => o.OrganizationSubjects)
                                              .ThenInclude(o => o.Subject)
                                          .FirstOrDefault(o => o.Id == teacher.OrganizationId);
            if (organization != null)
            {
                ViewData["SubjectName"] = new SelectList(organization.OrganizationSubjects.Select(o => o.Subject), "Id", "Name");
            }
            return View(teacher);
        }

        // GET: Teachers/EditTeacher/5
        public async Task<IActionResult> EditTeacher(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var teacher = await _context.Teachers.FindAsync(id);

            if (teacher == null)
            {
                return NotFound();
            }

            return View(teacher);
        }

        // POST: Teachers/EditTeacher/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditTeacher(int id, [Bind("Id,Login,Password,Name,Surname,SubjectId, OrganizationId")] Teacher teacher)
        {
            var existingTeacher = await _context.Teachers.FirstOrDefaultAsync(o => o.Login == teacher.Login && o.Id != id);
            if (existingTeacher != null)
            {
                ModelState.AddModelError("Login", "Логин уже используется.");
                return View(teacher);
            }

            else if (id != teacher.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(teacher);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TeacherExists(teacher.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Menu", "Teachers", new { id = teacher.Id });
            }
            return View(teacher);
        }

        //Удаление преподавателя
        // GET: Teachers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var teacher = await _context.Teachers
                .Include(t => t.Organization)
                .Include(t => t.Subject)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (teacher == null)
            {
                return NotFound();
            }

            return View(teacher);
        }
        // POST: Teachers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            int organizationId = Convert.ToInt32(TempData["OrganizationId"]);

            var teacher = await _context.Teachers.FindAsync(id);
            if (teacher != null)
            {
                _context.Teachers.Remove(teacher);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Menu", "Organizations", new { id = organizationId });
        }

        private bool TeacherExists(int id)
        {
            return _context.Teachers.Any(e => e.Id == id);
        }
    }
}
