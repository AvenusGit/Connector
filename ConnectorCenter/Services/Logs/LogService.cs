namespace ConnectorCenter.Services.Logs
{
    public static class LogService
    {
        public static string GetLastLogFilePath()
        {
            switch (Environment.OSVersion.Platform)
            {
                case PlatformID.Unix:
                    return @"/var/log/connectorCenter/connectorCenter.log";
                case PlatformID.Win32NT:
                    return Environment.GetEnvironmentVariable("LOCALAPPDATA") + @"\connectorCenter\Logs\connectorCenter.log";
                default: return "connectorCenter.log";
            }
        }
    }
}
