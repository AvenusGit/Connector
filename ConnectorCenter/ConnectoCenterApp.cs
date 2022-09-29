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
        public readonly ApplicationVersion ApplicationVersion = new ApplicationVersion("A", 0);
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
        public ApiSettings ApiSettings { get; set; } = ApiSettings.GetDefault();
        public OtherSettings OtherSettings { get; set; } = OtherSettings.GetDefault();
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
                    configuration = SettingsConfigurationService.LoadConfiguration(UserAccessSettings);
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
                    configuration = SettingsConfigurationService.LoadConfiguration(SupportAccessSettings);
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
                    LogSettings = LogSettings.LoadConfiguration();
                }
                catch (Exception ex)
                {
                    LogSettings = LogSettings.GetDefault();
                    Logger.LogError($"Не удалось обработать конфигурацию логгера. {ex.Message}. {ex.StackTrace}");
                }

                try
                {
                    configuration = SettingsConfigurationService.LoadConfiguration(ApiSettings);
                    if (configuration is not null && configuration is ApiSettings)
                    {
                        ApiSettings = (ApiSettings)configuration;
                        Logger.LogInformation("Конфигурация API успешно загружена.");
                    }

                    else
                    {
                        SettingsConfigurationService.SaveConfiguration(ApiSettings.GetDefault());
                        Logger.LogError("Не удалось загрузить конфигурацию API. Конфигурация установлена и перезаписана по умолчанию.");
                    }
                }
                catch (Exception ex)
                {
                    ApiSettings = ApiSettings.GetDefault();
                    Logger.LogError($"Не удалось обработать конфигурацию API. {ex.Message}. {ex.StackTrace}");
                }

                try
                {
                    configuration = SettingsConfigurationService.LoadConfiguration(OtherSettings);
                    if (configuration is not null && configuration is OtherSettings)
                    {
                        OtherSettings = (OtherSettings)configuration;
                        Logger.LogInformation("Конфигурация прочих настроек успешно загружена.");
                    }

                    else
                    {
                        SettingsConfigurationService.SaveConfiguration(OtherSettings.GetDefault());
                        Logger.LogError("Не удалось загрузить конфигурацию прочих настроек. Конфигурация установлена и перезаписана по умолчанию.");
                    }
                }
                catch (Exception ex)
                {
                    OtherSettings = OtherSettings.GetDefault();
                    Logger.LogError($"Не удалось обработать конфигурацию прочих настроек. {ex.Message}. {ex.StackTrace}");
                }
            }
        }
        #endregion
    }
}
