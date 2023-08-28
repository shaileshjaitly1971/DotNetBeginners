using Microsoft.Extensions.Configuration;

namespace InventoryBeginners.Interfaces
{
    public interface IAppConfig
    {
        string GetConnectionString(string connectionName);
        IConfigurationSection GetConfigurationSection(string Key);
    }
}
