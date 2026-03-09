using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Uset_zayavok.Models;


namespace Uset_zayavok.Controllers
{
    [Authorize]
    public class ListController : Controller
    {
        private readonly PostgresContext _context;

        public ListController(PostgresContext context)
        {
            _context = context;
        }

        public IActionResult Index(string search)
        {
           
            var userIdClaim = User.FindFirst("UserId")?.Value;
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            if (string.IsNullOrEmpty(userIdClaim)) return RedirectToAction("Login", "Account");

            int currentUserId = int.Parse(userIdClaim);

            
            var requests = _context.Requests.Include(r => r.Client).AsQueryable();

            if (userRole == "Заказчик")
            {
                
                requests = requests.Where(r => r.Clientid == currentUserId);
            }
          

           
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

        [HttpGet]
        public IActionResult Status()
        {
            
            var stats = _context.Requests
                .GroupBy(r => r.Requeststatus)
                .Select(g => new { Status = g.Key ?? "Не указан", Count = g.Count() })
                .ToDictionary(x => x.Status, x => x.Count);

            ViewBag.CompletedCount = _context.Requests
        .Count(r => r.Requeststatus == "Готова к выдаче" || r.Requeststatus == "Завершена");


            ViewBag.TotalRequests = _context.Requests.Count();

            ViewBag.TypeStats = _context.Requests
                .GroupBy(r => r.Hometechtype)
                .Select(g => new { Type = g.Key ?? "Не определено", Count = g.Count() })
                .ToDictionary(x => x.Type, x => x.Count);

            
            var completed = _context.Requests
                .Where(r => r.Startdate != null && r.Completiondate != null)
                .ToList();

            double avgDays = 0;
            if (completed.Any())
            {
                avgDays = completed.Average(r =>
                    (r.Completiondate.Value.ToDateTime(TimeOnly.MinValue) -
                     r.Startdate.Value.ToDateTime(TimeOnly.MinValue)).TotalDays);
            }
            ViewBag.AverageTime = Math.Round(avgDays, 1);

            return View(stats);
        }

        [Authorize(Roles = "Мастер,Администратор")]
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
        [HttpGet]
        public IActionResult Employees()
        {
            var staff = _context.Users
                .Where(u => u.Type != "Заказчик")
                .ToList();

            return View(staff);
        }
    }

} 