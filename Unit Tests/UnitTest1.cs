using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Brae;
using System.Collections.Generic;

namespace Unit_Tests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void DetermineAction_PriceIsEqualToMAMovingUp()
        {
            string response = "[{\"a\":2236279,\"p\":\"1248.3100\"," +
                "\"q\":\"0.50000000\",\"f\":2347107,\"l\":2347107,\"T\":1611796961861,\"m\":false,\"M\":true}," +

                "{\"a\":2236280,\"p\":\"1248.9600\"," +
                "\"q\":\"4.07078000\",\"f\":2347108,\"l\":2347108,\"T\":1611796968385,\"m\":true,\"M\":true}," +

                "{\"a\":2236281,\"p\":\"1248.9500\"," +
                "\"q\":\"1.60087000\",\"f\":2347109,\"l\":2347109,\"T\":1611796968385,\"m\":true,\"M\":true}," +

                "{\"a\":2236282,\"p\":\"1248.9300\"," +
                "\"q\":\"1.15236000\",\"f\":2347110,\"l\":2347110,\"T\":1611796968385,\"m\":true,\"M\":true}," +

                "{\"a\":2236283,\"p\":\"1248.4900\"," +
                "\"q\":\"0.07099000\",\"f\":2347111,\"l\":2347111,\"T\":1611796968385,\"m\":true,\"M\":true}," +

                "{\"a\":2236284,\"p\":\"1248.3600\"," +
                "\"q\":\"0.10500000\",\"f\":2347112,\"l\":2347112,\"T\":1611796968385,\"m\":true,\"M\":true}," +

                "{\"a\":2236285,\"p\":\"1249.1300\"," +
                "\"q\":\"0.40000000\",\"f\":2347113,\"l\":2347113,\"T\":1611796981802,\"m\":true,\"M\":true}," +

                "{\"a\":2236286,\"p\":\"1250.0000\"," +
                "\"q\":\"0.50357000\",\"f\":2347114,\"l\":2347114,\"T\":1611796983704,\"m\":false,\"M\":true}," +

                "{\"a\":2236287,\"p\":\"1250.0000\"," +
                "\"q\":\"0.29959000\",\"f\":2347115,\"l\":2347115,\"T\":1611796983887,\"m\":false,\"M\":true}," +

                "{\"a\":2236288,\"p\":\"1250.0000\"," +
                "\"q\":\"0.50225000\",\"f\":2347116,\"l\":2347116,\"T\":1611796984076,\"m\":false,\"M\":true}," +

                "{\"a\":2236289,\"p\":\"1250.0000\"," +
                "\"q\":\"0.51076000\",\"f\":2347117,\"l\":2347117,\"T\":1611796984263,\"m\":false,\"M\":true}," +

                "{\"a\":2236290,\"p\":\"1250.0000\"," +
                "\"q\":\"0.32700000\",\"f\":2347118,\"l\":2347118,\"T\":1611796985066,\"m\":false,\"M\":true}," +

                "{\"a\":2236291,\"p\":\"1250.0000\"," +
                "\"q\":\"0.41613000\",\"f\":2347119,\"l\":2347120,\"T\":1611797031646,\"m\":false,\"M\":true}," +

                "{\"a\":2236292,\"p\":\"1250.0000\"," +
                "\"q\":\"0.00802000\",\"f\":2347121,\"l\":2347121,\"T\":1611797041448,\"m\":true,\"M\":true}]";
            
            double movingAverage = 1250;
            Program.TradeActions action = Program.DetermineAction(new Program.Symbol("ETHUSD"), response, movingAverage);

            Assert.IsTrue(action.Equals(Program.TradeActions.BUY));
        }

        [TestMethod]
        public void DetermineAction_PriceIsBelowMAMovingUp()
        {
            string response = "[{\"a\":2236279,\"p\":\"1248.3100\"," +
                "\"q\":\"0.50000000\",\"f\":2347107,\"l\":2347107,\"T\":1611796961861,\"m\":false,\"M\":true}," +

                "{\"a\":2236280,\"p\":\"1248.9600\"," +
                "\"q\":\"4.07078000\",\"f\":2347108,\"l\":2347108,\"T\":1611796968385,\"m\":true,\"M\":true}," +

                "{\"a\":2236281,\"p\":\"1248.9500\"," +
                "\"q\":\"1.60087000\",\"f\":2347109,\"l\":2347109,\"T\":1611796968385,\"m\":true,\"M\":true}," +

                "{\"a\":2236282,\"p\":\"1248.9300\"," +
                "\"q\":\"1.15236000\",\"f\":2347110,\"l\":2347110,\"T\":1611796968385,\"m\":true,\"M\":true}," +

                "{\"a\":2236283,\"p\":\"1248.4900\"," +
                "\"q\":\"0.07099000\",\"f\":2347111,\"l\":2347111,\"T\":1611796968385,\"m\":true,\"M\":true}," +

                "{\"a\":2236284,\"p\":\"1248.3600\"," +
                "\"q\":\"0.10500000\",\"f\":2347112,\"l\":2347112,\"T\":1611796968385,\"m\":true,\"M\":true}," +

                "{\"a\":2236285,\"p\":\"1249.1300\"," +
                "\"q\":\"0.40000000\",\"f\":2347113,\"l\":2347113,\"T\":1611796981802,\"m\":true,\"M\":true}]";

            double movingAverage = 1250;
            Program.TradeActions action = Program.DetermineAction(new Program.Symbol("ETHUSD"), response, movingAverage);

            Assert.IsTrue(action.Equals(Program.TradeActions.DONOTHING));
        }

        [TestMethod]
        public void DetermineAction_PriceIsAboveMAMovingUp()
        {
            string response = "[{\"a\":2236279,\"p\":\"1248.3100\"," +
                "\"q\":\"0.50000000\",\"f\":2347107,\"l\":2347107,\"T\":1611796961861,\"m\":false,\"M\":true}," +

                "{\"a\":2236280,\"p\":\"1248.9600\"," +
                "\"q\":\"4.07078000\",\"f\":2347108,\"l\":2347108,\"T\":1611796968385,\"m\":true,\"M\":true}," +

                "{\"a\":2236281,\"p\":\"1248.9500\"," +
                "\"q\":\"1.60087000\",\"f\":2347109,\"l\":2347109,\"T\":1611796968385,\"m\":true,\"M\":true}," +

                "{\"a\":2236282,\"p\":\"1248.9300\"," +
                "\"q\":\"1.15236000\",\"f\":2347110,\"l\":2347110,\"T\":1611796968385,\"m\":true,\"M\":true}," +

                "{\"a\":2236283,\"p\":\"1248.4900\"," +
                "\"q\":\"0.07099000\",\"f\":2347111,\"l\":2347111,\"T\":1611796968385,\"m\":true,\"M\":true}," +

                "{\"a\":2236284,\"p\":\"1248.3600\"," +
                "\"q\":\"0.10500000\",\"f\":2347112,\"l\":2347112,\"T\":1611796968385,\"m\":true,\"M\":true}," +

                "{\"a\":2236285,\"p\":\"1250.1300\"," +
                "\"q\":\"0.40000000\",\"f\":2347113,\"l\":2347113,\"T\":1611796981802,\"m\":true,\"M\":true}]";

            double movingAverage = 1250;
            Program.TradeActions action = Program.DetermineAction(new Program.Symbol("ETHUSD"), response, movingAverage);

            Assert.IsTrue(action.Equals(Program.TradeActions.BUY));
        }

        [TestMethod]
        public void DetermineAction_PriceGoesAboveThenBelowMA()
        {
            string response = "[{\"a\":2236279,\"p\":\"1248.3100\"," +
                "\"q\":\"0.50000000\",\"f\":2347107,\"l\":2347107,\"T\":1611796961861,\"m\":false,\"M\":true}," +

                "{\"a\":2236280,\"p\":\"1248.9600\"," +
                "\"q\":\"4.07078000\",\"f\":2347108,\"l\":2347108,\"T\":1611796968385,\"m\":true,\"M\":true}," +

                "{\"a\":2236281,\"p\":\"1248.9500\"," +
                "\"q\":\"1.60087000\",\"f\":2347109,\"l\":2347109,\"T\":1611796968385,\"m\":true,\"M\":true}," +

                "{\"a\":2236282,\"p\":\"1248.9300\"," +
                "\"q\":\"1.15236000\",\"f\":2347110,\"l\":2347110,\"T\":1611796968385,\"m\":true,\"M\":true}," +

                "{\"a\":2236283,\"p\":\"1248.4900\"," +
                "\"q\":\"0.07099000\",\"f\":2347111,\"l\":2347111,\"T\":1611796968385,\"m\":true,\"M\":true}," +

                "{\"a\":2236284,\"p\":\"1248.3600\"," +
                "\"q\":\"0.10500000\",\"f\":2347112,\"l\":2347112,\"T\":1611796968385,\"m\":true,\"M\":true}," +

                "{\"a\":2236285,\"p\":\"1250.1300\"," +
                "\"q\":\"0.40000000\",\"f\":2347113,\"l\":2347113,\"T\":1611796981802,\"m\":true,\"M\":true}," +

                "{\"a\":2236285,\"p\":\"1249.1300\"," +
                "\"q\":\"0.40000000\",\"f\":2347113,\"l\":2347113,\"T\":1611796981802,\"m\":true,\"M\":true}]";

            double movingAverage = 1250;
            Program.TradeActions action = Program.DetermineAction(new Program.Symbol("ETHUSD"), response, movingAverage);

            Assert.IsTrue(action.Equals(Program.TradeActions.DONOTHING));
        }

        [TestMethod]
        public void DetermineAction_PriceGoesAboveThenBelowThenAboveMA()
        {
            string response = "[{\"a\":2236279,\"p\":\"1248.3100\"," +
                "\"q\":\"0.50000000\",\"f\":2347107,\"l\":2347107,\"T\":1611796961861,\"m\":false,\"M\":true}," +

                "{\"a\":2236280,\"p\":\"1248.9600\"," +
                "\"q\":\"4.07078000\",\"f\":2347108,\"l\":2347108,\"T\":1611796968385,\"m\":true,\"M\":true}," +

                "{\"a\":2236281,\"p\":\"1248.9500\"," +
                "\"q\":\"1.60087000\",\"f\":2347109,\"l\":2347109,\"T\":1611796968385,\"m\":true,\"M\":true}," +

                "{\"a\":2236282,\"p\":\"1248.9300\"," +
                "\"q\":\"1.15236000\",\"f\":2347110,\"l\":2347110,\"T\":1611796968385,\"m\":true,\"M\":true}," +

                "{\"a\":2236283,\"p\":\"1248.4900\"," +
                "\"q\":\"0.07099000\",\"f\":2347111,\"l\":2347111,\"T\":1611796968385,\"m\":true,\"M\":true}," +

                "{\"a\":2236284,\"p\":\"1248.3600\"," +
                "\"q\":\"0.10500000\",\"f\":2347112,\"l\":2347112,\"T\":1611796968385,\"m\":true,\"M\":true}," +

                "{\"a\":2236285,\"p\":\"1250.1300\"," +
                "\"q\":\"0.40000000\",\"f\":2347113,\"l\":2347113,\"T\":1611796981802,\"m\":true,\"M\":true}," +

                "{\"a\":2236291,\"p\":\"1249.9999\"," +
                "\"q\":\"0.41613000\",\"f\":2347119,\"l\":2347120,\"T\":1611797031646,\"m\":false,\"M\":true}," +

                "{\"a\":2236285,\"p\":\"1250.1300\"," +
                "\"q\":\"0.40000000\",\"f\":2347113,\"l\":2347113,\"T\":1611796981802,\"m\":true,\"M\":true}]";

            double movingAverage = 1250;
            Program.TradeActions action = Program.DetermineAction(new Program.Symbol("ETHUSD"), response, movingAverage);

            Assert.IsTrue(action.Equals(Program.TradeActions.BUY));
        }

        [TestMethod]
        public void DetermineAction_PriceGoesAboveThenBelowThenAboveThenBelowMA()
        {
            string response = "[{\"a\":2236279,\"p\":\"1248.3100\"," +
                "\"q\":\"0.50000000\",\"f\":2347107,\"l\":2347107,\"T\":1611796961861,\"m\":false,\"M\":true}," +

                "{\"a\":2236280,\"p\":\"1248.9600\"," +
                "\"q\":\"4.07078000\",\"f\":2347108,\"l\":2347108,\"T\":1611796968385,\"m\":true,\"M\":true}," +

                "{\"a\":2236281,\"p\":\"1248.9500\"," +
                "\"q\":\"1.60087000\",\"f\":2347109,\"l\":2347109,\"T\":1611796968385,\"m\":true,\"M\":true}," +

                "{\"a\":2236282,\"p\":\"1248.9300\"," +
                "\"q\":\"1.15236000\",\"f\":2347110,\"l\":2347110,\"T\":1611796968385,\"m\":true,\"M\":true}," +

                "{\"a\":2236283,\"p\":\"1248.4900\"," +
                "\"q\":\"0.07099000\",\"f\":2347111,\"l\":2347111,\"T\":1611796968385,\"m\":true,\"M\":true}," +

                "{\"a\":2236284,\"p\":\"1251.3600\"," +
                "\"q\":\"0.10500000\",\"f\":2347112,\"l\":2347112,\"T\":1611796968385,\"m\":true,\"M\":true}," +

                "{\"a\":2236285,\"p\":\"1249.1300\"," +
                "\"q\":\"0.40000000\",\"f\":2347113,\"l\":2347113,\"T\":1611796981802,\"m\":true,\"M\":true}," +

                "{\"a\":2236291,\"p\":\"1250.9999\"," +
                "\"q\":\"0.41613000\",\"f\":2347119,\"l\":2347120,\"T\":1611797031646,\"m\":false,\"M\":true}," +

                "{\"a\":2236285,\"p\":\"1249.1300\"," +
                "\"q\":\"0.40000000\",\"f\":2347113,\"l\":2347113,\"T\":1611796981802,\"m\":true,\"M\":true}]";

            double movingAverage = 1250;
            Program.TradeActions action = Program.DetermineAction(new Program.Symbol("ETHUSD"), response, movingAverage);

            Assert.IsTrue(action.Equals(Program.TradeActions.DONOTHING));
        }

        [TestMethod]
        public void DetermineAction_PriceGoesBelowMAMovingDown()
        {
            string response = "[{\"a\":2236279,\"p\":\"1248.3100\"," +
                "\"q\":\"0.50000000\",\"f\":2347107,\"l\":2347107,\"T\":1611796961861,\"m\":false,\"M\":true}," +

                "{\"a\":2236280,\"p\":\"1248.9600\"," +
                "\"q\":\"4.07078000\",\"f\":2347108,\"l\":2347108,\"T\":1611796968385,\"m\":true,\"M\":true}," +

                "{\"a\":2236281,\"p\":\"1248.9500\"," +
                "\"q\":\"1.60087000\",\"f\":2347109,\"l\":2347109,\"T\":1611796968385,\"m\":true,\"M\":true}," +

                "{\"a\":2236282,\"p\":\"1248.9300\"," +
                "\"q\":\"1.15236000\",\"f\":2347110,\"l\":2347110,\"T\":1611796968385,\"m\":true,\"M\":true}," +

                "{\"a\":2236283,\"p\":\"1248.4900\"," +
                "\"q\":\"0.07099000\",\"f\":2347111,\"l\":2347111,\"T\":1611796968385,\"m\":true,\"M\":true}," +

                "{\"a\":2236284,\"p\":\"1251.3600\"," +
                "\"q\":\"0.10500000\",\"f\":2347112,\"l\":2347112,\"T\":1611796968385,\"m\":true,\"M\":true}," +

                "{\"a\":2236285,\"p\":\"1249.1300\"," +
                "\"q\":\"0.40000000\",\"f\":2347113,\"l\":2347113,\"T\":1611796981802,\"m\":true,\"M\":true}," +

                "{\"a\":2236291,\"p\":\"1250.9999\"," +
                "\"q\":\"0.41613000\",\"f\":2347119,\"l\":2347120,\"T\":1611797031646,\"m\":false,\"M\":true}," +

                "{\"a\":2236285,\"p\":\"1246.1300\"," +
                "\"q\":\"0.40000000\",\"f\":2347113,\"l\":2347113,\"T\":1611796981802,\"m\":true,\"M\":true}]";

            double movingAverage = 1247;
            Program.TradeActions action = Program.DetermineAction(new Program.Symbol("ETHUSD"), response, movingAverage);

            Assert.IsTrue(action.Equals(Program.TradeActions.SELL));
        }

        [TestMethod]
        public void DetermineAction_PriceGoesBelowThenAboveMA()
        {
            string response = "[{\"a\":2236279,\"p\":\"1248.3100\"," +
                "\"q\":\"0.50000000\",\"f\":2347107,\"l\":2347107,\"T\":1611796961861,\"m\":false,\"M\":true}," +

                "{\"a\":2236280,\"p\":\"1248.9600\"," +
                "\"q\":\"4.07078000\",\"f\":2347108,\"l\":2347108,\"T\":1611796968385,\"m\":true,\"M\":true}," +

                "{\"a\":2236281,\"p\":\"1248.9500\"," +
                "\"q\":\"1.60087000\",\"f\":2347109,\"l\":2347109,\"T\":1611796968385,\"m\":true,\"M\":true}," +

                "{\"a\":2236282,\"p\":\"1248.9300\"," +
                "\"q\":\"1.15236000\",\"f\":2347110,\"l\":2347110,\"T\":1611796968385,\"m\":true,\"M\":true}," +

                "{\"a\":2236283,\"p\":\"1248.4900\"," +
                "\"q\":\"0.07099000\",\"f\":2347111,\"l\":2347111,\"T\":1611796968385,\"m\":true,\"M\":true}," +

                "{\"a\":2236284,\"p\":\"1251.3600\"," +
                "\"q\":\"0.10500000\",\"f\":2347112,\"l\":2347112,\"T\":1611796968385,\"m\":true,\"M\":true}," +

                "{\"a\":2236285,\"p\":\"1249.1300\"," +
                "\"q\":\"0.40000000\",\"f\":2347113,\"l\":2347113,\"T\":1611796981802,\"m\":true,\"M\":true}," +

                "{\"a\":2236291,\"p\":\"1246.1300\"," +
                "\"q\":\"0.41613000\",\"f\":2347119,\"l\":2347120,\"T\":1611797031646,\"m\":false,\"M\":true}," +

                "{\"a\":2236285,\"p\":\"1250.9999\"," +
                "\"q\":\"0.40000000\",\"f\":2347113,\"l\":2347113,\"T\":1611796981802,\"m\":true,\"M\":true}]";

            double movingAverage = 1247;
            Program.TradeActions action = Program.DetermineAction(new Program.Symbol("ETHUSD"), response, movingAverage);

            Assert.IsTrue(action.Equals(Program.TradeActions.DONOTHING));
        }

        [TestMethod]
        public void DetermineAction_PriceGoesBelowThenAboveThenBelowMA()
        {
            string response = "[{\"a\":2236279,\"p\":\"1248.3100\"," +
                "\"q\":\"0.50000000\",\"f\":2347107,\"l\":2347107,\"T\":1611796961861,\"m\":false,\"M\":true}," +

                "{\"a\":2236280,\"p\":\"1248.9600\"," +
                "\"q\":\"4.07078000\",\"f\":2347108,\"l\":2347108,\"T\":1611796968385,\"m\":true,\"M\":true}," +

                "{\"a\":2236281,\"p\":\"1248.9500\"," +
                "\"q\":\"1.60087000\",\"f\":2347109,\"l\":2347109,\"T\":1611796968385,\"m\":true,\"M\":true}," +

                "{\"a\":2236282,\"p\":\"1248.9300\"," +
                "\"q\":\"1.15236000\",\"f\":2347110,\"l\":2347110,\"T\":1611796968385,\"m\":true,\"M\":true}," +

                "{\"a\":2236283,\"p\":\"1248.4900\"," +
                "\"q\":\"0.07099000\",\"f\":2347111,\"l\":2347111,\"T\":1611796968385,\"m\":true,\"M\":true}," +

                "{\"a\":2236284,\"p\":\"1246.3600\"," +
                "\"q\":\"0.10500000\",\"f\":2347112,\"l\":2347112,\"T\":1611796968385,\"m\":true,\"M\":true}," +

                "{\"a\":2236285,\"p\":\"1249.1300\"," +
                "\"q\":\"0.40000000\",\"f\":2347113,\"l\":2347113,\"T\":1611796981802,\"m\":true,\"M\":true}," +

                "{\"a\":2236291,\"p\":\"1246.1300\"," +
                "\"q\":\"0.41613000\",\"f\":2347119,\"l\":2347120,\"T\":1611797031646,\"m\":false,\"M\":true}," +

                "{\"a\":2236285,\"p\":\"1246.9999\"," +
                "\"q\":\"0.40000000\",\"f\":2347113,\"l\":2347113,\"T\":1611796981802,\"m\":true,\"M\":true}]";

            double movingAverage = 1247;
            Program.TradeActions action = Program.DetermineAction(new Program.Symbol("ETHUSD"), response, movingAverage);

            Assert.IsTrue(action.Equals(Program.TradeActions.SELL));
        }

        [TestMethod]
        public void DetermineAction_PriceGoesBelowThenAboveThenBelowThenAboveMA()
        {
            string response = "[{\"a\":2236279,\"p\":\"1248.3100\"," +
                "\"q\":\"0.50000000\",\"f\":2347107,\"l\":2347107,\"T\":1611796961861,\"m\":false,\"M\":true}," +

                "{\"a\":2236280,\"p\":\"1248.9600\"," +
                "\"q\":\"4.07078000\",\"f\":2347108,\"l\":2347108,\"T\":1611796968385,\"m\":true,\"M\":true}," +

                "{\"a\":2236281,\"p\":\"1248.9500\"," +
                "\"q\":\"1.60087000\",\"f\":2347109,\"l\":2347109,\"T\":1611796968385,\"m\":true,\"M\":true}," +

                "{\"a\":2236282,\"p\":\"1248.9300\"," +
                "\"q\":\"1.15236000\",\"f\":2347110,\"l\":2347110,\"T\":1611796968385,\"m\":true,\"M\":true}," +

                "{\"a\":2236283,\"p\":\"1248.4900\"," +
                "\"q\":\"0.07099000\",\"f\":2347111,\"l\":2347111,\"T\":1611796968385,\"m\":true,\"M\":true}," +

                "{\"a\":2236284,\"p\":\"1246.3600\"," +
                "\"q\":\"0.10500000\",\"f\":2347112,\"l\":2347112,\"T\":1611796968385,\"m\":true,\"M\":true}," +

                "{\"a\":2236285,\"p\":\"1249.1300\"," +
                "\"q\":\"0.40000000\",\"f\":2347113,\"l\":2347113,\"T\":1611796981802,\"m\":true,\"M\":true}," +

                "{\"a\":2236291,\"p\":\"1246.1300\"," +
                "\"q\":\"0.41613000\",\"f\":2347119,\"l\":2347120,\"T\":1611797031646,\"m\":false,\"M\":true}," +

                "{\"a\":2236285,\"p\":\"1247.9999\"," +
                "\"q\":\"0.40000000\",\"f\":2347113,\"l\":2347113,\"T\":1611796981802,\"m\":true,\"M\":true}]";

            double movingAverage = 1247;
            Program.TradeActions action = Program.DetermineAction(new Program.Symbol("ETHUSD"), response, movingAverage);

            Assert.IsTrue(action.Equals(Program.TradeActions.DONOTHING));
        }

        [TestMethod]
        public void DetermineAction_PriceDoesntCrossMAFromAbove()
        {
            string response = "[{\"a\":2236279,\"p\":\"1248.3100\"," +
                "\"q\":\"0.50000000\",\"f\":2347107,\"l\":2347107,\"T\":1611796961861,\"m\":false,\"M\":true}," +

                "{\"a\":2236280,\"p\":\"1248.9600\"," +
                "\"q\":\"4.07078000\",\"f\":2347108,\"l\":2347108,\"T\":1611796968385,\"m\":true,\"M\":true}," +

                "{\"a\":2236281,\"p\":\"1248.9500\"," +
                "\"q\":\"1.60087000\",\"f\":2347109,\"l\":2347109,\"T\":1611796968385,\"m\":true,\"M\":true}," +

                "{\"a\":2236282,\"p\":\"1248.9300\"," +
                "\"q\":\"1.15236000\",\"f\":2347110,\"l\":2347110,\"T\":1611796968385,\"m\":true,\"M\":true}," +

                "{\"a\":2236283,\"p\":\"1248.4900\"," +
                "\"q\":\"0.07099000\",\"f\":2347111,\"l\":2347111,\"T\":1611796968385,\"m\":true,\"M\":true}," +

                "{\"a\":2236284,\"p\":\"1246.3600\"," +
                "\"q\":\"0.10500000\",\"f\":2347112,\"l\":2347112,\"T\":1611796968385,\"m\":true,\"M\":true}," +

                "{\"a\":2236285,\"p\":\"1249.1300\"," +
                "\"q\":\"0.40000000\",\"f\":2347113,\"l\":2347113,\"T\":1611796981802,\"m\":true,\"M\":true}," +

                "{\"a\":2236291,\"p\":\"1246.1300\"," +
                "\"q\":\"0.41613000\",\"f\":2347119,\"l\":2347120,\"T\":1611797031646,\"m\":false,\"M\":true}," +

                "{\"a\":2236285,\"p\":\"1247.9999\"," +
                "\"q\":\"0.40000000\",\"f\":2347113,\"l\":2347113,\"T\":1611796981802,\"m\":true,\"M\":true}]";

            double movingAverage = 1240;
            Program.TradeActions action = Program.DetermineAction(new Program.Symbol("ETHUSD"), response, movingAverage);

            Assert.IsTrue(action.Equals(Program.TradeActions.DONOTHING));
        }

        [TestMethod]
        public void DetermineAction_PriceDoesntCrossMAFromBelow()
        {
            string response = "[{\"a\":2236279,\"p\":\"1248.3100\"," +
                "\"q\":\"0.50000000\",\"f\":2347107,\"l\":2347107,\"T\":1611796961861,\"m\":false,\"M\":true}," +

                "{\"a\":2236280,\"p\":\"1248.9600\"," +
                "\"q\":\"4.07078000\",\"f\":2347108,\"l\":2347108,\"T\":1611796968385,\"m\":true,\"M\":true}," +

                "{\"a\":2236281,\"p\":\"1248.9500\"," +
                "\"q\":\"1.60087000\",\"f\":2347109,\"l\":2347109,\"T\":1611796968385,\"m\":true,\"M\":true}," +

                "{\"a\":2236282,\"p\":\"1248.9300\"," +
                "\"q\":\"1.15236000\",\"f\":2347110,\"l\":2347110,\"T\":1611796968385,\"m\":true,\"M\":true}," +

                "{\"a\":2236283,\"p\":\"1248.4900\"," +
                "\"q\":\"0.07099000\",\"f\":2347111,\"l\":2347111,\"T\":1611796968385,\"m\":true,\"M\":true}," +

                "{\"a\":2236284,\"p\":\"1246.3600\"," +
                "\"q\":\"0.10500000\",\"f\":2347112,\"l\":2347112,\"T\":1611796968385,\"m\":true,\"M\":true}," +

                "{\"a\":2236285,\"p\":\"1249.1300\"," +
                "\"q\":\"0.40000000\",\"f\":2347113,\"l\":2347113,\"T\":1611796981802,\"m\":true,\"M\":true}," +

                "{\"a\":2236291,\"p\":\"1246.1300\"," +
                "\"q\":\"0.41613000\",\"f\":2347119,\"l\":2347120,\"T\":1611797031646,\"m\":false,\"M\":true}," +

                "{\"a\":2236285,\"p\":\"1247.9999\"," +
                "\"q\":\"0.40000000\",\"f\":2347113,\"l\":2347113,\"T\":1611796981802,\"m\":true,\"M\":true}]";

            double movingAverage = 1260;
            Program.TradeActions action = Program.DetermineAction(new Program.Symbol("ETHUSD"), response, movingAverage);

            Assert.IsTrue(action.Equals(Program.TradeActions.DONOTHING));
        }

        [TestMethod]
        public void GetOrdersForSymbol_SetBreakpointAndCheckOrdersForBUY()
        {
            Program.Symbol symbol = new Program.Symbol("ETHUSD");
            List<Program.Order> orders = Program.GetOrdersForSymbol(symbol, Program.TradeActions.BUY);
            Assert.IsTrue(true);
        }

        [TestMethod]
        public void GetOrdersForSymbol_SetBreakpointAndCheckOrdersForSELL()
        {
            Program.Symbol symbol = new Program.Symbol("ETHUSD");
            List<Program.Order> orders = Program.GetOrdersForSymbol(symbol, Program.TradeActions.SELL);
            Assert.IsTrue(true);
        }

        [TestMethod]
        public void DetermineOrder_NormalCase()
        {
            List<Program.Order> orders = new List<Program.Order>();
            orders.Add(new Program.Order(1965.56, 0.13));
            orders.Add(new Program.Order(1946.56, 1.13));
            orders.Add(new Program.Order(1937.56, 0.24));
            orders.Add(new Program.Order(1958.56, 0.55));
            orders.Add(new Program.Order(1949.56, 10));
            orders.Add(new Program.Order(1950.56, 0.13));
            orders.Add(new Program.Order(1945.56, 0.43));

            Program.Order order = Program.DetermineOrder(10000, orders);
            //0.24 at 1937.56
            double funds = 465.0144;
            double quantity = .24;
            //0.43 at 1945.56
            funds += 836.5908;
            quantity += .43;
            //1.13 at 1946.56
            funds += 2199.6128;
            quantity += 1.13;
            //10 at 1949.56
            funds += 6498.782;
            quantity += 3.33306;
            Assert.IsTrue(order.price == 1949.56);
            Assert.IsTrue(order.quantity == quantity);
        }

        [TestMethod]
        public void DetermineOrder_NotEnoughFunds()
        {
            List<Program.Order> orders = new List<Program.Order>();
            orders.Add(new Program.Order(1965.56, 0.13));
            orders.Add(new Program.Order(1946.56, 1.13));
            orders.Add(new Program.Order(1937.56, 0.24));
            orders.Add(new Program.Order(1958.56, 0.55));
            orders.Add(new Program.Order(1949.56, 10));
            orders.Add(new Program.Order(1950.56, 0.13));
            orders.Add(new Program.Order(1945.56, 0.43));

            Program.Order order = Program.DetermineOrder(300, orders);
            //0.24 at 1937.56
            double funds = 465.0144;
            double quantity = 0.15483;

            Assert.IsTrue(order.price == 1937.56);
            Assert.IsTrue(order.quantity == quantity);
        }
    }
}
