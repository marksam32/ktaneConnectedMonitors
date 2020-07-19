namespace ConnectedMonitors
{
    public class Cable
    {
        public Color Color { get; set; }

        public Monitor To { get; set; }

        public Monitor From { get; set; }

        public string Tag { get { return string.Format("Cable_{0}_{1}", From.Index, To.Index); } }

        public CableDirection Direction { get; set; }

        public override string ToString()
        {
            return string.Format("From: {0}; To: {1}; Color = {2}.", From.Index, To.Index, Color.ToString());
        }
    }
}