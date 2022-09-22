using ConnectorCore.Models;
using ConnectorCenter.Models.Settings;
using ConnectorCenter.Services.Configurations;
using ConnectorCenter.Services.Authorize;
using ConnectorCenter.Models.Statistics;

namespace ConnectorCenter
{
    public class ConnectorCenterApp
    {
        #region Singletone
        public readonly ApplicationVersion ApplicationVersion = new ApplicationVersion()
        {
            VersionSeries = "A",
            VersionNumber = 0
        };
        public readonly string ConfigurationFolderPath = Environment.CurrentDirectory.ToString() + "\\Configurations";

        private static ConnectorCenterApp _connectorCenterApp;
        public static ConnectorCenterApp Instance
        {
            get
            {
                if(_connectorCenterApp is null)
                    _connectorCenterApp = new ConnectorCenterApp();
                return _connectorCenterApp;
            }
        }
        public static ConnectorCenterApp CreateInstance(ILogger logger)
        {
            _connectorCenterApp = new ConnectorCenterApp()
            {
                Logger = logger
            };
            return _connectorCenterApp;
        }
        #endregion
        public ILogger Logger { get; set; }
        #region Settings
        public AccessSettings UserAccessSettings { get; set; } = AccessSettings.GetUserDefault();
        public AccessSettings SupportAccessSettings { get; set; } = AccessSettings.GetSupportDefault();
        public LogSettings LogSettings { get; set; }
        public Statistic Statistics { get; private set; } = new Statistic();
        #endregion
        #region Methods
        public void Initialize()
        {
            object? configuration;
            using (var scope = Logger.BeginScope($"ConnectorCenterApp"))
            {
                try
                {
                    configuration = SettingsConfigurationService.LoadConfiguration(AccessSettings.GetUserDefault());
                    if (configuration is not null && configuration is AccessSettings)
                    {
                        UserAccessSettings = (AccessSettings)configuration;
                        Logger.LogInformation("Конфигурация доступов пользователей успешно загружена.");
                    }

                    else
                    {
                        SettingsConfigurationService.SaveConfiguration(AccessSettings.GetUserDefault());
                        Logger.LogError("Не удалось загрузить конфигурацию доступов пользователя. Конфигурация установлена и перезаписана по умолчанию.");
                    }


                }
                catch (Exception ex)
                {
                    UserAccessSettings = AccessSettings.GetUserDefault();
                    Logger.LogError($"Не удалось обработать конфигурацию пользовательских доступов. {ex.Message}. {ex.StackTrace}");
                }

                try
                {
                    configuration = SettingsConfigurationService.LoadConfiguration(AccessSettings.GetSupportDefault());
                    if (configuration is not null && configuration is AccessSettings)
                    {
                        SupportAccessSettings = (AccessSettings)configuration;
                        Logger.LogInformation("Конфигурация доступов техподдержки успешно загружена.");
                    }

                    else
                    {
                        SettingsConfigurationService.SaveConfiguration(AccessSettings.GetSupportDefault());
                        Logger.LogError("Не удалось загрузить конфигурацию доступов техподдержки. Конфигурация установлена и перезаписана по умолчанию.");
                    }
                }
                catch (Exception ex)
                {
                    SupportAccessSettings = AccessSettings.GetSupportDefault();
                    Logger.LogError($"Не удалось обработать конфигурацию доступов техподдержки. {ex.Message}. {ex.StackTrace}");
                }

                try
                {
                    LogSettings = new LogSettings().LoadConfiguration();
                }
                catch (Exception ex)
                {
                    LogSettings = LogSettings.GetDefault();
                    Logger.LogError($"Не удалось обработать конфигурацию логгера. {ex.Message}. {ex.StackTrace}");
                }
            }
        }
        #endregion
    }
}
