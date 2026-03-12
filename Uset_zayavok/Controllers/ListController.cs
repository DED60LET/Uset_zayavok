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

        public IActionResult Index(string search, string statusFilter, string typeFilter)
        {
            var userIdClaim = User.FindFirst("UserId")?.Value;
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            if (string.IsNullOrEmpty(userIdClaim)) return RedirectToAction("Login", "Account");
            int currentUserId = int.Parse(userIdClaim);


            var requests = _context.Requests.Include(r => r.Client).Include(r => r.Master).AsQueryable();


            if (userRole == "Заказчик")
            {
                requests = requests.Where(r => r.Clientid == currentUserId);
            }

            if (!string.IsNullOrEmpty(search))
            {
                requests = requests.Where(r => r.Hometechmodel.Contains(search) || r.Requestid.ToString() == search);
            }


            if (!string.IsNullOrEmpty(statusFilter))
            {
                requests = requests.Where(r => r.Requeststatus == statusFilter);
            }


            if (!string.IsNullOrEmpty(typeFilter))
            {
                requests = requests.Where(r => r.Hometechtype == typeFilter);
            }


            ViewBag.Types = _context.Requests.Select(r => r.Hometechtype).Distinct().ToList();

            return View(requests.ToList());
        }

        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.Masters = _context.Users
                .Where(u => u.Type == "Мастер")
                .ToList();

            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Request request)
        {
            int nextId = _context.Requests.Any() ? _context.Requests.Max(r => r.Requestid) + 1 : 1;
            request.Requestid = nextId;

            request.Startdate = DateOnly.FromDateTime(DateTime.Now);
            request.Requeststatus = "Новая заявка";

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Requests.Add(request);
                    _context.SaveChanges();
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Ошибка БД: " + ex.Message);
                }
            }

           
            ViewBag.Masters = _context.Users
                .Where(u => u.Type == "Мастер")
                .ToList();

            return View(request);
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

        [Authorize(Roles = "Мастер,Администратор,Менеджер")]
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

            ViewBag.Masters = _context.Users.Where(u => u.Type == "Мастер").ToList();

            return View(request);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Request model) 
        {
           
            var existingRequest = _context.Requests.Find(id);

            if (existingRequest == null)
            {
                return NotFound(); 
            }

            if (ModelState.IsValid)
            {
                try
                {
                    
                    existingRequest.Requeststatus = model.Requeststatus;
                 
                    existingRequest.Masterid = model.Masterid;
                    existingRequest.Priority = model.Priority;
                    existingRequest.TimeSpent = model.TimeSpent;

                    
                    if (model.Requeststatus == "Готова к выдаче")
                    {
                        existingRequest.Completiondate = DateOnly.FromDateTime(DateTime.Now);
                    }

                    _context.SaveChanges();
                    return RedirectToAction(nameof(Index)); 
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Ошибка при сохранении: " + ex.Message);
                }
            }

           
            ViewBag.Masters = _context.Users.Where(u => u.Type == "Мастер").ToList();

            return View(model); 
        }


    }
}