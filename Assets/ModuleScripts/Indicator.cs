namespace ConnectedMonitors
{
    public class IndicatorInfo
    {
        public int Monitor { get; set; }

        public int Index { get; set; }

        public bool Enabled { get; set; }

        public Color Color { get; set; }

        public int GlobalIndex { get; set; }

        public bool IsFlashing { get; set; }

        public override string ToString()
        {
            return string.Format("Index: {0}: Color: {1}, It is {2}", Index + 1, Color.ToString(), AppearenceString());
        }

        private string AppearenceString()
        {
            return (IsFlashing ? "Blinking" : "Solid");
        }
    }
}