namespace ConnectedMonitors
{
    public class MonitorPressContext
    {
        public Monitor LastPressedMax { get; set; }
        public Monitor LastPressedMin { get; set; }
        public bool IsNegativeContext { get; set; }
    }
}