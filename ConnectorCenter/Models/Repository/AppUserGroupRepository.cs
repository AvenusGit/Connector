using ConnectorCenter.Data;
using ConnectorCore.Models;
using ConnectorCore.Models.Connections;
using ExtendedXmlSerializer;
using ExtendedXmlSerializer.Configuration;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace ConnectorCenter.Models.Repository
{
    public class AppUserGroupRepository : IRepository<AppUserGroup>
    {
        private DataBaseContext _dbContext;
        public AppUserGroupRepository(DataBaseContext context) 
        {
            _dbContext= context;
        }
        public async Task<AppUserGroup?> GetById(long Id)
        {
            return await _dbContext.UserGroups
                        .Include(userGroup => userGroup.Connections)
                            .ThenInclude(conn => conn.Server)
                                .ThenInclude(srv => srv.Connections)
                        .Include(userGroup => userGroup.Connections)
                            .ThenInclude(gr => gr.ServerUser)
                                .ThenInclude(su => su.Credentials)
                        .Include(gr => gr.Users)
                            .ThenInclude(usr => usr.Credentials)
                        .FirstOrDefaultAsync(group => group.Id == Id);
        }
        public async Task<AppUserGroup?> GetByIdSimple(long Id)
        {
            return await _dbContext.UserGroups.FirstOrDefaultAsync(usr => usr.Id == Id);
        }
        public async Task<AppUserGroup?> GetByIdUsersOnly(long Id)
        {
            return await _dbContext.UserGroups
                        .Include(gr => gr.Users)
                            .ThenInclude(usr => usr.Credentials)
                        .FirstOrDefaultAsync(usr => usr.Id == Id);
        }
        public async Task<AppUserGroup?> GetByIdWithConnections(long id)
        {
            return await _dbContext.UserGroups
                        .Include(usr => usr.Connections)
                            .ThenInclude(conn => conn.ServerUser)
                                .ThenInclude(usr => usr!.Credentials)
                        .Include(usr => usr.Connections)
                            .ThenInclude(conn => conn.Server)
                        .FirstOrDefaultAsync(usr => usr.Id == id);
        }

        public async Task<IEnumerable<AppUserGroup>> GetAll()
        {
            return await _dbContext.UserGroups
                        .Include(gr => gr.Connections)
                        .Include(gr => gr.Users)
                        .ToListAsync();
        }

        public async Task Add(AppUserGroup element)
        {
            await _dbContext.UserGroups.AddAsync(element);
            await _dbContext.SaveChangesAsync();
        }

        public async Task Remove(AppUserGroup element)
        {
            _dbContext.UserGroups.Remove(element);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<AppUserGroup?> RemoveById(long id)
        {
            AppUserGroup? appUserGroup = await _dbContext.UserGroups
                        .Include(gr => gr.Users)
                        .Include(gr => gr.Connections)
                        .FirstOrDefaultAsync(usr => usr.Id == id);
            if (appUserGroup != null)
            {
                appUserGroup.Connections.Clear();
                appUserGroup.Users.Clear(); // очистка пользователей и подключений, чтобы они не были удалены каскадно
                _dbContext.Update(appUserGroup);
                await _dbContext.SaveChangesAsync();
                _dbContext.UserGroups.Remove(appUserGroup);
                await _dbContext.SaveChangesAsync();
                return appUserGroup;
            }
            return null;
        }

        public async Task Update(AppUserGroup element)
        {
            _dbContext.Update(element);
            await _dbContext.SaveChangesAsync();
        }

        public async Task AddConnection(AppUserGroup group, Connection connection)
        {
            group.Connections.Add(connection);
            _dbContext.Update(group);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<Connection?> DeleteConnection(AppUserGroup group, long connectionId)
        {
            Connection? deletedConnection = await _dbContext.Connections.FindAsync(connectionId);
            if(deletedConnection is null)
                return null;
            group.Connections.Remove(deletedConnection);
            _dbContext.Update(group);
            await _dbContext.SaveChangesAsync();
            return deletedConnection;
        }

        public async Task AddUser(AppUserGroup group, AppUser user)
        {
            group.Users.Add(user);
            _dbContext.Update(group);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteUser(AppUserGroup group, long userId)
        {
            AppUser? appUser= await _dbContext.Users.FindAsync(userId);
            if(appUser is null) return;
            group.Users.Remove(appUser);
            _dbContext.Update(group);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<int> Count()
        {
            return await _dbContext.Users.CountAsync();
        }
    }
}
