using Microsoft.AspNetCore.Mvc;
using Uset_zayavok.Models;
using System.Linq;

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
            // Получаем все заявки из базы
            var requests = _context.Requests.AsQueryable();

            // Если есть текст поиска (требование 2.3), фильтруем
            if (!string.IsNullOrEmpty(search))
            {
                requests = requests.Where(r => r.Hometechmodel.Contains(search) ||
                                              r.Requestid.ToString() == search);
            }

            return View(requests.ToList()); // Передаем список объектов в HTML
        }
    }
}