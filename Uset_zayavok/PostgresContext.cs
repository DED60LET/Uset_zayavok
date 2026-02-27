using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Uset_zayavok.Models;

namespace Uset_zayavok;

public partial class PostgresContext : DbContext
{
    public PostgresContext()
    {
    }

    public PostgresContext(DbContextOptions<PostgresContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Comment> Comments { get; set; }

    public virtual DbSet<Request> Requests { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=postgres;Username=postgres;Password=123456789");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Comment>(entity =>
        {
            entity.HasKey(e => e.Commentid).HasName("comments_pkey");

            entity.ToTable("comments");

            entity.Property(e => e.Commentid)
                .ValueGeneratedNever()
                .HasColumnName("commentid");
            entity.Property(e => e.Masterid).HasColumnName("masterid");
            entity.Property(e => e.Message).HasColumnName("message");
            entity.Property(e => e.Requestid).HasColumnName("requestid");

            entity.HasOne(d => d.Master).WithMany(p => p.Comments)
                .HasForeignKey(d => d.Masterid)
                .HasConstraintName("comments_masterid_fkey");

            entity.HasOne(d => d.Request).WithMany(p => p.Comments)
                .HasForeignKey(d => d.Requestid)
                .HasConstraintName("comments_requestid_fkey");
        });

        modelBuilder.Entity<Request>(entity =>
        {
            entity.HasKey(e => e.Requestid).HasName("requests_pkey");

            entity.ToTable("requests");

            entity.Property(e => e.Requestid)
                .ValueGeneratedNever()
                .HasColumnName("requestid");
            entity.Property(e => e.Clientid).HasColumnName("clientid");
            entity.Property(e => e.Completiondate).HasColumnName("completiondate");
            entity.Property(e => e.Hometechmodel)
                .HasMaxLength(100)
                .HasColumnName("hometechmodel");
            entity.Property(e => e.Hometechtype)
                .HasMaxLength(100)
                .HasColumnName("hometechtype");
            entity.Property(e => e.Masterid).HasColumnName("masterid");
            entity.Property(e => e.Problemdescryption).HasColumnName("problemdescryption");
            entity.Property(e => e.Repairparts).HasColumnName("repairparts");
            entity.Property(e => e.Requeststatus)
                .HasMaxLength(50)
                .HasColumnName("requeststatus");
            entity.Property(e => e.Startdate).HasColumnName("startdate");

            entity.HasOne(d => d.Client).WithMany(p => p.RequestClients)
                .HasForeignKey(d => d.Clientid)
                .HasConstraintName("requests_clientid_fkey");

            entity.HasOne(d => d.Master).WithMany(p => p.RequestMasters)
                .HasForeignKey(d => d.Masterid)
                .HasConstraintName("requests_masterid_fkey");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Userid).HasName("users_pkey");

            entity.ToTable("users");

            entity.Property(e => e.Userid)
                .ValueGeneratedNever()
                .HasColumnName("userid");
            entity.Property(e => e.Fio)
                .HasMaxLength(255)
                .HasColumnName("fio");
            entity.Property(e => e.Login)
                .HasMaxLength(50)
                .HasColumnName("login");
            entity.Property(e => e.Password)
                .HasMaxLength(50)
                .HasColumnName("password");
            entity.Property(e => e.Phone)
                .HasMaxLength(20)
                .HasColumnName("phone");
            entity.Property(e => e.Type)
                .HasMaxLength(50)
                .HasColumnName("type");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
