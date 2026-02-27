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
    }
}