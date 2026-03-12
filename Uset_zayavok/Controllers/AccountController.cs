using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using Uset_zayavok.Models; // Не забудь про этот using для доступа к моделям

namespace Uset_zayavok.Controllers
{
    public class AccountController : Controller
    {
        private readonly PostgresContext _context;

        public AccountController(PostgresContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login(string login, string password)
        {
            var user = _context.Users.FirstOrDefault(u => u.Login == login && u.Password == password);

            if (user != null)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.Fio),
                    new Claim(ClaimTypes.Role, user.Type),
                    new Claim("UserId", user.Userid.ToString())
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity));

                return RedirectToAction("Index", "List");
            }

            ViewBag.Error = "Неверный логин или пароль";
            return View();
        }

        
        [HttpGet]
        public IActionResult Register() => View();

      
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                
                if (_context.Users.Any(u => u.Login == model.Login))
                {
                    ModelState.AddModelError("Login", "Этот логин уже занят");
                    return View(model);
                }

               
                var newUser = new User
                {
                    Fio = model.Fio,
                    Phone = model.Phone,
                    Login = model.Login,
                    Password = model.Password, 
                    Type = "Заказчик"
                };

                try
                {
                    _context.Users.Add(newUser);
                    _context.SaveChanges();
                    return RedirectToAction("Login");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Ошибка при регистрации: " + ex.Message);
                }
            }
            return View(model);
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }
    }
}