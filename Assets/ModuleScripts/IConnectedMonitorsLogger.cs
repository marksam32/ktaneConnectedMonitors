namespace ConnectedMonitors
{
	public interface IConnectedMonitorsLogger
	{
		void LogMessage(string message, params object[] parameters);
	}
}