using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ConnectorCore.Models;
using ConnectorCore.Models.Connections;
using ConnectorCore.Models.VisualModels;

namespace ConnectorCenter.Data
{
    public class DataBaseContext : IdentityDbContext
    {
        protected readonly IConfiguration Configuration;
        public DataBaseContext(DbContextOptions<DataBaseContext> options) : base(options) => Database.EnsureDeleted();
            
        public DbSet<AppUser> Users { get; set; }
        public DbSet<UserSettings> UserSettings { get; set; }
        public DbSet<AppUserGroup> UserGroups { get; set; }
        public DbSet<Server> Servers { get; set; }
        public DbSet<Connection> Connections { get; set; }
        public DbSet<VisualScheme> VisualSchemes { get; set; }
        public DbSet<ColorScheme> ColorSchemes { get; set; }
        public DbSet<FontScheme> FontSchemes { get; set; }
        public DbSet<ColorProperty> ColorProperties { get; set; }
    }
}