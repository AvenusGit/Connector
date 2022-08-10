using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using ConnectorCore.Interfaces;

namespace ConnectorCore.Models.DataBase
{
    public class DataBaseContext : DbContext
    {
        public DataBaseContext() : base("DefaultConnection")
        {

        }
        public DbSet<IUser> Users { get; set; }
    }
}
