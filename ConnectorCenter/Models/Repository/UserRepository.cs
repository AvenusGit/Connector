using ConnectorCenter.Data;
using ConnectorCore.Models;
using ConnectorCore.Models.VisualModels;
using Microsoft.EntityFrameworkCore;

namespace ConnectorCenter.Models.Repository
{
    public class AppUserRepository : IRepository<AppUser>
    {
        #region Fields
        private DataBaseContext _dbContext;
        #endregion
        #region Constructors
        public AppUserRepository(DataBaseContext context)
        {
            _dbContext = context;
        }
        #endregion
        public Task Add(AppUser element)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<AppUser>> GetAll()
        {
            return await _dbContext.Users
                .Include(usr => usr.Credentials)
                .Include(usr => usr.Connections)
                    .ThenInclude(conn => conn.Server)
                .Include(usr => usr.Connections)
                    .ThenInclude(conn => conn.ServerUser)
                        .ThenInclude(susr => susr.Credentials)
                .ToListAsync();
        }

        public async Task<IEnumerable<AppUser>> GetAllCredentialsOnly()
        {
            return await _dbContext.Users
                .Include(usr => usr.Credentials)
                .ToListAsync();
        }

        public async Task<AppUser?> GetById(long Id)
        {
            return await _dbContext.Users
                .Include(usr => usr.Credentials)
                .Include (usr => usr.UserSettings)
                    .ThenInclude(usrsett => usrsett.RdpSettings)
                .Include(usr => usr.Connections)
                    .ThenInclude(conn => conn.Server)
                .Include(usr => usr.Groups)
                .FirstAsync(usr => usr.Id == Id);
        }

        public Task Remove(AppUser element)
        {
            throw new NotImplementedException();
        }

        public Task<AppUser> RemoveById(long Id)
        {
            throw new NotImplementedException();
        }

        public Task Update(AppUser element)
        {
            throw new NotImplementedException();
        }

        public async Task<VisualScheme> GetVisualScheme()
        {
            throw new NotImplementedException();
        }

        public async Task<int> Count()
        {
            return await _dbContext.Users.CountAsync();
        }
    }
}
