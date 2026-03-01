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

        public IActionResult Status()
        {
            var stats = _context.Requests
        .GroupBy(r => r.Requeststatus)
        .Select(g => new { Status = g.Key ?? "Не указан", Count = g.Count() })
        .ToDictionary(x => x.Status, x => x.Count);

            ViewBag.TotalRequests = _context.Requests.Count();
            return View(stats);
        }

        public IActionResult Edit(int id)
        {
            var request = _context.Requests.Find(id);
            if (request == null) return NotFound();
            ViewBag.Statuses = new List<string>
             {
            "Новая заявка",
            "В процессе ремонта",
            "Готова к выдаче",
            "Завершена"
            };

            return View(request);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Request request)
        {
            if (id != request.Requestid) return NotFound();

            try
            {
                var dbEntry = _context.Requests.Find(id);
                if (dbEntry == null) return NotFound();
                dbEntry.Requeststatus = request.Requeststatus;
                dbEntry.Problemdescryption = request.Problemdescryption;
                dbEntry.Repairparts = request.Repairparts;
                if (request.Requeststatus == "Готова к выдаче")
                {
                    dbEntry.Completiondate = DateOnly.FromDateTime(DateTime.Now);
                }

                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View(request);
            }
        }
    }

} 