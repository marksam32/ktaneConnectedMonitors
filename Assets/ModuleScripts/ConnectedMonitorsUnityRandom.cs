namespace ConnectedMonitors
{
	public class ConnectedMonitorsUnityRandom : IConnectedMonitorsRandom 
	{
		public int Range(int from, int to)
		{
			return UnityEngine.Random.Range(from, to);
		}
	}	
}
