using System;

namespace ConnectedMonitors
{
	public class MonitorLoggerSetup : IDisposable
	{
		private readonly Monitor monitor_;
		public MonitorLoggerSetup(Monitor monitor)
		{
			monitor_ = monitor;
			monitor_.DebugMonitorIndex();
		}

		public void Dispose()
		{
			monitor_.DebugTotalScore();
		}
	}
}
