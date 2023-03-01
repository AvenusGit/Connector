using ConnectorCenter.Data;
using ConnectorCore.Models.Connections;
using Microsoft.EntityFrameworkCore;

namespace ConnectorCenter.Models.Repository
{
    public class ConnectionRepository : IRepository<Connection>
    {
        #region Fields
        private readonly DataBaseContext _context;
        #endregion
        #region Constructors
        public ConnectionRepository(DataBaseContext context) 
        {
            _context = context;
        }
        #endregion
        public Task Add(Connection element)
        {
            throw new NotImplementedException();
        }

        public async Task<int> Count()
        {
            return await _context.Connections.CountAsync();
        }

        public Task<IEnumerable<Connection>> GetAll()
        {
            throw new NotImplementedException();
        }

        public async Task<Connection?> GetById(long Id)
        {
            return await _context.Connections
                        .Include(conn => conn.Server)
                        .Include(conn => conn.ServerUser)
                            .ThenInclude(user => user!.Credentials)
                        .FirstOrDefaultAsync(conn => conn.Id == Id);
        }

        public Task<Connection> Remove(Connection element)
        {
            throw new NotImplementedException();
        }

        public Task<Connection?> RemoveById(long Id)
        {
            throw new NotImplementedException();
        }

        public Task Update(Connection element)
        {
            throw new NotImplementedException();
        }
    }
}
