using GymDomain.Model;
using GymInfrastructure.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace GymInfrastructure.Controllers
{
    public class RolesController : Controller
    {
        RoleManager<IdentityRole> _roleManager;
        UserManager<User> _userManager;
        private readonly DbgymContext _context;
        public RolesController(RoleManager<IdentityRole> roleManager, UserManager<User> userManager, DbgymContext context)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _context = context;
        }
        public IActionResult Index() => View(_roleManager.Roles.ToList());
        [Authorize(Roles = "admin")]
        public IActionResult UserList() => View(_userManager.Users.ToList());
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Edit(string userId)
        {
            // отримуємо користувача
            User user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                //список ролей користувача
                var userRoles = await _userManager.GetRolesAsync(user);
                var allRoles = _roleManager.Roles.ToList();
                ChangeRoleViewModel model = new ChangeRoleViewModel
                {
                    UserId = user.Id,
                    UserEmail = user.Email,
                    UserRoles = userRoles,
                    AllRoles = allRoles
                };
                return View(model);
            }

            return NotFound();
        }
        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Edit(string userId, List<string> roles)
        {
            // отримуємо користувача
            User user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                // список ролей користувача
                var userRoles = await _userManager.GetRolesAsync(user);
                // получаем все роли
                var allRoles = _roleManager.Roles.ToList();
                // список ролей, які було додано
                var addedRoles = roles.Except(userRoles);
                // список ролей, які було видалено
                var removedRoles = userRoles.Except(roles);

                await _userManager.AddToRolesAsync(user, addedRoles);

                await _userManager.RemoveFromRolesAsync(user, removedRoles);

                if (await _userManager.IsInRoleAsync(user, "trainer"))
                {
                    var member = _context.Members.Where(m => m.Email == user.Email).FirstOrDefault();
                    Trainer trainer = new Trainer { GymId = member.GymId, TrainerName = member.Name, Phone = member.Phone, Email = member.Email };
                    _context.Trainers.Add(trainer);
                    _context.Members.Remove(member);
                    await _context.SaveChangesAsync();
                    _context.Update(trainer);
                    _context.Update(member);
                }
                if (await _userManager.IsInRoleAsync(user, "user"))
                {
                    var trainer = _context.Trainers.Where(m => m.Email == user.Email).FirstOrDefault();
                    if (trainer != null) {
                        Member member = new Member { GymId = trainer.GymId, Name = trainer.TrainerName, Phone = trainer.Phone, Email = trainer.Email, SubscriptionId = -1 };
                        _context.Members.Add(member);
                        _context.Trainers.Remove(trainer);
                        await _context.SaveChangesAsync();
                    }
                }



                return RedirectToAction("UserList");
            }

            return NotFound();
        }

    }
}
