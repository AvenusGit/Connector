using ConnectorCore.Models;
using ConnectorCenter.Models.Settings;
using ConnectorCenter.Services.Configurations;
using ConnectorCenter.Services.Authorize;
using ConnectorCenter.Models.Statistics;

namespace ConnectorCenter
{
    /// <summary>
    /// Application main class
    /// Initialization on app start. It is singletone, use Instance property.
    /// </summary>
    public class ConnectorCenterApp
    {
        #region Singletone
        /// <summary>
        /// Application version 
        /// </summary>
        public readonly ApplicationVersion ApplicationVersion = new ApplicationVersion("A", 0);
        /// <summary>
        /// Configuration folder path
        /// </summary>
        public readonly string ConfigurationFolderPath = Environment.CurrentDirectory.ToString() + "\\Configurations";

        private static ConnectorCenterApp _connectorCenterApp;
        /// <summary>
        /// Current instance ConnectorCenterApp class
        /// </summary>
        public static ConnectorCenterApp Instance
        {
            get
            {
                if(_connectorCenterApp is null)
                    _connectorCenterApp = new ConnectorCenterApp();
                return _connectorCenterApp;
            }
        }
        /// <summary>
        /// Create instance ConnectorCenter
        /// </summary>
        /// <param name="logger">Logger</param>
        /// <returns>Instance ConnectorCenter</returns>
        public static ConnectorCenterApp CreateInstance(ILogger logger)
        {
            _connectorCenterApp = new ConnectorCenterApp()
            {
                Logger = logger
            };
            return _connectorCenterApp;
        }
        /// <summary>
        /// Current logger
        /// </summary>
        public ILogger Logger { get; set; }
        #endregion

        #region Settings
        /// <summary>
        /// User access settings configuration
        /// </summary>
        public AccessSettings UserAccessSettings { get; set; } = AccessSettings.GetUserDefault();
        /// <summary>
        /// Support access settings configuration
        /// </summary>
        public AccessSettings SupportAccessSettings { get; set; } = AccessSettings.GetSupportDefault();
        /// <summary>
        /// Logger configuration
        /// </summary>
        public LogSettings LogSettings { get; set; }
        /// <summary>
        /// Api configuration
        /// </summary>
        public ApiSettings ApiSettings { get; set; } = ApiSettings.GetDefault();
        /// <summary>
        /// Other configuration
        /// </summary>
        public OtherSettings OtherSettings { get; set; } = OtherSettings.GetDefault();
        /// <summary>
        /// Current statistics
        /// </summary>
        public Statistic Statistics { get; private set; } = new Statistic();
        #endregion
        #region Methods
        /// <summary>
        /// Load all configuration from configuration path and apply 
        /// </summary>
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
                    LogSettings = LogSettings.LoadConfiguration()!;
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
                        SettingsConfigurationService.SaveConfiguration((OtherSettings)OtherSettings.GetDefault());
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
