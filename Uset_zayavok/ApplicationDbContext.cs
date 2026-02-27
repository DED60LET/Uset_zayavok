using Microsoft.EntityFrameworkCore;
using Uset_zayavok.Models;

namespace Uset_zayavok.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Request> Requests { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("users");
                entity.HasKey(e => e.Userid);
                entity.Property(e => e.Userid).HasColumnName("userid");
                entity.Property(e => e.Fio).HasColumnName("fio");
                entity.Property(e => e.Phone).HasColumnName("phone");
                entity.Property(e => e.Login).HasColumnName("login");
                entity.Property(e => e.Password).HasColumnName("password");
                entity.Property(e => e.Type).HasColumnName("type");
            });

            modelBuilder.Entity<Request>(entity =>
            {

                entity.ToTable("requests");
                entity.HasKey(e => e.Requestid);
                entity.Property(e => e.Requestid).HasColumnName("requestid");
                entity.Ignore("Userid");
                entity.Ignore("Userid1");
                entity.Property(e => e.Startdate).HasColumnName("startdate");
                entity.Property(e => e.Hometechtype).HasColumnName("hometechtype");
                entity.Property(e => e.Hometechmodel).HasColumnName("hometechmodel");
                entity.Property(e => e.Problemdescryption).HasColumnName("problemdescryption");
                entity.Property(e => e.Requeststatus).HasColumnName("requeststatus");
                entity.Property(e => e.Completiondate).HasColumnName("completiondate");
                entity.Property(e => e.Repairparts).HasColumnName("repairparts");
                entity.Property(e => e.Masterid).HasColumnName("masterid");
                entity.Property(e => e.Clientid).HasColumnName("clientid");
            });

            modelBuilder.Entity<Request>()
                .HasOne(r => r.Client)
                .WithMany()
                .HasForeignKey(r => r.Clientid)
                .HasPrincipalKey(u => u.Userid);

            modelBuilder.Entity<Request>()
                .HasOne(r => r.Master)
                .WithMany()
                .HasForeignKey(r => r.Masterid)
                .HasPrincipalKey(u => u.Userid);
        }
    }
}