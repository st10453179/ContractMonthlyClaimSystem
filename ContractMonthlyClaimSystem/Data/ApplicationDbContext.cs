using System.Collections.Generic;
using System.Data.Entity;
using System.Reflection.Emit;
using ContractMonthlyClaimSystem.Models;

namespace ContractMonthlyClaimSystem.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

            public DbSet<Lecture> Lecturers { get; set; }
            public DbSet<Claim> Claims { get; set; }
            public DbSet<Invoice> Invoices { get; set; }
        public object Lectures { get; internal set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                base.OnModelCreating(modelBuilder);

                // Configure relationships
                modelBuilder.Entity<Claim>()
                    .HasOne(c => c.Lecturer)
                    .WithMany(l => l.Claims)
                    .HasForeignKey(c => c.LecturerId);

                // Seed initial data (
                modelBuilder.Entity<Lecture>().HasData(
                    new Lecture
                    {
                        LecturerId = 1,
                        FirstName = "Bridget",
                        LastName = "Moganedi",
                        Email = "bridget.moganedi@gmail.com",
                        PhoneNumber = "0945876789",
                        Department = "Computer Science",
                        DateJoined = DateTime.Now.AddYears(-2)
                    }
                );
            }
        }
    }

