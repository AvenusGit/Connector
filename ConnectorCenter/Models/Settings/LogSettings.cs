using System.Xml.Linq;
using System.Xml;
using ConnectorCenter.Services.Logs;
using System.Xml.Serialization;

namespace ConnectorCenter.Models.Settings
{
    public class LogSettings // не реализует ISettingsConfiguration из-за разной реализации чтения/записи
    {
        [XmlIgnoreAttribute]
        public static string ConfigurationPath
        {
            get
            {
                return ConnectorCenterApp.Instance.ConfigurationFolderPath + @"\Log.config";
            }

        }
        public LogSettings() { }
        public LogSettings(LogLevels logLevel, string logPath, int logFileCount, string logFileSize, string pattern)
        {
            LogLevel = logLevel;
            LogPath = logPath;
            LogFileCount = logFileCount;
            LogFileSize = logFileSize;
            Pattern = pattern;
        }
        public static LogSettings GetDefault()
        {
            return new LogSettings(LogLevels.INFO, LogService.GetLastLogFilePath(), 3, "100KB",
                "[ %-5p ] %d{dd.MM.yyyy HH:mm:ss} [%property{scope}] %m%n");
        }
        public LogLevels LogLevel { get; set; }
        public string LogPath { get; set; }
        public int LogFileCount { get; set; }
        public string LogFileSize { get; set; }
        public string Pattern { get; set; }
        public enum LogLevels
        {
            ALL,
            DEBUG,
            INFO,
            WARN,
            ERROR,
            FATAL,
            OFF
        }

