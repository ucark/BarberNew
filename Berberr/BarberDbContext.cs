﻿using Barber.Models;
using Microsoft.EntityFrameworkCore;

namespace Barber
{
    public class BarberDbContext : DbContext
    {
        public BarberDbContext(DbContextOptions<BarberDbContext> options) : base(options)
        {
        }

        public DbSet<Barbers> Barbers { get; set; }
        public DbSet<Customers> Customers { get; set; }
        public DbSet<Employees> Employees { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Server=GUSION\\SQLEXPRESS;Database=Barbers;User Id=sa;Password=123456;Encrypt=False");
            }
        }
    }
}
