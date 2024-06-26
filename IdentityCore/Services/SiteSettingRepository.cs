﻿using IdentityCore.Data;
using IdentityCore.Tools;

namespace IdentityCore.Services
{
    public class SiteSettingRepository : ISiteSettingRepository
    {
        #region Ctor

        private readonly ApplicationDbContext _context;

        public SiteSettingRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        #endregion

        public EmailSetting GetDefaultEmail()
        {
            return _context.EmailSettings.FirstOrDefault(s => !s.IsDelete && s.IsDefault)!;
        }
    }
}
