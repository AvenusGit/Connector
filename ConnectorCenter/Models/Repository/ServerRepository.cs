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

        public async Task Add(Server server)
        {
            await _dbContext.Servers.AddAsync(server);
            await _dbContext.SaveChangesAsync();
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
        public async Task<Server?> Remove(Server server)
        {
            _dbContext.Servers.Remove(server);
            await _dbContext.SaveChangesAsync();
            return server;
        }
        public async Task<Server?> RemoveById(long Id)
        {
            Server? server = await _dbContext.Servers.FindAsync(Id);
            if (server == null) return null;
            _dbContext.Servers.Remove(server);
            await _dbContext.SaveChangesAsync();
            return server;
        }
        public async Task Update(Server server)
        {
            _dbContext.Update(server);
            await _dbContext.SaveChangesAsync();
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

        public async Task<bool> ServerExist(long id)
        {
            return await _dbContext.Servers.AnyAsync(srv => srv.Id == id);
        }
    }
}
