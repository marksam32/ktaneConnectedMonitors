using System;

namespace ConnectedMonitors
{
    public enum CableDirection

    {
        N,
        NE,
        E,
        SE,
        S,
        SW,
        W,
        NW
    }

    public static class DirectionHelper
    {
        public static CableDirection Opposite(CableDirection direction)
        {
            switch (direction)
            {
                case CableDirection.N:
                    return CableDirection.S;
                case CableDirection.NE:
                    return CableDirection.SW;
                case CableDirection.E:
                    return CableDirection.W;
                case CableDirection.SE:
                    return CableDirection.NW;
                case CableDirection.S:
                    return CableDirection.N;
                case CableDirection.SW:
                    return CableDirection.NE;
                case CableDirection.W:
                    return CableDirection.E;
                case CableDirection.NW:
                    return CableDirection.SE;
                default:
                    throw new ArgumentException("Direction unknown", direction.ToString());
                    
            }
        }
    }
}