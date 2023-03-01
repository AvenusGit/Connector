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

        public async Task<Connection?> GetByIdWithServerOnly(long id)
        {
            return await _context.Connections
                        .Include(srv => srv.Server)
                        .FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task<Connection?> Remove(Connection connection)
        {
            _context.Connections.Remove(connection);
            await _context.SaveChangesAsync();
            return connection;
        }

        public async Task<Connection?> RemoveById(long id)
        {
            Connection? connection = await _context.Connections
                        .Include(srv => srv.Server)
                        .FirstOrDefaultAsync(m => m.Id == id);
            if (connection is null) return null;
            _context.Connections.Remove(connection);
            await _context.SaveChangesAsync();
            return connection;
        }

        public async Task Update(Connection connection)
        {
            _context.Update(connection);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Check on exisisting connection in database
        /// </summary>
        /// <param name="id">Connection identifier</param>
        /// <returns>True - connection exist, otherwise - false.</returns>
        public async Task<bool> ConnectionExists(long id)
        {
            return await _context.Connections.AnyAsync(conn => conn.Id == id);
        }
    }
}
