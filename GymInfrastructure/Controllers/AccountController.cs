using DocumentFormat.OpenXml.InkML;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Wordprocessing;
using GymDomain.Model;
using GymInfrastructure.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace GymInfrastructure.Controllers
{
    public class AccountController : Controller
    {

        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly DbgymContext _context;
        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager, DbgymContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }
        [HttpGet]
        public IActionResult Register()
        {
            ViewData["GymId"] = new SelectList(_context.Gyms, "Id", "Adress");
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> RemoveSubscription()
        {
            User currentUser = await _userManager.GetUserAsync(User);
            var subscriptions = _context.Subscriptions.ToList();
            var member = _context.Members.Where(m => m.Email == currentUser.Email).FirstOrDefault();
            member.SubscriptionId = -1;
            _context.SaveChanges();


            return RedirectToAction("Details", "Members", new { id = member.Id });

        }
      
        [HttpGet]
        public async Task<IActionResult> SetSubscription()
        {
            ViewData["SubscriptionId"] = new SelectList(_context.Subscriptions, "Id", "Type");
            ViewData["ClassItems"] = new SelectList(_context.GroupClasses, "Id", "Name");
            ViewData["GymId"] = new SelectList(_context.Gyms, "Id", "Adress");
            User currentUser = await _userManager.GetUserAsync(User);
            var subscriptions = _context.Subscriptions.ToList();
            var member = _context.Members.Where(m => m.Email == currentUser.Email).FirstOrDefault();
            if (member != null)
            {
                ViewBag.MemberId = member.Id;

                return View(subscriptions);
            }
            member = new Member { Name = currentUser.Name, Email = currentUser.Email, Phone = currentUser.PhoneNumber, GymId = currentUser.GymId, SubscriptionId = -1 };
            _context.Members.Add(member);
            await _context.SaveChangesAsync();
            ViewBag.MemberId = member.Id;

            return View(subscriptions);
        }
        [HttpPost]
        public IActionResult SetSubscription(int subscriptionId, int memberId)
        {
            Member member = _context.Members.Where(m => m.Id == memberId).FirstOrDefault();
            member.SubscriptionId = subscriptionId;
            _context.SaveChanges();


            return RedirectToAction("Controle", "Home");
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            ModelState.Remove("Phone");
            if (ModelState.IsValid)
            {
                User user = new User { Email = model.Email, UserName = model.Email, GymId = model.GymId, Name = model.Name, PhoneNumber = model.Phone };
                // додаємо користувача
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    // установка кукі
                    await _signInManager.SignInAsync(user, false);
                    return RedirectToAction("SetSubscription", "Account");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }
            ViewData["GymId"] = new SelectList(_context.Gyms, "Id", "Adress");
            return View(model);
        }
        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            returnUrl = "/";
            return View(new LoginViewModel { ReturnUrl = returnUrl });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result =
                    await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);
                if (result.Succeeded)
                {
                    // перевіряємо, чи належить URL додатку
                    if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                    {
                        return Redirect(model.ReturnUrl);
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Неправильний логін чи (та) пароль");
                }
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            // видаляємо автентифікаційні куки
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

    }

}
