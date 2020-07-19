using System;
using System.Collections.Generic;
using System.Linq;

namespace ConnectedMonitors
{
    public class CableScore
    {
        private static readonly IDictionary<CableDirection, IDictionary<Color, int>> CableScores = new Dictionary<CableDirection, IDictionary<Color, int>>
        {
            {CableDirection.NW, Make(new [] {-3, 2, -2, 1, -1, 3}) },
            {CableDirection.N, Make(new [] {-3, -1, 2, -2, 3, 1}) },
            {CableDirection.NE, Make(new [] {3, -3, -1, 1, 2, -2}) },
            {CableDirection.E, Make(new [] {-1, -2, 1, 3, 2, -3}) },
            {CableDirection.SE, Make(new [] {-3, -2, 1, -1, 3, 2}) },
            {CableDirection.S, Make(new [] {3, -2, 1, 2, -1, -3}) },
            {CableDirection.SW, Make(new [] {3, 1, -2, 2, -3, -1}) },
            {CableDirection.W, Make(new [] {3, 2, -1, -3, 1, -2}) }

        };

        private static int GetScoreValue(CableDirection direction, Color color, int multiplier)
        {
            return CableScores[direction][color] * multiplier;
        }

        public static int GetCableScore(Monitor monitor)
        {
            var multipliers = GetMultipliers(monitor.InCables.Concat(monitor.OutCables));
            var inValue = monitor.InCables.Sum(x => GetScoreValue(DirectionHelper.Opposite(x.Direction), x.Color, multipliers[x.Color]));
            var outValue = monitor.OutCables.Sum(x => GetScoreValue(x.Direction, x.Color, multipliers[x.Color]));

            return inValue + outValue;
        } 

        internal static Dictionary<Color, int> GetMultipliers(IEnumerable<Cable> cables)
        {
            var returnValues = new Dictionary<Color, int>();
            var enumValues = Enum.GetValues(typeof(Color)).Cast<Color>().ToList();

            foreach (var item in enumValues)
            {
                returnValues.Add(item, Math.Max(0, cables.Count(x => x.Color == item)));
            }

            return returnValues;
        }

        private static IDictionary<Color, int> Make(IList<int> values)
        {
            var returnValues = new Dictionary<Color, int>();
            var enumValues = Enum.GetValues(typeof(Color)).Cast<Color>().ToList();

            for (int i = 0; i < enumValues.Count(); i++)
            {
                returnValues.Add(enumValues[i], values[i]);
            }

            return returnValues;
        }
    }
}