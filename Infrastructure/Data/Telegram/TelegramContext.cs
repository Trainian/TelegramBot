using ApplicationCore.Entities.Telegram;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Data.Telegram
{
    public class TelegramContext : DbContext
    {
        public TelegramContext (DbContextOptions<TelegramContext> options)
            : base(options)
        {
            //Database.EnsureCreated();
        }

        public DbSet<Problem> Problems { get; set; }
        public DbSet<TelegramUser> Users { get; set; }
        public DbSet<Answer> Answers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Problem>()
                .HasOne(p => p.UserCreateProblem)
                .WithMany(u => u.Problems)
                .HasForeignKey(k => k.UserCreateProblemId);

            modelBuilder.Entity<Problem>()
                .HasOne(p => p.UserGetProblem)
                .WithMany(u => u.GetProblems)
                .HasForeignKey(k => k.UserGetProblemId);

            modelBuilder.Entity<Problem>()
                .HasMany(p => p.Answers)
                .WithOne(a => a.Problem);

            modelBuilder.Entity<TelegramUser>()
                .HasMany(u => u.Problems)
                .WithOne(p => p.UserCreateProblem);

            modelBuilder.Entity<TelegramUser>()
                .HasMany(u => u.GetProblems)
                .WithOne(p => p.UserGetProblem);

            modelBuilder.Entity<TelegramUser>()
                .HasMany(u => u.Answers)
                .WithOne(a => a.UserCreate);

            modelBuilder.Entity<Answer>()
                .HasOne(a => a.UserCreate)
                .WithMany(u => u.Answers)
                .HasForeignKey(k => k.UserCreateId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Answer>()
                .HasOne(a => a.Problem)
                .WithMany(p => p.Answers)
                .HasForeignKey(k => k.ProblemId)
                .OnDelete(DeleteBehavior.Restrict);

            base.OnModelCreating(modelBuilder);
        }
    }
}
