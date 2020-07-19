using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System;
using System.Text;

namespace ConnectedMonitors
{
    public class Monitor
    {
        private List<Cable> outCables_ = new List<Cable>();

        private List<Cable> inCables_ = new List<Cable>();

        private List<IndicatorInfo> indicators_ = new List<IndicatorInfo>();

        private int score_;

        private readonly IConnectedMonitorsLogger _connectedMonitorsLogger;

        public Monitor(int index, IConnectedMonitorsLogger connectedMonitorsLogger)
        {
            Index = index;
            _connectedMonitorsLogger = connectedMonitorsLogger;
        } 

        public int Index { get; private set; }

        public int DisplayValue { get; set; }

        public int Score
        {
            get { return score_; }
            set { score_ = value; }
        }

        public bool IsPressed { get; set; }

        public IList<Monitor> GetGreenNonPressedMonitors()
        {
            var monitors = inCables_.Where(x => x.From.IsGreen && !x.From.IsPressed).Select(z => z.From).ToList();
            monitors.AddRange(outCables_.Where(x => x.To.IsGreen && !x.To.IsPressed).Select(z => z.To));
            //_connectedMonitorsLogger.LogMessage("Incables: " + string.Join(", ", inCables_.Select(x => x.Tag).ToArray()));
            //_connectedMonitorsLogger.LogMessage("OutCables: " + string.Join(", ", outCables_.Select(x => x.Tag).ToArray()));

            return monitors;
        }

        public  Color MonitorColor { get; set; }

        public DisplayColor DisplayColor { get; set; }

        public bool IsGreen { get { return DisplayColor == DisplayColor.Green; } }

        public int IndicatorCount { get { return indicators_.Count; } }

        public ReadOnlyCollection<IndicatorInfo> Indicators { get { return indicators_.AsReadOnly(); } }

        public ReadOnlyCollection<Cable> OutCables { get { return outCables_.AsReadOnly(); } }

        public ReadOnlyCollection<Cable> InCables { get { return inCables_.AsReadOnly(); } }
        
        public ReadOnlyCollection<Cable> Cables
        {
            get { return OutCables.Concat(InCables).ToList().AsReadOnly(); }
        }

        public void AddOutCable(Cable cable)
        {
            outCables_.Add(cable);
        }

        public void AddInCable(Cable cable)
        {
            inCables_.Add(cable);
        }

        public void AddIndicators(List<IndicatorInfo> indicators)
        {
            indicators_ = indicators;
            var indicatorsString = string.Join("; ", indicators_.Select(x => x.ToString()).ToArray());
            _connectedMonitorsLogger.LogMessage("Monitor: {0} | Number: {1} | Color: {2} | Indicators: {4} | Display color: {3}",
                Index + 1,
                DisplayValue.ToString(),
                MonitorColor.ToString(),
                DisplayColor.ToString(),
                indicatorsString.Length > 0 ? indicatorsString : "None");
                
        }

        public void AddScore(string message, int score)
        {
            _connectedMonitorsLogger.LogMessage("Monitor: {0}, Section: {1}, Score: {2}", Index + 1, message, score);
            score_ += score;
        }

        public void DebugTotalScore()
        {
            _connectedMonitorsLogger.LogMessage("Monitor: {0}, Total score: {1}", Index + 1, Score);
        }

        public void DebugMonitorIndex()
        {
            _connectedMonitorsLogger.LogMessage("Monitor: {0}:", Index + 1);
        }

        public MonitorType GetMonitorType()
        {
            switch (MonitorColor)
            {
                case Color.Red:
                    return ToMonitorType(Score, MonitorType.RedEven, MonitorType.RedOdd);
                case Color.Blue:
                    return ToMonitorType(Score, MonitorType.BlueEven, MonitorType.BlueOdd);
                case Color.Green:
                    return ToMonitorType(Score, MonitorType.GreenEven, MonitorType.GreenOdd);
                case Color.Orange:
                    return ToMonitorType(Score, MonitorType.OrangeEven, MonitorType.OrangeOdd);
                case Color.Purple:
                    return ToMonitorType(Score, MonitorType.PurpleEven, MonitorType.PurpleOdd);
                default:
                    throw new InvalidOperationException(string.Format("Invalid color: {0}", MonitorColor.ToString()));
            }
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("Monitor: [");
            builder.AppendFormat("Index = {0}; ", Index);
            builder.AppendFormat("DisplayValue = {0}; ", DisplayValue);
            builder.AppendFormat("Color = {0}; ", MonitorColor.ToString());
            builder.AppendFormat("DisplayColor = {0}; ", DisplayColor.ToString());
            builder.AppendFormat("Score = {0}; ", Score.ToString());
            builder.AppendFormat("Indicators = [{0}]; ", string.Join("; ", indicators_.Select(x => x.ToString()).ToArray()));
            builder.AppendFormat("Outcables = [{0}]", string.Join("; ", outCables_.Select(x => x.ToString()).ToArray()));
            builder.AppendFormat("Incables = [{0}]", string.Join("; ", inCables_.Select(x => x.ToString()).ToArray()));
            builder.Append("]");

            return builder.ToString();
        }
        
        private static MonitorType ToMonitorType(int value, MonitorType even, MonitorType odd)
        {
            return value % 2 == 0 ? even : odd;
        }
        
        //Testing:
        internal bool EnsurePositive { get; set; }

        internal bool EnsureNegative { get; set; }
    }
}