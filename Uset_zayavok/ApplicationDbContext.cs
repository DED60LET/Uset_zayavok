using Microsoft.EntityFrameworkCore;
using Uset_zayavok.Models;

namespace Uset_zayavok
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<Request> Requests { get; set; }
    }
}