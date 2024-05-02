using GymInfrastructure.Models;
using GymInfrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Threading;
using GymDomain.Model;
using DocumentFormat.OpenXml.InkML;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
namespace GymInfrastructure.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<User> _userManager;
        private readonly DbgymContext _context;

        public HomeController(ILogger<HomeController> logger, UserManager<User> userManager, DbgymContext context)
        {
            _logger = logger;
            _userManager = userManager;
            _context = context;

        }



        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Controle()
        {

            var currentUser = await _userManager.GetUserAsync(User);

            // ��������� �� ���������� ������������� �� �� ���� User
            if (User.Identity.IsAuthenticated && User.IsInRole("trainer"))
            {
                // �������� ������ �������
                var trainers = _context.Trainers.ToList();

                // �������� ������ ������� �� �������������
                return RedirectToAction("TrainerPage");
            }
            if (User.Identity.IsAuthenticated && User.IsInRole("user"))
            {
                // �������� ������ �������� ��� ��������� �����������
                var members = _context.Members.Where(m => m.Email == currentUser.Email).ToList();

                // �������� ������ �������� �� �������������
                return View(members);
            }
            // ��������� �� ���������� ������������� �� �� ���� Trainer

            else
            {
                // ���� ���������� �� ������������� ��� �� �� ����� � �������� �����, �������, ������� ���������� ����� �������
                return View(); // ����������, �� � ��� � ������� AccessDenied.cshtml ��� ����������� ������� �������
            }

        }
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

                }
                return RedirectToAction("TrainerPage");
            }
            ViewData["GymId"] = new SelectList(_context.Gyms, "Id", "Adress", trainer.GymId);
            return View(trainer);
        }
        [Authorize(Roles = "admin,trainer")]
        public async Task<IActionResult> TrainerPage()
        {

            var currentUser = await _userManager.GetUserAsync(User);

            // ��������� �� ���������� ������������� �� �� ���� User
            if (User.Identity.IsAuthenticated && User.IsInRole("trainer"))
            {
                // �������� ������ �������
                var trainer = _context.Trainers
                    .Include(t => t.Gym).Include(t => t.GroupClasses).FirstOrDefault(m => m.Email == currentUser.Email);

                // �������� ������ ������� �� �������������
                return View("TrainerPage", trainer);
            }
            else
            {
                // ���� ���������� �� ������������� ��� �� �� ����� � �������� �����, �������, ������� ���������� ����� �������
                return NotFound(); // ����������, �� � ��� � ������� AccessDenied.cshtml ��� ����������� ������� �������
            }
        }
        public IActionResult Privacy()
        {
            return View();
        }
        public IActionResult Statistics()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