        public static void SaveDefaultConfiguration()
        {
            CreateOrChangeLoggerConfiguration(GetDefault());
        }
        public void SaveConfiguration()
        {
            CreateOrChangeLoggerConfiguration(this);
        }
        public static LogSettings LoadConfiguration()
        {
            try
            {
                LogSettings configuration = new LogSettings();
                XmlDocument doc = new XmlDocument();
                doc.Load(ConfigurationPath);
                bool result;

                XmlNode node = doc.SelectSingleNode("/log4net/root/level");
                configuration.LogLevel = (LogLevels)Enum.Parse(typeof(LogLevels), node.Attributes["value"].Value);

                node = doc.SelectSingleNode("/log4net/appender/param[@name='File']");
                configuration.LogPath = node.Attributes["value"].Value;

                int res;
                node = doc.SelectSingleNode("/log4net/appender/maxSizeRollBackups");
                if (int.TryParse(node.Attributes["value"].Value, out res))
                    configuration.LogFileCount = res;

                node = doc.SelectSingleNode("/log4net/appender/maximumFileSize");
                configuration.LogFileSize = node.Attributes["value"].Value;

                node = doc.SelectSingleNode("/log4net/appender/layout/param");
                configuration.Pattern = node.Attributes["value"].Value;
                if (configuration is null)
                {
                    SaveDefaultConfiguration();
                    return GetDefault();
                }
                return configuration;
            }
            catch
            {
                SaveDefaultConfiguration();
                return GetDefault();
            }
        }
        private static void CreateOrChangeLoggerConfiguration(LogSettings conf)
        {
            XDocument document = GenerateXmlDocument(conf);
            Stream stream = new FileStream(ConfigurationPath, FileMode.OpenOrCreate);
            document.Save(stream);
            stream.Close();
        }
        private static XDocument GenerateXmlDocument(LogSettings conf)
        {
            XDocument document = new XDocument();
            XComment warning = new XComment("WARNING: not use RU char in comment!");
            document.Add(warning);
            XElement log4net = new XElement("log4net");
            document.Add(log4net);

            XElement root = new XElement("root");
            log4net.Add(root);

            XComment variants = new XComment("Log level variants: ALL, DEBUG, INFO, WARN, ERROR, FATAL, OFF. Default: ERROR.");
            root.Add(variants);

            XElement level = new XElement("level");
            XAttribute levelValue = new XAttribute("value", conf.LogLevel.ToString());
            level.Add(levelValue);
            root.Add(level);

            XComment consoleWarn = new XComment("Write Log4Net records in console. Uncomment for enable it.");
            root.Add(consoleWarn);
            XComment consoleString = new XComment(@"<appender-ref ref=""console""/>");
            root.Add(consoleString);

            XElement appenderRef = new XElement("appender-ref");
            XAttribute refAttr = new XAttribute("ref", "file");
            appenderRef.Add(refAttr);
            root.Add(appenderRef);

            XElement appenderConsole = new XElement("appender");
            XAttribute consoleName = new XAttribute("name", "console");
            appenderConsole.Add(consoleName);
            XAttribute consoleType = new XAttribute("type", "log4net.Appender.ConsoleAppender");
            appenderConsole.Add(consoleType);
            log4net.Add(appenderConsole);

            XElement layotConsole = new XElement("layout");
            XAttribute layotConsoleType = new XAttribute("type", "log4net.Layout.PatternLayout");
            layotConsole.Add(layotConsoleType);
            appenderConsole.Add(layotConsole);

            XElement conversionPatternConsole = new XElement("conversionPattern");
            XAttribute conversionPatternConsoleValue = new XAttribute("value", "%date %level %logger - %message%newline");
            conversionPatternConsole.Add(conversionPatternConsoleValue);
            layotConsole.Add(conversionPatternConsole);

            XElement appenderFile = new XElement("appender");
            XAttribute appenderFileName = new XAttribute("name", "file");
            appenderFile.Add(appenderFileName);
            XAttribute appenderFileType = new XAttribute("type", "log4net.Appender.RollingFileAppender");
            appenderFile.Add(appenderFileType);
            log4net.Add(appenderFile);

            XElement lockingModel = new XElement("lockingModel");
            XAttribute lockingModelType = new XAttribute("type", "log4net.Appender.FileAppender+MinimalLock");
            lockingModel.Add(lockingModelType);
            appenderFile.Add(lockingModel);

            XElement paramPath = new XElement("param");
            XAttribute paramPathName = new XAttribute("name", "File");
            paramPath.Add(paramPathName);
            XAttribute paramPathType = new XAttribute("type", "log4net.Util.PatternString");
            paramPath.Add(paramPathType);
            XAttribute paramPathValue = new XAttribute("value", conf.LogPath);
            paramPath.Add(paramPathValue);
            appenderFile.Add(paramPath);

            XElement paramAppendPath = new XElement("param");
            XAttribute paramAppendPathName = new XAttribute("name", "AppendToFile");
            paramAppendPath.Add(paramAppendPathName);
            XAttribute paramAppendPathValue = new XAttribute("value", "true");
            paramAppendPath.Add(paramAppendPathValue);
            appenderFile.Add(paramAppendPath);

            XElement rollingStyle = new XElement("rollingStyle");
            XAttribute rollingStyleValue = new XAttribute("value", "Size");
            rollingStyle.Add(rollingStyleValue);
            appenderFile.Add(rollingStyle);

            XElement maxSizeRollBackups = new XElement("maxSizeRollBackups");
            XAttribute maxSizeRollBackupsValue = new XAttribute("value", conf.LogFileCount);
            maxSizeRollBackups.Add(maxSizeRollBackupsValue);
            appenderFile.Add(maxSizeRollBackups);

            XElement maximumFileSize = new XElement("maximumFileSize");
            XAttribute maximumFileSizeValue = new XAttribute("value", conf.LogFileSize);
            maximumFileSize.Add(maximumFileSizeValue);
            appenderFile.Add(maximumFileSize);

            XElement staticLogFileName = new XElement("staticLogFileName");
            XAttribute staticLogFileNameValue = new XAttribute("value", "true");
            staticLogFileName.Add(staticLogFileNameValue);
            appenderFile.Add(staticLogFileName);

            XElement layot = new XElement("layout");
            XAttribute layotType = new XAttribute("type", "log4net.Layout.PatternLayout");
            layot.Add(layotType);
            appenderFile.Add(layot);

            XElement layotParam = new XElement("param");
            XAttribute layotParamName = new XAttribute("name", "ConversionPattern");
            layotParam.Add(layotParamName);
            XAttribute layotParamValue = new XAttribute("value", conf.Pattern);
            layotParam.Add(layotParamValue);
            layot.Add(layotParam);
            return document;
        }
        public override string ToString()
        {
            return GenerateXmlDocument(this).ToString();
        }
    }
}

