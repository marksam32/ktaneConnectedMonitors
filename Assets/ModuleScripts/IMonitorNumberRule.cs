using System;
using System.Collections.Generic;

namespace ConnectedMonitors
{
    public interface IMonitorNumberRule
    {
        bool IsValid(int number);
        int Score { get; }
    }

    public static class RuleFactory
    {
        public static readonly IList<IMonitorNumberRule> Rules = new List<IMonitorNumberRule>
        {
            new DivisibleBySevenRule(),
            new GreaterThanTenRule(),
            new LastDigitZeroRule(),
            new LastDigitSevenRule(),
            new DivisibleByNineRule(),
            new FirstDigitFiveRule(),
            new LessThanZeroRule(),
            new IsPrimeRule(),
            new LastDigitFourRule(),
            new FirstDigitTwoRule(),
            new AIsPrimeRule(),
            new LastDigitSixRule(),
            new DivisibleBySixRule(),
            new LastDigitEightRule(),
            new FirstDigitThreeRule(),
            new APlusOneIsBRule(),
            new DivisibleByTenRule(),
            new BIsPrimeRule()
        };
    }

    internal class DivisibleBySevenRule : IMonitorNumberRule
    {
        public int Score { get { return 3; } }

        public bool IsValid(int number)
        {
            return number % 7 == 0;
        }
    }
    internal class GreaterThanTenRule : IMonitorNumberRule
    {
        public int Score { get { return 3; } }

        public bool IsValid(int number)
        {
            var parts = NumberHelper.Split(number);

            return parts.Item1 + parts.Item2 > 10;
        }
    }

    internal class LastDigitZeroRule : IMonitorNumberRule
    {
        public int Score { get { return 3; } }

        public bool IsValid(int number)
        {
            return number % 10 == 0;
        }
    }

    internal class LastDigitSevenRule : IMonitorNumberRule
    {
        public int Score { get { return 2; } }

        public bool IsValid(int number)
        {
            return number % 10 == 7;
        }
    }
    
    internal class DivisibleByNineRule : IMonitorNumberRule
    {
        public int Score { get { return 2; } }

        public bool IsValid(int number)
        {
            return number % 9 == 0;
        }
    }

    internal class FirstDigitFiveRule : IMonitorNumberRule
    {
        public int Score { get { return 2; } }

        public bool IsValid(int number)
        {
            return NumberHelper.Split(number).Item1 == 5;
        }
    }

    internal class LessThanZeroRule : IMonitorNumberRule
    {
        public int Score { get { return 1; } }

        public bool IsValid(int number)
        {
            var parts = NumberHelper.Split(number);

            return parts.Item2 - parts.Item1 < 0;
        }
    }

    internal class IsPrimeRule : IMonitorNumberRule
    {
        public int Score { get { return 1; } }

        public bool IsValid(int number)
        {
            return NumberHelper.IsPrime(number);
        }
    }

    internal class LastDigitFourRule : IMonitorNumberRule
    {
        public int Score { get { return 1; } }

        public bool IsValid(int number)
        {
            return number % 10 == 4;
        }
    }

    internal class FirstDigitTwoRule : IMonitorNumberRule
    {
        public int Score { get { return -1; } }

        public bool IsValid(int number)
        {
            return NumberHelper.Split(number).Item1 == 2;
        }
    }

    internal class AIsPrimeRule : IMonitorNumberRule
    {
        public int Score { get { return -1; } }

        public bool IsValid(int number)
        {
            return NumberHelper.IsPrime(NumberHelper.Split(number).Item1);
        }
    }

    internal class LastDigitSixRule : IMonitorNumberRule
    {
        public int Score { get { return -1; } }

        public bool IsValid(int number)
        {
            return number % 10 == 6;
        }
    }

    internal class DivisibleBySixRule : IMonitorNumberRule
    {
        public int Score { get { return -2; } }

        public bool IsValid(int number)
        {
            return number % 6 == 0;
        }
    }

    internal class LastDigitEightRule : IMonitorNumberRule
    {
        public int Score { get { return -2; } }

        public bool IsValid(int number)
        {
            return number % 10 == 8;
        }
    }

    internal class FirstDigitThreeRule : IMonitorNumberRule
    {
        public int Score { get { return -2; } }

        public bool IsValid(int number)
        {
            return NumberHelper.Split(number).Item1 == 3;
        }
    }

    internal class APlusOneIsBRule : IMonitorNumberRule
    {
        public int Score { get { return -3; } }

        public bool IsValid(int number)
        {
            var parts = NumberHelper.Split(number);
            return parts.Item1 + 1 == parts.Item2;
        }
    }

    internal class DivisibleByTenRule : IMonitorNumberRule
    {
        public int Score { get { return -3; } }

        public bool IsValid(int number)
        {
            return number % 10 == 0;
        }
    }

    internal class BIsPrimeRule : IMonitorNumberRule
    {
        public int Score { get { return -3; } }

        public bool IsValid(int number)
        {
            return NumberHelper.IsPrime(NumberHelper.Split(number).Item2);
        }
    }


    internal static class NumberHelper
    {
        public static Pair<int, int> Split(int number)
        {
            if(number < 10)
            {
                return new Pair<int, int>(0, number);
            }

            return new Pair<int, int>((int)Math.Floor(number / 10.0), number % 10);
        }

        public static bool IsPrime(int number)
        {
            if (number <= 1) 
            { 
                return false; 
            }
            if (number == 2)
            {
                return true;
            }
            if (number % 2 == 0)
            {
                return false;
            }

            var boundary = (int)Math.Floor(Math.Sqrt(number));

            for (int i = 3; i <= boundary; i += 2)
            {
                if (number % i == 0)
                {
                    return false;
                }              
            }              
            return true;
        }
    }
}

