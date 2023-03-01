using ConnectorCenter.Data;
using ConnectorCore.Models;
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
        public Task<Server?> GetById(long Id)
        {
            throw new NotImplementedException();
        }
        public Task Remove(Server element)
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

        public async Task<int> Count()
        {
            return await _dbContext.Servers.CountAsync();
        }
    }
}
