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
    public class MembersController : Controller
    {
        private readonly DbgymContext _context;

        public MembersController(DbgymContext context)
        {
            _context = context;
        }

        // GET: Members
        public async Task<IActionResult> Index()
        {
            var currentUser = User.Identity.Name;

            IQueryable<Member> dbgymContext;
            if (User.Identity.IsAuthenticated)
            {
                dbgymContext = _context.Members
                    .Include(m => m.GroupClasses)
                    .Include(m => m.Subscription)
                    .Where(m => m.Email == currentUser)
                    .DefaultIfEmpty();
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
            if (User.IsInRole("admin"))
            {
                dbgymContext = _context.Members
                    .Include(m => m.GroupClasses)
                    .Include(m => m.Subscription)
                    .DefaultIfEmpty();
            }


            return View(await dbgymContext.ToListAsync());
        }
        public IActionResult GetSubscriptionsByGym(int gymId)
        {
            var subscription = _context.Subscriptions
                                       .Where(gc => gc.GymId == gymId)
                                       .Select(gc => new { id = gc.Id, name = gc.Type })
                                       .ToList();
            return Json(subscription);
        }

        // GET: Members/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var member = await _context.Members
                .Include(m => m.Subscription)
                .Include(m => m.GroupClasses)
                .Include(m => m.Gym)
                .DefaultIfEmpty()
                .FirstOrDefaultAsync(m => m.Id == id);
            if (member == null)
            {
                return NotFound();
            }

            return View(member);
        }

        public IActionResult GetGroupClassesByGym(int gymId)
        {
            var groupClasses = _context.GroupClasses
                                       .Where(gc => gc.GymId == gymId)
                                       .Select(gc => new { id = gc.Id, name = gc.Name })
                                       .ToList();
            return Json(groupClasses);
        }
        // GET: Members/Create
        [Authorize(Roles = "admin")]
        public IActionResult Create()
        {
            ViewData["SubscriptionId"] = new SelectList(_context.Subscriptions, "Id", "Type");
            ViewData["ClassItems"] = new SelectList(_context.GroupClasses, "Id", "Name");
            ViewData["GymId"] = new SelectList(_context.Gyms, "Id", "Adress");

            return View();
        }

        // POST: Members/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Create([Bind("SubscriptionId,GymId,Name,Phone,Email,Id")] Member member, int[] selectedClassIds)
        {
            ModelState.Remove("Subscription");
            ModelState.Remove("Gym");
            if (ModelState.IsValid)
            {
                foreach (var classId in selectedClassIds)
                {
                    var groupClass = _context.GroupClasses.Find(classId);
                    if (groupClass != null)
                    {
                        member.GroupClasses.Add(groupClass);
                    }
                }
                _context.Add(member);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["SubscriptionId"] = new SelectList(_context.Subscriptions, "Id", "Type", member.SubscriptionId);
            return View(member);
        }

        // GET: Members/Edit/5
        [Authorize(Roles = "admin,user")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var member = await _context.Members
                                      .Include(m => m.GroupClasses)
                                      .FirstOrDefaultAsync(m => m.Id == id);

            if (member == null)
            {
                return NotFound();
            }


            var memberClassIds = member.GroupClasses.Select(gc => gc.Id).ToList();


            var availableClasses = _context.GroupClasses
                                           .Where(gc => !memberClassIds.Contains(gc.Id))
                                           .ToList();
            ViewData["MemberClassItems"] = new SelectList(memberClassIds, "Id", "Name");
            ViewData["ClassItems"] = new SelectList(availableClasses, "Id", "Name");
            ViewData["GymId"] = new SelectList(_context.Gyms, "Id", "Adress", member.GymId);
            ViewData["SubscriptionId"] = new SelectList(_context.Subscriptions, "Id", "Type", member.SubscriptionId);

            return View(member);
        }


        // POST: Members/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin,user")]
        public async Task<IActionResult> Edit(int id, [Bind("SubscriptionId,GymId,Name,Phone,Email,Id")] Member member, int[] classesToRemove, int[] selectedClassIds)
        {
            if (id != member.Id)
            {
                return NotFound();
            }

            ModelState.Remove("Gym");
            ModelState.Remove("Subscription");
            ModelState.Remove("Email");
            ModelState.Remove("selectedClassIds");
            if (ModelState.IsValid)
            {
                var existingMember = await _context.Members
                                                   .Include(m => m.GroupClasses)
                                                   .FirstOrDefaultAsync(m => m.Id == member.Id);

                if (existingMember == null)
                {
                    return NotFound();
                }

                // Remove unselected classes
                foreach (var classId in classesToRemove)
                {
                    var groupClass = existingMember.GroupClasses.FirstOrDefault(gc => gc.Id == classId);
                    if (groupClass != null)
                    {
                        existingMember.GroupClasses.Remove(groupClass);
                    }
                }

                // Add new selected classes
                foreach (var classId in selectedClassIds)
                {
                    if (!existingMember.GroupClasses.Any(gc => gc.Id == classId))
                    {
                        var groupClassToAdd = await _context.GroupClasses.FindAsync(classId);
                        if (groupClassToAdd != null)
                        {
                            existingMember.GroupClasses.Add(groupClassToAdd);
                        }
                    }
                }

                // Update other member properties
                existingMember.Name = member.Name;
                existingMember.Phone = member.Phone;

                existingMember.SubscriptionId = member.SubscriptionId;
                existingMember.GymId = member.GymId;

                try
                {
                    _context.Update(existingMember);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MemberExists(member.Id))
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

            ViewData["SubscriptionId"] = new SelectList(_context.Subscriptions, "Id", "Type", member.SubscriptionId);
            return View(member);
        }


        // GET: Members/Delete/5
        [Authorize(Roles = "admin,user")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var member = await _context.Members
                .Include(m => m.Subscription)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (member == null)
            {
                return NotFound();
            }

            return View(member);
        }

        // POST: Members/Delete/5
        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "admin,user")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var member = await _context.Members.FindAsync(id);
            if (member != null)
            {
                _context.Members.Remove(member);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MemberExists(int id)
        {
            return _context.Members.Any(e => e.Id == id);
        }
    }
}
