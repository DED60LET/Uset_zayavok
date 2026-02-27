using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Uset_zayavok.Data;
using Uset_zayavok.Models;


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
            var requests = _context.Requests.Include(r => r.Client).AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                requests = requests.Where(r => r.Hometechmodel.Contains(search) ||
                                             r.Requestid.ToString() == search);
            }

            return View(requests.ToList());
        }
    }
}