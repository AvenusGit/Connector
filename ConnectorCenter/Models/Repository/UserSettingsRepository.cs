using ConnectorCenter.Data;
using ConnectorCore.Models;
using ConnectorCore.Models.VisualModels;
using Microsoft.EntityFrameworkCore;

namespace ConnectorCenter.Models.Repository
{
    public class UserSettingsRepository : IRepository<UserSettings>
    {
        #region Fields
        private readonly DataBaseContext _context;
        #endregion
        #region Constructors
        public UserSettingsRepository(DataBaseContext context) 
        {
            _context = context;
        }
        #endregion
        public Task Add(UserSettings element)
        {
            throw new NotImplementedException();
        }

        public async Task<int> Count()
        {
            return await _context.UserSettings.CountAsync();
        }

        public async Task<IEnumerable<UserSettings>> GetAll()
        {
            return await _context.UserSettings
                .Include(us => us.RdpSettings)
                .ToArrayAsync();
        }

        public async Task<UserSettings?> GetById(long id)
        {
            return await _context.UserSettings
                .Include(us => us.RdpSettings)
                .FirstOrDefaultAsync(us => us.Id == id);
        }

        public async Task<UserSettings?> GetByUserId(long userId)
        {
            return await _context.UserSettings
                .Include(us => us.RdpSettings)
                .FirstOrDefaultAsync(us => us.AppUserId == userId);
        }

        public Task<UserSettings?> Remove(UserSettings? element)
        {
            throw new NotImplementedException();
        }

        public Task<UserSettings?> RemoveById(long Id)
        {
            throw new NotImplementedException();
        }

        public async Task Update(UserSettings userSettings)
        {
            _context.Update(userSettings);
            await _context.SaveChangesAsync();
        }
        public async Task<ColorScheme?> GetColorSchemeById(long id)
        {
            return await _context.ColorSchemes.FindAsync(id);
        }
        public async Task UpdateColorScheme(ColorScheme colorScheme)
        {
            _context.Update(colorScheme);
            await _context.SaveChangesAsync();
        }
        public async Task UpdateFontScheme(FontScheme fontScheme)
        {
            _context.Update(fontScheme);
            await _context.SaveChangesAsync();
        }
        public async Task UpdateRdpSettings(RdpSettings rdpSettings)
        {
            _context.Update(rdpSettings);
            await _context.SaveChangesAsync();
        }
    }
}
