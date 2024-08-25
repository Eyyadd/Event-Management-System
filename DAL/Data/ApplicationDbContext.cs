using DAL.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext() { }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }



        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Event>()
                    .HasOne(e => e.user) 
                    .WithMany()
                    .HasForeignKey(e => e.OrganizerId)
                    .OnDelete(DeleteBehavior.Restrict);
        }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<EventBooking> EventBooking { get; set; }
        public DbSet<Feedback> Feedbacks { get; set; }
        public DbSet<Tickets> Tickets { get; set; }

    }
}
