using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace ConnectedMonitors
{
    public static class IndicatorScore
    {
        private static readonly Dictionary<Color, List<Pair<int, int>>> ColorOfIndicatorLightValues = new Dictionary<Color, List<Pair<int, int>>>
        {
            {Color.Red, new List<Pair<int, int>>{new Pair<int, int>(1, 2), new Pair<int, int>(-3, -1), new Pair<int, int>(3, 3)} },
            {Color.Orange, new List<Pair<int, int>>{new Pair<int, int>(-2, -3), new Pair<int, int>(2, -2), new Pair<int, int>(2, 1)} },
            {Color.Green, new List<Pair<int, int>>{new Pair<int, int>(-3, -1), new Pair<int, int>(1, 3), new Pair<int, int>(2, 3)} },
            {Color.Blue, new List<Pair<int, int>>{new Pair<int, int>(3, 2), new Pair<int, int>(-1, 2), new Pair<int, int>(-1, 1)} },
            {Color.Purple, new List<Pair<int, int>>{new Pair<int, int>(1, -2), new Pair<int, int>(-2, 1), new Pair<int, int>(-3, -2)} },
            {Color.White, new List<Pair<int, int>>{new Pair<int, int>(-1, -3), new Pair<int, int>(2, -1), new Pair<int, int>(3, 3)} }
        };

        private static readonly Dictionary<Color, List<int>> PositionOfIndicatorLightValues = new Dictionary<Color, List<int>>
        {
            {Color.Red, new List<int>{2, -6, -2 }  },
            {Color.Orange, new List<int>{4, -2, 6} },
            {Color.Green, new List<int>{-6, 6, -6} },
            {Color.Blue, new List<int>{-4, -4, 4} },
            {Color.Purple, new List<int>{6, -2, 2} },
            {Color.White, new List<int>{4, 2, -4} }
        };

        public static int GetIndicatorScore(ReadOnlyCollection<IndicatorInfo> indicators)
        {
            Dictionary<Color, int> ColorCount = new Dictionary<Color, int>();
            return indicators.Select(s => GetScoreFromPosition(s) + GetScoreFromColor(s, ColorCount)).Sum();
        } 

        internal static int GetScoreFromPosition(IndicatorInfo indicator)
        {
            return indicator.IsFlashing ? PositionOfIndicatorLightValues[indicator.Color][indicator.Index] : ColorOfIndicatorLightValues[indicator.Color][indicator.Index].Item1;
        }

        internal static int GetScoreFromColor(IndicatorInfo indicator, Dictionary<Color,int> colorCountDictionary)
        {
            int colorCount;

            if (colorCountDictionary.TryGetValue(indicator.Color, out colorCount))
            {
                colorCount++;
                colorCountDictionary[indicator.Color]++;
            }
            else
            {
                colorCount = 1;
                colorCountDictionary.Add(indicator.Color, colorCount);
            }

            return ColorOfIndicatorLightValues[indicator.Color][colorCount - 1].Item2;
        }
    }
}