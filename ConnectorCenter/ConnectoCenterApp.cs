using ConnectorCore.Models;
namespace ConnectorCenter
{
    public class ConnectorCenterApp
    {
        public readonly ApplicationVersion ApplicationVersion = new ApplicationVersion()
        {
            VersionSeries = "A",
            VersionNumber = 0
        };
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
    }
}
