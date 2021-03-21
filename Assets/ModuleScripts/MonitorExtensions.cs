using System;
using System.Linq;

namespace ConnectedMonitors
{
    public static class MonitorExtensions
    {
        public static void CalculateScore(this Monitor monitor)
        {
            monitor.Score = 0;
            using (new MonitorLoggerSetup(monitor))
            {
                monitor.AddScore("1", GetMonitorColorScore(monitor.MonitorColor));
                monitor.AddScore("2", GetDisplayScore(monitor.DisplayValue));
                monitor.AddScore("3", GetIndicatorAmountScore(monitor));
                monitor.AddScore("4", IndicatorScore.GetIndicatorScore(monitor.Indicators));
                monitor.AddScore("5", CableScore.GetCableScore(monitor));
            }

            if (monitor.EnsurePositive)
            {
                var score = monitor.Score;
                monitor.Score = score == 0 ? 1 : (score < 0 ? -score : score);
                return;
            }

            if (monitor.EnsureNegative)
            {
                var score = monitor.Score;
                monitor.Score = score == 0 ? -1 : (score > 0 ? -score : score);
            }
        }

        internal static int GetDisplayScore(int number)
        {
            return RuleFactory.Rules.Where(x => x.IsValid(number)).Sum(x => x.Score);
        }

        private static int GetMonitorColorScore(Color color)
        {
            switch (color)
            {
                case Color.Red:
                    return 2;
                case Color.Orange:
                    return 1;
                case Color.Green:
                    return 0;
                case Color.Blue:
                    return -1;
                case Color.Purple:
                    return -2;
                default:
                    throw new ArgumentException("Unknown color", color.ToString());
            }
        }

        internal static int GetIndicatorAmountScore(Monitor monitor)
        {
            return (monitor.DisplayValue % 2 == 0) ? monitor.IndicatorCount : -monitor.IndicatorCount;
        }
    }
}

