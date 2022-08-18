using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brae
{
    static class Calculations
    {
        public static double GetActionPrice(double emaShort, double emaLong, double shortPeriods, double longPeriods)
        {
            double macdToday = emaShort - emaLong;
            double actionPrice = (macdToday - emaShort + ((2d * emaShort) / (shortPeriods + 1d)) + emaLong - ((2d * emaLong) / (longPeriods + 1d))) / ((2d / (shortPeriods + 1d)) - (2d / (longPeriods + 1d)));

            // Used to verify accuracy of calculations
            //double[] current = { actionPrice };
            //double testemashort = GetEMA(shortPeriods, current, emaShort);
            //double testemalong = GetEMA(longPeriods, current, emaLong);
            //double testmacd = testemashort - testemalong;

            return actionPrice;
        }

        public static double GetEMA(double periods, double[] dailyPrices, double prevEMA)
        {
            double tempEMA = dailyPrices[0] * (2d / (periods + 1d)) + prevEMA * (1d - (2d / (periods + 1d)));
            if (dailyPrices.Length != 1)
            {
                return GetEMA(periods, dailyPrices.Where((value, index) => index != 0).ToArray(), tempEMA);
            }
            else
            {
                return tempEMA;
            }
        }

        public static double GetSMA(double[] prices, double periods)
        {
            double pricesSum = 0;

            for (int i = 0; i < periods; i++)
            {
                pricesSum += prices[i];
            }

            return pricesSum / periods;
        }
    }
}
