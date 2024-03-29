﻿using FreshFarmMarket.ViewModels;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FreshFarmMarket.Model
{
    public class UserDbContext : IdentityDbContext<ApplicationUser>
    {

        private readonly IConfiguration _configuration;
        public UserDbContext(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string connectionString = _configuration.GetConnectionString("AuthConnectionString"); optionsBuilder.UseSqlServer(connectionString);
        }

		public DbSet<AuditLog> AuditLogs { get; set; }
	}
}
