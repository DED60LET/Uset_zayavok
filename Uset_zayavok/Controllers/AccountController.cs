using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;


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

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }
    }
}