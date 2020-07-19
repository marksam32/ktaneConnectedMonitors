using System.Collections.Generic;

namespace ConnectedMonitors
{
	public class DisplayValueRecalculatonResult
	{
		private IList<Monitor> _changed;

		public DisplayValueRecalculatonResult()
		{
			_changed = new List<Monitor>();
		}

		public void Add(Monitor monitor)
		{
			_changed.Add(monitor);
		}
		
		public IList<Monitor> Changed
		{
			get { return _changed; }
		}
		
		public bool HasChangedSign { get; set; }
	}
}
