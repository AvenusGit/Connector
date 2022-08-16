namespace ConnectorCenter
{
    public class ConnectorCenterApp
    {
        private ConnectorCenterApp _connectorCenterApp;
        public ConnectorCenterApp Instance
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
