using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using Uset_zayavok.Models;


namespace Uset_zayavok.Controllers
{
    public class ListController : Controller
    {
        private readonly PostgresContext _context;

        public ListController(PostgresContext context)
        {
            _context = context;
        }

        public IActionResult Index(string search)
        {
            var requests = _context.Requests.Include(r => r.Client).AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                requests = requests.Where(r => r.Hometechmodel.Contains(search) ||
                                             r.Requestid.ToString() == search);
            }

            return View(requests.ToList());
        }

        public IActionResult Create()
        {
            ViewBag.Clients = _context.Users.Where(u => u.Type == "Заказчик").ToList();
            return View();

        }
        [HttpPost]
        public IActionResult Create(Request request)
        {
            int nextId = _context.Requests.Any() ? _context.Requests.Max(r => r.Requestid) + 1 : 1;
            request.Requestid = nextId;

            request.Startdate = DateOnly.FromDateTime(DateTime.Now);
            request.Requeststatus = "Новая заявка";

            _context.Requests.Add(request);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}