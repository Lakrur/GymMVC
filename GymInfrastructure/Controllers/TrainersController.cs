using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GymDomain.Model;
using GymInfrastructure;
using Microsoft.AspNetCore.Authorization;

namespace GymInfrastructure.Controllers
{
    public class TrainersController : Controller
    {
        private readonly DbgymContext _context;

        public TrainersController(DbgymContext context)
        {
            _context = context;
        }

        // GET: Trainers
        public async Task<IActionResult> Index()
        {
            var dbgymContext = _context.Trainers.Include(t => t.Gym);
            return View(await dbgymContext.ToListAsync());
        }

        // GET: Trainers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var trainer = await _context.Trainers
                .Include(t => t.Gym)
                .Include(t => t.GroupClasses)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (trainer == null)
            {
                return NotFound();
            }

            return View(trainer);
        }

        // GET: Trainers/Create
        [Authorize(Roles = "admin")]
        public IActionResult Create()
        {
            ViewData["GymId"] = new SelectList(_context.Gyms, "Id", "Adress");
            return View();
        }

        // POST: Trainers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Create([Bind("GymId,ClassId,Specialization,TrainerName,Phone,Email,Id")] Trainer trainer, int[] selectedClassIds)
        {

            ModelState.Remove("Gym");
            ModelState.Remove("selectedClassIds");
            if (ModelState.IsValid)
            {

                if (selectedClassIds != null)
                {
                    foreach (var classId in selectedClassIds)
                    {
                        var groupClass = await _context.GroupClasses.FindAsync(classId);
                        if (groupClass != null)
                        {
                            trainer.GroupClasses.Add(groupClass);
                        }
                    }
                }
                _context.Add(trainer);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["GymId"] = new SelectList(_context.Gyms, "Id", "Adress", trainer.GymId);
            return View(trainer);
        }

        // GET: Trainers/Edit/5
        [Authorize(Roles = "admin,trainer")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var trainer = await _context.Trainers.Include(m => m.GroupClasses).FirstOrDefaultAsync(m => m.Id == id);
            if (trainer == null)
            {
                return NotFound();
            }
            ViewData["GymId"] = new SelectList(_context.Gyms, "Id", "Adress", trainer.GymId);
            return View(trainer);
        }

        // POST: Trainers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin,trainer")]
        public async Task<IActionResult> Edit(int id, [Bind("GymId,ClassId,Specialization,TrainerName,Phone,Email,Id")] Trainer trainer, int[] classesToRemove, int[] selectedClassIds)
        {
            if (id != trainer.Id)
            {
                return NotFound();
            }
            ModelState.Remove("Gym");
            ModelState.Remove("selectedClassIds");
            if (ModelState.IsValid)
            {
                try
                {
                    // Update basic details of the trainer
                    var existingTrainer = await _context.Trainers
                        .Include(t => t.GroupClasses)
                        .FirstOrDefaultAsync(t => t.Id == id);

                    if (existingTrainer == null)
                    {
                        return NotFound();
                    }

                    existingTrainer.TrainerName = trainer.TrainerName;
                    existingTrainer.Phone = trainer.Phone;
                    existingTrainer.Email = trainer.Email;
                    existingTrainer.GymId = trainer.GymId;

                    // Handle group classes removal
                    foreach (var classId in classesToRemove)
                    {
                        var groupClass = existingTrainer.GroupClasses.FirstOrDefault(gc => gc.Id == classId);
                        if (groupClass != null)
                        {
                            existingTrainer.GroupClasses.Remove(groupClass);
                        }
                    }

                    // Handle group classes addition
                    foreach (var classId in selectedClassIds)
                    {
                        if (!existingTrainer.GroupClasses.Any(gc => gc.Id == classId))
                        {
                            var groupClassToAdd = await _context.GroupClasses.FindAsync(classId);
                            if (groupClassToAdd != null)
                            {
                                existingTrainer.GroupClasses.Add(groupClassToAdd);
                            }
                        }
                    }

                    _context.Update(existingTrainer);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TrainerExists(trainer.Id))
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
            ViewData["GymId"] = new SelectList(_context.Gyms, "Id", "Adress", trainer.GymId);
            return View(trainer);
        }


        // GET: Trainers/Delete/5
        [Authorize(Roles = "admin,trainer")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var trainer = await _context.Trainers
                .Include(t => t.Gym)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (trainer == null)
            {
                return NotFound();
            }

            return View(trainer);
        }

        // POST: Trainers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin,trainer")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var trainer = await _context.Trainers.FindAsync(id);
            if (trainer != null)
            {
                _context.Trainers.Remove(trainer);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TrainerExists(int id)
        {
            return _context.Trainers.Any(e => e.Id == id);
        }
    }
}
