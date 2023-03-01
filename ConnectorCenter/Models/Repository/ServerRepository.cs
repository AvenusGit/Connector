using ConnectorCenter.Data;
using ConnectorCore.Models;
using ConnectorCore.Models.Connections;
using ExtendedXmlSerializer.Configuration;
using Microsoft.EntityFrameworkCore;

namespace ConnectorCenter.Models.Repository
{
    public class ServerRepository : IRepository<Server>
    {
        private DataBaseContext _dbContext;
        public ServerRepository(DataBaseContext context)
        {
            _dbContext = context;
        }

        public Task Add(Server element)
        {
            throw new NotImplementedException();
        }
        public async Task<IEnumerable<Server>> GetAll()
        {
            return await  _dbContext.Servers
                        .Include(srv => srv.Connections)
                            .ThenInclude(conn => conn.ServerUser)
                                .ThenInclude(usr => usr!.Credentials)
                        .ToListAsync();
        }
        public async Task<Server?> GetById(long id)
        {
            return await _dbContext.Servers
                        .Include(srv => srv.Connections)
                            .ThenInclude(conn => conn.ServerUser)
                                .ThenInclude(usr => usr!.Credentials)
                        .FirstOrDefaultAsync(srv => srv.Id == id);
        }
        public async Task<Server?> GetByIdSimple(long id)
        {
            return await _dbContext.Servers.FindAsync(id);
        }
        public Task<Server> Remove(Server element)
        {
            throw new NotImplementedException();
        }
        public Task<Server?> RemoveById(long Id)
        {
            throw new NotImplementedException();
        }
        public Task Update(Server element)
        {
            throw new NotImplementedException();
        }

        public async Task AddConnection(Server server, Connection connection)
        {
            server.Connections.Add(connection);
            _dbContext.Update(server);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<int> Count()
        {
            return await _dbContext.Servers.CountAsync();
        }
    }
}
