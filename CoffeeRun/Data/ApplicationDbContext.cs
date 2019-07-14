using System;
using System.Collections.Generic;
using System.Text;
using CoffeeRun.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using CoffeeRun.ViewModels;

namespace CoffeeRun.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Friend> Friends { get; set; }
        public virtual DbSet<Run> Runs { get; set; }
        public virtual DbSet<Order> Orders { get; set; }
        public DbSet<CoffeeRun.ViewModels.DashboardViewModel> DashboardViewModel { get; set; }
    }
}
