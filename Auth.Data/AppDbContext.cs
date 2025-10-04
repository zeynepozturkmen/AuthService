﻿using Auth.Entities;
using Microsoft.EntityFrameworkCore;

namespace Auth.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<ApiKey> ApiKeys { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    }
}
