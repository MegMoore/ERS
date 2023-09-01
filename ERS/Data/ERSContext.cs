using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ERS.Models;

namespace ERS.Data
{
    public class ERSContext : DbContext
    {
        public ERSContext (DbContextOptions<ERSContext> options)
            : base(options)
        {
        }

        public DbSet<ERS.Models.Employee> Employees { get; set; } = default!;
        public DbSet<Expense> Expenses { get; set; } = default!;
    }
}
