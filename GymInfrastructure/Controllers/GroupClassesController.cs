using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GymDomain.Model;
using GymInfrastructure;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Authorization;

namespace GymInfrastructure.Controllers
{
    public class GroupClassesController : Controller
    {
        private readonly DbgymContext _context;

        public GroupClassesController(DbgymContext context)
        {
            _context = context;
        }

        // GET: GroupClasses
        public async Task<IActionResult> Index()
        {
            if (User.Identity.IsAuthenticated && !User.IsInRole("trainer") && !User.IsInRole("admin"))
            {
                string currentUserEmail = User.Identity.Name;
                var member = await _context.Members.FirstOrDefaultAsync(m => m.Email == currentUserEmail);
                var dbgymContext = _context.GroupClasses
                                            .Include(g => g.Gym)
                                            .Include(m => m.Members)
                                            .Where(c => c.GymId == member.GymId);
                return View(await dbgymContext.ToListAsync());
            }
            if (User.IsInRole("trainer"))
            {
                string currentUserEmail = User.Identity.Name;
                var trainer = await _context.Trainers.FirstOrDefaultAsync(m => m.Email == currentUserEmail);
                var dbgymContext = _context.GroupClasses
                                           .Include(g => g.Gym)
                                            .Include(m => m.Members)
                                           .Where(c => c.GymId == trainer.GymId);
                return View(await dbgymContext.ToListAsync());
            }
            var allClasses = await _context.GroupClasses.Include(g => g.Gym).Include(m => m.Members).ToListAsync();
            return View(allClasses);
        }


        public async Task<IActionResult> TrainerClasses()
        {
            string currentUserEmail = User.Identity.Name;

            // Find the trainer with the same email as the current user
            var trainer = await _context.Trainers.FirstOrDefaultAsync(t => t.Email == currentUserEmail);
            if (trainer == null)
            {
                return NotFound();
            }

            var dbgymContext = trainer.GroupClasses;
            return View("Index", dbgymContext.ToList());
        }


        // GET: GroupClasses/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var groupClass = await _context.GroupClasses
                .Include(g => g.Gym)
                .Include(g => g.Trainers)
                .Include(g => g.Members)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (groupClass == null)
            {
                return NotFound();
            }

            return View(groupClass);
        }
        // GET: GroupClasses/Rollin/5
    
        public async Task<IActionResult> Rollin(int id)
        {
            if (User.Identity.Name == null)
            {
                return RedirectToAction("Login", "Account");
            }
            // Get the email of the current user
            string currentUserEmail = User.Identity.Name;

            // Find the member with the same email as the current user
            var member = await _context.Members.FirstOrDefaultAsync(m => m.Email == currentUserEmail);
            if (member == null)
            {
                return NotFound();
            }

            // Find the group class with the given id
            var groupClass = await _context.GroupClasses.FindAsync(id);
            if (groupClass == null)
            {
                return NotFound();
            }

            // Add the member to the group class
            groupClass.Members.Add(member);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "GroupClasses");
        }
    
        public async Task<IActionResult> UnRollin(int id)
        {
            string currentUserEmail = User.Identity.Name;

            // Знайти учасника з такою самою електронною поштою, як у поточного користувача
            var member = await _context.Members.Include(m => m.GroupClasses).FirstOrDefaultAsync(m => m.Email == currentUserEmail);
            if (member == null)
            {
                return NotFound();
            }

            // Знайти груповий клас за заданим ідентифікатором
            var groupClass = await _context.GroupClasses.FindAsync(id);
            if (groupClass == null)
            {
                return NotFound();
            }

            // Видалити учасника зі списку учасників групи
            member.GroupClasses.Remove(groupClass);
            _context.Update(member);
            // Зберегти зміни в базі даних
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "GroupClasses");


        }
        // GET: GroupClasses/Create
        [Authorize(Roles = "admin")]
        public IActionResult Create()
        {
            ViewData["GymId"] = new SelectList(_context.Gyms, "Id", "Adress");
            return View();
        }

        // POST: GroupClasses/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Create([Bind("Room,Schedule,Name,Price,GymId,Id")] GroupClass groupClass)
        {
            ModelState.Remove("Gym");
            if (ModelState.IsValid)
            {
                _context.Add(groupClass);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["GymId"] = new SelectList(_context.Gyms, "Id", "Adress", groupClass.GymId);
            return View(groupClass);
        }

        // GET: GroupClasses/Edit/5
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var groupClass = await _context.GroupClasses.FindAsync(id);
            if (groupClass == null)
            {
                return NotFound();
            }
            ViewData["GymId"] = new SelectList(_context.Gyms, "Id", "Adress", groupClass.GymId);
            return View(groupClass);
        }

        // POST: GroupClasses/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Edit(int id, [Bind("Room,Schedule,Name,Price,GymId,Id")] GroupClass groupClass)
        {
            if (id != groupClass.Id)
            {
                return NotFound();
            }
            ModelState.Remove("Gym");
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(groupClass);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GroupClassExists(groupClass.Id))
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
            ViewData["GymId"] = new SelectList(_context.Gyms, "Id", "Adress", groupClass.GymId);
            return View(groupClass);
        }

        // GET: GroupClasses/Delete/5
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var groupClass = await _context.GroupClasses
                .Include(g => g.Gym)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (groupClass == null)
            {
                return NotFound();
            }

            return View(groupClass);
        }

        // POST: GroupClasses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var groupClass = await _context.GroupClasses.FindAsync(id);
            if (groupClass != null)
            {
                _context.GroupClasses.Remove(groupClass);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool GroupClassExists(int id)
        {
            return _context.GroupClasses.Any(e => e.Id == id);
        }
    }
}
