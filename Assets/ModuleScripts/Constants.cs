using System.Collections.Generic;

namespace ConnectedMonitors
{
    public static class Constants
    {
        public static readonly Dictionary<Color, List<MonitorType>> PressingOrder = new Dictionary<Color, List<MonitorType>>
        {
            {
                Color.Red,
                new List<MonitorType>{ MonitorType.PurpleOdd, MonitorType.GreenOdd, MonitorType.OrangeEven, MonitorType.BlueEven,
                    MonitorType.PurpleEven, MonitorType.GreenEven, MonitorType.RedOdd, MonitorType.RedEven, MonitorType.OrangeOdd, MonitorType.BlueOdd}
            },
            {
                Color.Orange,
                new List<MonitorType>{ MonitorType.PurpleEven, MonitorType.GreenEven, MonitorType.RedEven, MonitorType.OrangeEven, 
                    MonitorType.GreenOdd, MonitorType.PurpleOdd, MonitorType.OrangeOdd, MonitorType.BlueOdd, MonitorType.BlueEven, MonitorType.RedOdd}
            },
            {
                Color.Green,
                new List<MonitorType>{ MonitorType.BlueOdd, MonitorType.GreenEven, MonitorType.RedEven, MonitorType.RedOdd, MonitorType.BlueEven, 
                    MonitorType.OrangeEven, MonitorType.OrangeOdd, MonitorType.PurpleOdd, MonitorType.GreenOdd, MonitorType.PurpleEven}
            },
            {
                Color.Blue,
                new List<MonitorType>{ MonitorType.RedEven, MonitorType.RedOdd, MonitorType.PurpleOdd, MonitorType.OrangeEven, MonitorType.OrangeOdd, 
                    MonitorType.GreenOdd, MonitorType.BlueEven, MonitorType.PurpleEven, MonitorType.GreenEven, MonitorType.BlueOdd}
            },
            {
                Color.Purple,
                new List<MonitorType>{ MonitorType.PurpleEven, MonitorType.GreenOdd, MonitorType.PurpleOdd, MonitorType.BlueEven, MonitorType.GreenEven, 
                    MonitorType.RedEven, MonitorType.RedOdd, MonitorType.BlueOdd, MonitorType.OrangeOdd, MonitorType.OrangeEven}
            }
        };
        
        public static readonly Dictionary<int, List<CableInfo>> CableStructures = new Dictionary<int, List<CableInfo>>
        {
            {0, new List<CableInfo>{Create(1, CableDirection.E), Create(3, CableDirection.S), Create(4, CableDirection.SE)} },
            {1, new List<CableInfo>{Create(2, CableDirection.E), Create(3, CableDirection.SW), Create(4, CableDirection.S), Create(5, CableDirection.SE)} },
            {2, new List<CableInfo>{Create(4, CableDirection.SW), Create(5, CableDirection.S), Create(6, CableDirection.SE)} },
            {3, new List<CableInfo>{Create(4, CableDirection.E), Create(7, CableDirection.S), Create(8, CableDirection.SE)} },
            {4, new List<CableInfo>{Create(5, CableDirection.E), Create(7, CableDirection.SW), Create(8, CableDirection.S), Create(9, CableDirection.SE)} },
            {5, new List<CableInfo>{Create(6, CableDirection.E), Create(8, CableDirection.SW), Create(9, CableDirection.S), Create(10, CableDirection.SE)} },
            {6, new List<CableInfo>{Create(9, CableDirection.SW), Create(10, CableDirection.S)} },
            {7, new List<CableInfo>{Create(8, CableDirection.E), Create(11, CableDirection.S), Create(12, CableDirection.SE)} },
            {8, new List<CableInfo>{Create(9, CableDirection.E), Create(11, CableDirection.SW), Create(12, CableDirection.S), Create(13, CableDirection.SE)} },
            {9, new List<CableInfo>{Create(10, CableDirection.E), Create(12, CableDirection.SW), Create(13, CableDirection.S), Create(14, CableDirection.SE)} },
            {10, new List<CableInfo>{Create(13, CableDirection.SW), Create(14, CableDirection.S)} },
            {11, new List<CableInfo>{Create(12, CableDirection.E)} },
            {12, new List<CableInfo>{Create(13, CableDirection.E)} },
            {13, new List<CableInfo>{Create(14, CableDirection.E)} },
            {14, new List<CableInfo>()}
        };
        
        private static CableInfo Create(int index, CableDirection direction)
        {
            return new CableInfo(index, direction);
        }
    }
}


