using Microsoft.AspNetCore.Mvc;
using Uset_zayavok.Models;
using System.Linq;
using Uset_zayavok.Data;

namespace Uset_zayavok.Controllers
{
    public class ListController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ListController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index(string search)
        {
            var requests = _context.Requests.AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                requests = requests.Where(r => r.Hometechmodel.Contains(search) ||
                                             r.Requestid.ToString() == search);
            }

            return View(requests.ToList());
        }
    }
} 