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
    public class OrganizationsController : Controller
    {
        private readonly DbSchedulingContext _context;

        public OrganizationsController(DbSchedulingContext context)
        {
            _context = context;
        }

        //Регистрация организации
        // GET: Organizations/Registration
        public IActionResult Registration()
        {
            return View();
        }
        // POST: Organizations/Registration
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Registration([Bind("Id,Name,Login,Password, Specialization")] Organization organization)
        {
            var existingOrganization = await _context.Organizations.FirstOrDefaultAsync(o => o.Login == organization.Login);

            if (existingOrganization != null)
            {
                ModelState.AddModelError("Login", "Логин уже используется.");
                return View(organization);
            }
            else if (ModelState.IsValid)
            {
                _context.Add(organization);
                await _context.SaveChangesAsync();
                return RedirectToAction("Menu", "Organizations", new { id = organization.Id });
            }

            return View(organization);
        }
        
        //Авторизация организации
        // GET: Organizations/Authorization
        public IActionResult Authorization()
        {
            return View();
        }
        // POST: Organizations/Authorization
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Authorization([Bind("Login,Password")] Organization organization)
        {

            var user = await _context.Organizations.FirstOrDefaultAsync(o => o.Login == organization.Login && o.Password == organization.Password);

            // Проверка введенных данных пользователя
            if (user != null)
            {
                return RedirectToAction("Menu", "Organizations", new { id = user.Id });
            }
            else
            {
                ViewBag.ErrorMessage = "Неверный логин или пароль";
                return View(user);
            }
        }

        //Меню организации
        // GET: Organizations/Menu/5
        [HttpGet]
        public async Task<IActionResult> Menu(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var viewModel = new OrganizationViewSubjectAndTeacher();
            viewModel.Organizations = await _context.Organizations
                .Include(o => o.OrganizationSubjects)
                    .ThenInclude(s => s.Subject)
                .Include(o => o.Teachers)
                    .ThenInclude(t =>  t.Subject)
                .AsNoTracking()
                .ToListAsync();

            if (id != null)
            {
                ViewData["Id"] = id;

                TempData["OrganizationId"] = id;

                Organization organization = viewModel.Organizations.Where(
                    o => o.Id == id).Single();

                viewModel.Organization = organization;

                viewModel.Subjects = organization.OrganizationSubjects.Select(s => s.Subject);

                viewModel.Teachers = organization.Teachers;
            }

            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> ScheduleTeachers(int? id)
        {

            if(id == null)
            {
                return NotFound();
            }

            var schedule = await _context.Organizations
                  .Include(o => o.Teachers)
                      .ThenInclude(t => t.ScheduleTeachers)
                      .ThenInclude(s => s.Schedule)
                      .ThenInclude(s => s.Lesson)
                   .FirstOrDefaultAsync(o => o.Id == id);

            return View(schedule);
        }

        //Редактирование
        // GET: Organizations/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var organization = await _context.Organizations.FindAsync(id);
            if (organization == null)
            {
                return NotFound();
            }
            return View(organization);
        }
        // POST: Organizations/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Login,Password, Specialization")] Organization organization)
        {
            if (id != organization.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(organization);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrganizationExists(organization.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Menu", "Organizations", new { id = organization.Id });
            }
            return View(organization);
        }

        //Удаление
        // GET: Organizations/Delete/5
        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var organization = await _context.Organizations
                .FirstOrDefaultAsync(m => m.Id == id);
            if (organization == null)
            {
                return NotFound();
            }

            return View(organization);
        }
        // POST: Organizations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var organization = await _context.Organizations.FindAsync(id);
            if (organization != null)
            {
                _context.Organizations.Remove(organization);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Home", "Index");
        }

        private bool OrganizationExists(int id)
        {
            return _context.Organizations.Any(e => e.Id == id);
        }
    }
}
