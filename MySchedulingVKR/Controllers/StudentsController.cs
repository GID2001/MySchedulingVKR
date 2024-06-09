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
    public class StudentsController : Controller
    {
        private readonly DbSchedulingContext _context;

        public StudentsController(DbSchedulingContext context)
        {
            _context = context;
        }

        //Регистрация студента
        //GET: Students/Registration
        public IActionResult Registration()
        {
            return View();
        }
        //POST: Students/Registration
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Registration([Bind("Id,Name,Surname,Login,Password")] Student student)
        {
            var existingStudent = await _context.Students.FirstOrDefaultAsync(o => o.Login == student.Login);

            if (existingStudent != null)
            {
                ModelState.AddModelError("Login", "Логин уже используется.");
                return View(student);
            }

            if (ModelState.IsValid)
            {
                _context.Add(student);
                await _context.SaveChangesAsync();
                return RedirectToAction("Menu", "Students", new { id = student.Id });
            }
            return View(student);
        }
        //GET: Students/Registration

        //Авторизация студента
        //GET: Students/Authorization
        public IActionResult Authorization()
        {
            return View();
        }
        //POST: Students/Authorization
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Authorization([Bind("Login,Password")] Student student)
        {

            var user = await _context.Students.FirstOrDefaultAsync(o => o.Login == student.Login && o.Password == student.Password);

            // Проверка введенных данных пользователя
            if (user != null)
            {
                // Если аутентификация успешна, выполните необходимые действия (например, установка пользовательской сессии и редирект)
                return RedirectToAction("Menu", "Students", new { id = user.Id });
            }
            else
            {
                // Если аутентификация неуспешна, можно вернуть представление с сообщением об ошибке
                ViewBag.ErrorMessage = "Неверный логин или пароль";
                return View(user);
            }
        }

        //Меню студента
        //GET Students/5
        public async Task<IActionResult> Menu(int id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var viewModel = new StudentSubjectView();

            viewModel.Subjects = await _context.Subjects
                .Include(s => s.OrganizationSubjects)
                    .ThenInclude(s => s.Organization)
                    .ThenInclude(o => o.Teachers)
                        .ThenInclude(t => t.TeacherAccesses)
                .AsNoTracking()
                .ToListAsync();

            viewModel.Student = await _context.Students.FirstOrDefaultAsync(s => s.Id == id);

            return View(viewModel);
        }

        public async Task<IActionResult> RegistrationLesson(int subjectId, int organizationId, int teacherId, int studentId)
        {
            if (studentId == null || subjectId == null || teacherId == null || organizationId == null)
            {
                return NotFound();
            }
            else
            {
                var viewModel = new StudentTeacherView();

                viewModel.Teachers = await _context.Teachers
                    .Include(t => t.Organization)
                    .Include(t => t.Subject)
                    .Include(t => t.TeacherAccesses)
                        .ThenInclude(ta => ta.Lesson)
                    .Where(t => t.SubjectId == subjectId && t.OrganizationId == organizationId && t.TeacherAccesses.Any(t => t.Access == true))
                    .AsNoTracking()
                    .ToListAsync();

                viewModel.Student = await _context.Students.FirstOrDefaultAsync(s => s.Id == studentId);

                viewModel.Teacher = viewModel.Teachers.FirstOrDefault(s => s.Id == teacherId);

                viewModel.Lessons = viewModel.Teacher.TeacherAccesses.Where(s => s.Access == true).Select(s => s.Lesson);

                return View(viewModel);
            }
        }

        //POST: Students/RegistrationLesson
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegistrationLesson(int teacherId, int studentId, int lessonId)
        {
            if(studentId == null || teacherId == null || lessonId == null)
            {
                return NotFound();
            }
            else
            {
                var teacherAccess = await _context.TeacherAccesses.FirstOrDefaultAsync(t => t.TeacherId == teacherId && t.LessonId == lessonId);
                
                if(teacherAccess != null && teacherAccess.Access == true)
                {
                    teacherAccess.Access = false;
                    _context.Update(teacherAccess);
                    await _context.SaveChangesAsync();

                    var schedule = new Schedule();
                    schedule.LessonId = lessonId;
                    
                    //_context.Schedules.Add(schedule);
                    //await _context.SaveChangesAsync();

                    var scheduleTeacher = new ScheduleTeacher();
                    scheduleTeacher.ScheduleId = schedule.LessonId;
                    scheduleTeacher.TeacherId = teacherId;
                    //_context.ScheduleTeachers.Add(scheduleTeacher);
                    //await _context.SaveChangesAsync();

                    var scheduleStudent = new ScheduleStudent();
                    scheduleStudent.StudentId = studentId;
                    scheduleStudent.ScheduleId = schedule.LessonId;
                    //_context.ScheduleStudents.Add(scheduleStudent);
                    //await _context.SaveChangesAsync();



                    schedule.ScheduleTeachers.Add(scheduleTeacher);
                    schedule.ScheduleStudents.Add(scheduleStudent);

                    _context.Schedules.Add(schedule);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    return NotFound();
                }



            }



                return RedirectToAction("Menu", "Students", new { id = studentId });

        }

        [HttpGet]
        public async Task<IActionResult> ScheduleStudent(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var schedule = await _context.Students
                .Include(s => s.ScheduleStudents)
                    .ThenInclude(s => s.Schedule)
                    .ThenInclude(t => t.Lesson)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(i => i.Id == id);

            return View(schedule);
        }


        // GET: Students
        public async Task<IActionResult> Index()
        {
            return View(await _context.Students.ToListAsync());
        }

        //Информация о студенте
        // GET: Students/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.Students
                .FirstOrDefaultAsync(m => m.Id == id);
            if (student == null)
            {
                return NotFound();
            }

            return View(student);
        }

        //Создание студента
        // GET: Students/Create
        public IActionResult Create()
        {
            return View();
        }
        // POST: Students/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Login,Password,Name,Surname")] Student student)
        {
            if (ModelState.IsValid)
            {
                _context.Add(student);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(student);
        }

        //Редактирование студента
        // GET: Students/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.Students.FindAsync(id);
            if (student == null)
            {
                return NotFound();
            }
            return View(student);
        }
        // POST: Students/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Login,Password,Name,Surname")] Student student)
        {
            if (id != student.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(student);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StudentExists(student.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(student);
        }

        //Удаление студента
        // GET: Students/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.Students
                .FirstOrDefaultAsync(m => m.Id == id);
            if (student == null)
            {
                return NotFound();
            }

            return View(student);
        }
        // POST: Students/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var student = await _context.Students.FindAsync(id);
            if (student != null)
            {
                _context.Students.Remove(student);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool StudentExists(int id)
        {
            return _context.Students.Any(e => e.Id == id);
        }
    }
}
