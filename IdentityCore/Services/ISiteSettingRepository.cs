using IdentityCore.Tools;

namespace IdentityCore.Services
{
    public interface ISiteSettingRepository
    {
        EmailSetting GetDefaultEmail();
    }
}
