﻿using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ConnectorCore.Models;
using ConnectorCore.Models.Connections;

namespace ConnectorCenter.Data
{
    public class DataBaseContext : IdentityDbContext
    {
        public DataBaseContext(DbContextOptions<DataBaseContext> options) : base(options) => Database.EnsureCreated();
        public DbSet<AppUser> AppUsers { get; set; }
        public DbSet<UserSettings> UserSettings { get; set; }
        public DbSet<UserGroup> UserGroups { get; set; }
        public DbSet<Server> Servers { get; set; }
        public DbSet<Connection> Connections { get; set; }
    }
}