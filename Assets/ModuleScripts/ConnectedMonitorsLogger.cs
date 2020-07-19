using UnityEngine;

namespace ConnectedMonitors
{
    public class ConnectedMonitorsLogger : IConnectedMonitorsLogger
    {
        private int ModuleId { get; set; }
        
        public ConnectedMonitorsLogger(int moduleId)
        {
            ModuleId = moduleId;
        }

        public void LogMessage(string message, params object[] parameters)
        {
            Debug.LogFormat("[Connected Monitors #{0}] {1}", ModuleId, string.Format(message, parameters));
        }
    }
}