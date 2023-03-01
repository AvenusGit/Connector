using ConnectorCenter.Data;
using ConnectorCore.Models;
using ConnectorCore.Models.Connections;
using ConnectorCore.Models.VisualModels;
using ExtendedXmlSerializer.Configuration;
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
        public async Task Add(AppUser element)
        {
            await _dbContext.Users.AddAsync(element);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<AppUser>> GetAll()
        {
            return await _dbContext.Users
                .Include(usr => usr.Groups)
                    .ThenInclude(gr => gr.Connections)
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

        public async Task<AppUser?> GetByIdSimple(long id)
        {
            return await _dbContext.Users.FindAsync(id);
        }

        public async Task<AppUser?> GetByIdWithConnections(long id)
        {
            return await _dbContext.Users
                        .Include(usr => usr.Groups)
                            .ThenInclude(gr => gr.Connections)
                                .ThenInclude(conn => conn.ServerUser)
                                    .ThenInclude(usr => usr!.Credentials)
                        .Include(usr => usr.Groups)
                            .ThenInclude(gr => gr.Connections)
                                .ThenInclude(conn => conn.Server)
                        .Include(usr => usr.Connections)
                            .ThenInclude(conn => conn.ServerUser)
                                .ThenInclude(usr => usr!.Credentials)
                        .Include(usr => usr.Connections)
                            .ThenInclude(conn => conn.Server)
                        .FirstOrDefaultAsync(usr => usr.Id == id);
        }

        public async Task<AppUser?> GetByIdWithConnectionsSimple(long id)
        {
            return await _dbContext.Users
                        .Include(usr => usr.Connections)
                        .FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task<AppUser?> GetByIdWithVisualScheme(long id)
        {
            return await _dbContext.Users
                        .Include(user => user.VisualScheme)
                            .ThenInclude(vs => vs.ColorScheme)
                        .Include(user => user.VisualScheme)
                            .ThenInclude(vs => vs.FontScheme)
                        .FirstOrDefaultAsync(user => user.Id == id);
        }

        public async Task<AppUser?> GetByIdCredentialsOnly(long Id)
        {
            return await _dbContext.Users
                .Include(usr => usr.Credentials)
                .FirstAsync(usr => usr.Id == Id);
        }

        public async Task<AppUser?> Remove(AppUser user)
        {
            user.Connections.Clear(); // no cascade deleting
            _dbContext.Update(user);
            _dbContext.Users.Remove(user);
            await _dbContext.SaveChangesAsync();
            return user;
        }

        public async Task<AppUser?> RemoveById(long id)
        {
            AppUser? user = await _dbContext.Users
                .Include(usr => usr.Connections)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (user is null)
                return null;
            user.Connections.Clear(); // no cascade deleting
            _dbContext.Update(user);
            _dbContext.Users.Remove(user);
            await _dbContext.SaveChangesAsync();
            return user;
        }

        public async Task Update(AppUser element)
        {
            _dbContext.Update(element);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<VisualScheme> GetVisualScheme(long id)
        {
            AppUser? user = await _dbContext.Users
                        .Include(user => user.VisualScheme)
                            .ThenInclude(vs => vs.ColorScheme)
                        .Include(user => user.VisualScheme)
                            .ThenInclude(vs => vs.FontScheme)
                        .FirstOrDefaultAsync(user => user.Id == id);
            if(user is null) return VisualScheme.GetDefaultVisualScheme();
            return user.VisualScheme;
        }

        public async Task SetVisualSchemeToDefault(AppUser user)
        {
            user.VisualScheme = VisualScheme.GetDefaultVisualScheme();
            _dbContext.Update(user);
            await _dbContext.SaveChangesAsync();
        }

        public async Task AddConnection(AppUser user, Connection connection)
        {
            user.Connections.Add(connection);
            _dbContext.Update(user);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteConnection(AppUser user, long connectionId)
        {
            Connection? connection = await _dbContext.Connections.FindAsync(connectionId);
            if (connection is null) return;
            user.Connections.Remove(connection);
            _dbContext.Update(user);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<int> Count()
        {
            return await _dbContext.Users.CountAsync();
        }

        public async Task<bool> IsNotOnlyOneAdministrator(AppUser user)
        {
            if (user.Role != AppUser.AppRoles.Administrator)
                return true;
            return await _dbContext.Users
                                .Where(usr => usr.Role == AppUser.AppRoles.Administrator && usr.Id != user.Id)
                                .AnyAsync();
        }

        public async Task<bool> UserExistByID(long id)
        {
            return await _dbContext.Users.AnyAsync(usr => usr.Id == id);
        }
        public async Task<bool> UserExistByName(string name)
        {
            return await _dbContext.Users.AnyAsync(usr => usr.Name == name);
        }
    }
}
