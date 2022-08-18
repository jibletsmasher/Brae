using System;
using System.IO;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text;
using System.Security.Cryptography;
using System.Globalization;
using System.Linq;
using System.Diagnostics;

namespace Brae
{
    public class Program
    {
        //deprecated static int MovingAverageDays = 15;
        static double EMADaysLong = 26d;
        static double EMADaysShort = 12d;
        static int ObservedDays = (int) EMADaysLong * 2;
        static string period = "1w";
        static double[] PricesByPeriod = new double[ObservedDays];
        static string BaseUrl = "https://api.binance.us/api/v3/";
        static string CurrentLogFilePath = "";
        static DateTime CurrentLogFileDate;
        static string TimePath = "time";

        public enum TradeActions
        {
            BUY,
            SELL,
            DONOTHING
        }

        // use this website for reference on binance api https://github.com/binance-us/binance-official-api-docs/blob/master/rest-api.md#http-return-codes
        static void Main(string[] args)
        {
            //TODO: add XLM and NANO when we can withdraw
            List<Symbol> symbols = new List<Symbol>
            { new Symbol("ADAUSD"), new Symbol("BTCUSD", 0.5), new Symbol("ETHUSD"), new Symbol("ALGOUSD", 0.5), new Symbol("LTCUSD", 0.25), new Symbol("XTZUSD", 0.25),
              new Symbol("ZRXUSD", 0.25), new Symbol("BATUSD", 0.25), new Symbol("DOGEUSD", 0.25), new Symbol("UNIUSD", 0.25), new Symbol("MANAUSD", 0.25),
              new Symbol("SOLUSD", 0.25), new Symbol("LINKUSD", 0.25), new Symbol("VETUSD", 0.25), new Symbol("COMPUSD", 0.25), new Symbol("IOTAUSD", 0.25), new Symbol("ONEUSD", 0.25) };
            // List of removed currencies.  Removed BNB to hold for trading fee reduction
            //new Symbol("NANOUSD"), new Symbol("XLMUSD"), new Symbol("BNBUSD", 0.25), 
            //new Symbol("SUSHIUSD", 0.25) SUSHI was throwing exceptions and causing the rest of the trading pairs to not trade

            // 60000 is the amount of milliseconds in one minute
            int waitTime = 30000;
            
            while (true)
            {
                try
                {
                    UpdateLogFile();
                    foreach (Symbol symbol in symbols)
                    {
                        string tradesPath = "aggTrades?symbol=" + symbol + "&limit=1000"; // tradesPath is later modified to include startTime and endTime
                        string klinesPath = "klines?symbol=" + symbol + "&interval=" + period + "&limit=" + ObservedDays.ToString(); // get twice the amount of data to calc SMA as seed for EMA
                        string responseServerTime, responseAggTrades, responseKlines;
                        long serverTime;

                        // Retrieve kline information for calculations
                        // We could place a condition on the daily EMA calculations to only calculate once a day, but imo that brings unnecessary complications
                        responseKlines = Request(klinesPath).Result;
                        symbol.OpeningPrice = PopulatePricesByPeriod(responseKlines);

                        double smaLong = Calculations.GetSMA(PricesByPeriod, EMADaysLong);
                        double[] emaPeriods = PricesByPeriod.Where((value, index) => index >= EMADaysLong).ToArray();
                        double emaLongToday = Calculations.GetEMA(EMADaysLong, emaPeriods, smaLong);

                        // Get last EMADaysShort * 2 entries to calculate SMA
                        emaPeriods = PricesByPeriod.Where((value, index) => index >= PricesByPeriod.Length - EMADaysShort * 2).ToArray();
                        double smaShort = Calculations.GetSMA(emaPeriods, EMADaysShort);
                        emaPeriods = PricesByPeriod.Where((value, index) => index >= PricesByPeriod.Length - EMADaysShort).ToArray();
                        double emaShortToday = Calculations.GetEMA(EMADaysShort, emaPeriods, smaShort);

                        double actionPrice = Calculations.GetActionPrice(emaShortToday, emaLongToday, EMADaysShort, EMADaysLong);
                        RecordInfo("Action Price for " + symbol + ": " + actionPrice);

                        // Get the server's current time
                        responseServerTime = Request(TimePath).Result;

                        // Convert server's current time to a long to determine start and end time for trades retrieval
                        string serverTimeString = GetItemFromInfo(responseServerTime, "serverTime", '}', false);
                        serverTime = Convert.ToInt64(serverTimeString);
                        long startTime = symbol.LastTradeTime != 0 ? symbol.LastTradeTime : serverTime - waitTime;
                        // If start time is more than an hour (3600000 ms) then set it back
                        if (serverTime - startTime > 3500000) startTime = serverTime - 3500000;
                        tradesPath += "&startTime=" + startTime.ToString() + "&endTime=" + serverTime.ToString();

                        // Retrieve the aggregate trades
                        responseAggTrades = Request(tradesPath).Result;

                        // Parse the trades to determine if the price has moved above or below
                        TradeActions action = DetermineAction(symbol, responseAggTrades, actionPrice);

                        if (action.Equals(TradeActions.BUY))
                        {
                            SendBuyRequest(symbol);
                        }
                        else if (action.Equals(TradeActions.SELL))
                        {
                            SendSellRequest(symbol);
                        }
                        // else do nothing

                        // FOR TESTING
                        //SendBuyRequest(symbol);
                        //SendSellRequest(symbol);
                    }
                    System.Threading.Thread.Sleep(waitTime);
                }
                catch (Exception e)
                {
                    RecordInfo("Exception thrown at " + DateTime.Now.ToString() + ": " + e.Message + "\nStack Trace: " + e.StackTrace);
                    try
                    {
                        System.Threading.Thread.Sleep(waitTime);
                    }
                    catch (Exception ex)
                    {
                        RecordInfo("We can't wait for some reason... Exception thrown at " + DateTime.Now.ToString() + ": " + ex.Message + "\nStack Trace: " + ex.StackTrace);
                    }
                }
            }
            
            
        }

        // returns latest opening price for that particular symbol
        public static double PopulatePricesByPeriod(string response)
        {
            // See this for how response is formatted: https://github.com/binance-us/binance-official-api-docs/blob/master/rest-api.md#klinecandlestick-data
            // reponse is sorted in chronological order
            int indexOfQuote, indexOfSecondQuote, indexOfBrace;
            response = response.Substring(1); // Remove the first brace

            double openingPrice = 0d;
            for (int i = 0; i < ObservedDays; i++)
            {
                indexOfBrace = response.IndexOf('[');
                indexOfQuote = response.IndexOf('"', indexOfBrace);
                indexOfSecondQuote = response.IndexOf('"', indexOfQuote+1);
                openingPrice = Convert.ToDouble(response.Substring(indexOfQuote + 1, indexOfSecondQuote - indexOfQuote - 1));
                PricesByPeriod[i] = openingPrice;
                response = response.Substring(response.IndexOf(']') + 1);
            }
            return openingPrice;
        }

        public static TradeActions DetermineAction(Symbol symbol, string responseAggTrades, double actionPrice)
        {
            // INFO: this method goes through all of the aggregated trades made since the last time we checked all of the trades.
            //          after parsing through all of the trades,
            //              if the sum price movement rises from below the action price -> buy
            //              if the sum price movement falls from above the action price -> sell
            //              if the sum price movement neither rises nor falls -> do nothing
            // See this for how resonse is formatted: https://github.com/binance-us/binance-official-api-docs/blob/master/rest-api.md#compressedaggregate-trades-list
            double price;
            string item;
            bool isFirstTrade = false;
            // If the last trade time doesn't exist then set it to the first trade time in the response and set price flag
            if (symbol.LastTradeTime == 0 && !responseAggTrades.Equals("[]"))
            {
                item = GetItemFromInfo(responseAggTrades, "T", ',', false);
                symbol.LastTradeTime = Convert.ToInt64(item);

                item = GetItemFromInfo(responseAggTrades, "p", '"', true);
                price = Convert.ToDouble(item);
                symbol.IsPriceLowerThanMA = price < actionPrice;
                isFirstTrade = true;
            }

            // Go through each aggregate trade
            int indexOfOpeningBrace = responseAggTrades.IndexOf('{');
            int indexOfClosingBrace;
            string tradeInfo;
            bool priceChangeHolder;
            bool actionHasChanged = false;
            TradeActions action = TradeActions.DONOTHING;
            while (indexOfOpeningBrace != -1)
            {
                // Get info for single aggregate trade
                indexOfClosingBrace = responseAggTrades.IndexOf('}');
                tradeInfo = responseAggTrades.Substring(indexOfOpeningBrace + 1, indexOfClosingBrace - indexOfOpeningBrace - 1);

                // Update last trade time
                item = GetItemFromInfo(tradeInfo, "T", ',', false);
                symbol.LastTradeTime = Convert.ToInt64(item);

                // Get price and compare it to action price
                item = GetItemFromInfo(tradeInfo, "p", '"', true);
                price = Convert.ToDouble(item);

                priceChangeHolder = symbol.IsPriceLowerThanMA;
                symbol.IsPriceLowerThanMA = price < actionPrice;
                if (priceChangeHolder != symbol.IsPriceLowerThanMA || isFirstTrade)
                {
                    isFirstTrade = false;
                    if (actionHasChanged)
                    {
                        action = TradeActions.DONOTHING;
                        actionHasChanged = false;
                    }
                    else
                    {
                        //action = symbol.IsPriceLowerThanMA ? TradeActions.SELL : TradeActions.BUY;
                        //actionHasChanged = true;
                        //If we want to limit trading even further then extend the buy action price further from the calculated price
                        if (symbol.IsPriceLowerThanMA)
                        {
                            // We want to sell right away when the price is lower than the MA
                            action = TradeActions.SELL;
                            actionHasChanged = true;
                        }
                        else if (actionPrice < price && symbol.OpeningPrice < price)
                        {
                            // Limit the amount of buys and sells by being more selective with our buy
                            action = TradeActions.BUY;
                            actionHasChanged = true;
                        }
                        else if (actionPrice > price)
                        {
                            symbol.IsPriceLowerThanMA = true;
                        }
                    }
                }

                responseAggTrades = responseAggTrades.Substring(indexOfClosingBrace + 1);
                indexOfOpeningBrace = responseAggTrades.IndexOf('{');
            }

            return action;
        }

        #region Buying and Selling
        public static void SendBuyRequest(Symbol symbol)
        {
            long milliseconds = GetMilliseconds();

            double funds = DetermineFundsForBuy(symbol, milliseconds.ToString());
            if (funds == 0) return;

            List<Order> orders = GetOrdersForSymbol(symbol, TradeActions.BUY);
            Order order = DetermineOrder(funds, orders);
            order.quantity = GetAppropriateQuantity(symbol, order.quantity);
            if (order.quantity == 0) return;

            string query = "&symbol=" + symbol.ToString() +
                "&side=BUY" +
                "&type=MARKET" +
                "&quantity=" + order.quantity +
                //"&price=" + order.price + we don't need the price to be in there for a market order, but I want to keep here for posterity
                "&recvWindow=5000" +
                "&timestamp=" + milliseconds;

            string response = AccountRequest("order?", query).Result;
            RecordOrder(response);
        }

        public static List<Order> GetOrdersForSymbol(Symbol symbol, TradeActions action)
        {
            // See this for how response is formatted: https://github.com/binance-us/binance-official-api-docs/blob/master/rest-api.md#order-book
            string depthInfo = Request("depth?symbol=" + symbol).Result;
            string terminator = "";
            string side = "";
            if (action.Equals(TradeActions.BUY))
            {
                terminator = "]}";
                side = "asks";
            }
            else if (action.Equals(TradeActions.SELL))
            {
                RecordInfo("ERROR: GetOrdersForSymbol was called with an action of SELL");
                //terminator = "],";
                //side = "bids";
            }
            else
            {
                RecordInfo("ERROR: GetOrdersForSymbol was called with an action of DONOTHING");
            }

            string pairings = GetItemFromInfo(depthInfo, side, terminator, false);
            pairings = pairings.Substring(1); // remove the first [
            int bracketIndex = 0;
            List<Order> orders = new List<Order>();
            string price, quantity;
            int quoteIndex1, quoteIndex2;
            while (bracketIndex != -1)
            {
                quoteIndex1 = pairings.IndexOf('"', bracketIndex);
                quoteIndex2 = pairings.IndexOf('"', quoteIndex1 + 1);
                // When selling, we don't care about the price, so the price will be 1, as in we will buy the "whole" quantity
                price = action.Equals(TradeActions.BUY) ? pairings.Substring(quoteIndex1 + 1, quoteIndex2 - quoteIndex1 - 1) : "1";

                quoteIndex1 = pairings.IndexOf('"', quoteIndex2 + 1);
                quoteIndex2 = pairings.IndexOf('"', quoteIndex1 + 1);
                quantity = pairings.Substring(quoteIndex1 + 1, quoteIndex2 - quoteIndex1 - 1);
                orders.Add(new Order(price, quantity));
                bracketIndex = pairings.IndexOf('[', quoteIndex2);
            }
            return orders;
        }

        public static Order DetermineOrder(double funds, List<Order> orders)
        {
            //INFO: funds - amount of funds available to use for the order retrieved from api request
            //      orders - list of limit orders retrieved from api request

            // ascending order -> this way our order will pick up the bottom sell orders
            Comparison<Order> comparison = (x, y) => x.price.CompareTo(y.price);
            orders.Sort(comparison);

            double lastPrice = 0;
            double quantity = 0;
            double fundsToUse;
            foreach (Order order in orders)
            {
                lastPrice = order.price;
                fundsToUse = order.price * order.quantity;
                if (funds - fundsToUse < 0)
                {
                    quantity += Math.Floor(funds) / order.price; // Round the dollar value down so we don't attempt to place an order than might be slightly too large
                    break;
                }
                funds -= fundsToUse;
                quantity += order.quantity;
            }

            return new Order(lastPrice, quantity);
        }

        public static double DetermineFundsForBuy(Symbol symbol, string timeStamp)
        {
            double USDAvailable = GetFundsCount("asset\":\"USD\",\"free\"", timeStamp);
            if (USDAvailable > 100)
            {
                if (USDAvailable < 5000)
                {
                    USDAvailable = USDAvailable - 5d;
                }
                else if (USDAvailable > 8000)
                {
                    USDAvailable = 4000d;
                }
                else
                {
                    USDAvailable = USDAvailable / 2d;
                }
            }
            else
            {
                USDAvailable = 0;
            }
            return USDAvailable;
        }

        public static double GetFundsCount(string itemToObtain, string timeStamp)
        {
            //TODO: test this method
            //      to test this method we'll have to run the program and set a breakpoint
            // See this for how response is formatted: https://github.com/binance-us/binance-official-api-docs/blob/master/rest-api.md#account-information-user_data
            string response = AccountRequest("account?", "timestamp=" + timeStamp).Result;
            string item = GetItemFromInfo(response, itemToObtain, '"', true);
            RecordInfo("item to obtain: " + itemToObtain + "\nitem obtained: " + item);
            double funds = Convert.ToDouble(item);
            //funds = funds * 0.995; // Reduce funds by 0.5% to prevent LOT_SIZE error
            return funds;
}

        public static void SendSellRequest(Symbol symbol)
        {
            long milliseconds = GetMilliseconds();

            double funds = GetFundsCount("asset\":\"" + symbol.ToString().Replace("USD", "") + "\",\"free\"", milliseconds.ToString());
            funds = GetAppropriateQuantity(symbol, funds);
            if (funds == 0) return;

            string query = "&symbol=" + symbol +
                "&side=SELL" +
                "&type=MARKET" +
                "&quantity=" + funds +
                "&recvWindow=5000" +
                "&timestamp=" + milliseconds;

            string response = AccountRequest("order?", query).Result;
            RecordOrder(response);
        }

        public static double GetAppropriateQuantity(Symbol symbol, double quantity)
        {
            //See https://github.com/binance-us/binance-official-api-docs/blob/master/rest-api.md#exchange-information
            // or this https://api.binance.com/api/v3/exchangeInfo?symbol=VETUSDT
            string response = "{\"code\":-1121,\"msg\":\"Invalid symbol.\"}";

            string path = "exchangeInfo?symbol=";
            response = Request(path + symbol).Result;
            if (response.IndexOf("{\"code\":-1121,\"msg\":\"Invalid symbol.\"}") != -1)
            {
                // No idea why, but some symbols don't retrieve information while on the VM, but do when typing the request into the local web client
                // try to grab all info and extract specific symbol info
                string symbolUSDT = symbol + "T"; // Add "T" because the exchangeInfo doesn't recognize USD as a symbol sometimes
                response = Request(path + symbolUSDT).Result;
                int index = response.IndexOf(symbolUSDT);
                if (index == -1)
                {
                    RecordInfo("WHY DOES " + symbolUSDT + " NOT EXIST");
                }
            }

            double minQty = Convert.ToDouble(GetItemFromInfo(response, "filterType\":\"LOT_SIZE\",\"minQty\"", '"', true));
            response = response.Substring(response.IndexOf("filterType\":\"LOT_SIZE\",\"minQty\""));
            double maxQty = Convert.ToDouble(GetItemFromInfo(response, "\"maxQty\"", '"', true));
            if (quantity < minQty || quantity > maxQty) return 0;

            double stepSize = Convert.ToDouble(GetItemFromInfo(response, "\"stepSize\"", '"', true));
            // Validate LOT_SIZE Requirements: https://github.com/binance-us/binance-official-api-docs/blob/master/rest-api.md#lot_size
            for (int i=1000000; i > 0; i /= 10)
            {
                quantity = Math.Truncate(quantity * i) / i;
                int quan = (int)Math.Truncate(quantity * i);
                int step = (int)Math.Truncate(stepSize * i);
                int min = (int)Math.Truncate(minQty * i);
                if ((quan - min) % step == 0)
                {
                    break;
                }
            }

            // **The MIN_NOTIONAL numbers being returned from requests seem inconsistent with their respective symbols:
            //      ex) ETHUSDT has a minNotional of 10 and an avgPriceMins of 5, meaning quantity has to be at least 2... the same numbers can be found for all symbols
            //// Validate MIN_NOTIONAL Requirements: https://github.com/binance-us/binance-official-api-docs/blob/master/rest-api.md#min_notional
            //double minNotional = Convert.ToDouble(GetItemFromInfo(response, "filterType\":\"MIN_NOTIONAL\",\"minNotional\"", '"', true));
            //string avgPriceMinsHolder = GetItemFromInfo(response, "applyToMarket\":true,\"avgPriceMins\"", '}', false);
            //if (!String.IsNullOrEmpty(avgPriceMinsHolder))
            //{
            //    double avgPriceMins = Convert.ToDouble(avgPriceMinsHolder);
            //    if (avgPriceMins * quantity < minNotional) return 0;
            //}
            return quantity;
        }

        public static void RecordOrder(string orderInfo)
        {
            // See "Response FULL" for how orderInfo is formatted: https://github.com/binance-us/binance-official-api-docs/blob/master/rest-api.md#test-new-order-trade

            bool isOrderSuccessful = !string.IsNullOrEmpty(GetItemFromInfo(orderInfo, "symbol", '"', true));
            StreamWriter logWriter = new StreamWriter(CurrentLogFilePath, true);
            if (isOrderSuccessful)
            {
                logWriter.WriteLine("SUCCESSFUL ORDER AT " + DateTime.Now.ToString() + " DETAILS BELOW");
                orderInfo.Replace(',', '\n');
                logWriter.WriteLine(orderInfo);
                logWriter.WriteLine("----------------------------------------");
                logWriter.Close();
            }
            else
            {
                logWriter.WriteLine("ERROR IN ORDER AT " + DateTime.Now.ToString() + " DETAILS BELOW");
                logWriter.WriteLine(orderInfo);
                logWriter.WriteLine("----------------------------------------");
                logWriter.Close();
                //throw new Exception();
            }
        }
        #endregion

        #region General Methods and Classes
        public static string GetItemFromInfo(string info, string item, char terminator, bool isWrappedInQuotes)
        {
            return GetItemFromInfo(info, item, terminator.ToString(), isWrappedInQuotes);
        }

        public static string GetItemFromInfo(string info, string item, string terminator, bool isWrappedInQuotes)
        {
            // info - response string
            // item - string of item identifier in response without quotes
            // terminator - character after the end of the desired information
            // isWrappedInQuotes - in the documentation of the response, is the desired information wrapped in quotes?
            int startingLocOffset = isWrappedInQuotes ? 2 : 1;
            int indexOfItem = info.IndexOf(item);
            if (indexOfItem == -1) return "";
            int indexEndOfItem = indexOfItem + item.Length;
            int indexOfColon = info.IndexOf(':', indexEndOfItem);
            int indexOfTerminator = info.IndexOf(terminator, indexOfColon + startingLocOffset);
            string value = info.Substring(indexOfColon + startingLocOffset, indexOfTerminator - indexOfColon - startingLocOffset);

            return value;
        }

        public static long GetMilliseconds()
        {
            // For some reason, there is a difference of 21582225 milliseconds between local computer and the server
            return (long)(Math.Round((DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0)).TotalMilliseconds));// + 21580225);
        }

        public static void UpdateLogFile()
        {
            string processFolder = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);
            var culture = CultureInfo.CreateSpecificCulture("en-US");
            DirectoryInfo directory = Directory.CreateDirectory(processFolder + "\\LogFiles");
            string pathPlacehodler = directory.FullName + "\\" + DateTime.Now.Date.ToString("d", culture).Replace('/', '-') + ".txt";
            if (!DateTime.Now.Date.Equals(CurrentLogFileDate.Date) || string.IsNullOrEmpty(CurrentLogFilePath) && !File.Exists(pathPlacehodler))
            {
                // Create a new file for a new day
                CurrentLogFilePath = pathPlacehodler;
                FileStream stream = File.Create(CurrentLogFilePath);
                stream.Close();
                CurrentLogFileDate = DateTime.Now;
            }
        }

        public static void RecordInfo(string info)
        {
            StreamWriter logWriter = new StreamWriter(CurrentLogFilePath, true);
            logWriter.WriteLine(info);
            logWriter.WriteLine("----------------------------------------");
            logWriter.Close();
        }

        public static async Task<string> Request(string path)
        {
            HttpClient client = new HttpClient();
            var response = client.GetAsync(BaseUrl + path).Result;
            //var response = await client.GetAsync("https://jsonplaceholder.typicode.com/todos/1");  // USED TO TEST

            string responseString = response.Content.ReadAsStringAsync().Result;

            StreamWriter logWriter = new StreamWriter(CurrentLogFilePath, true);
            logWriter.WriteLine(path + " at " + DateTime.Now.ToString());
            logWriter.WriteLine(responseString);
            logWriter.WriteLine("-------------------------");
            logWriter.Close();
            client.Dispose();

            return responseString;
        }

        public static async Task<string> AccountRequest(string path, string query)
        {
            try
            {
                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Add("X-MBX-APIKEY", "oGwGuYj9js7qqTbmZJ9zzInNwPguODPHU0nZapxXDnJ18oyP9Hq7gJCZD11IhmdV");
                query += "&signature=" + GetSignature(query);

                HttpResponseMessage response;
                if (IsPostRequest(path))
                {
                    response = client.PostAsync(BaseUrl + path + query, null).Result;
                }
                else
                {
                    response = client.GetAsync(BaseUrl + path + query).Result;
                }

                string responseString = response.Content.ReadAsStringAsync().Result;

                StreamWriter logWriter = new StreamWriter(CurrentLogFilePath, true);
                logWriter.WriteLine(path);
                logWriter.WriteLine("query: " + query);
                logWriter.WriteLine(responseString);
                logWriter.WriteLine("-------------------------");
                logWriter.Close();
                client.Dispose();

                return responseString;
            }
            catch (Exception e)
            {
                RecordInfo("Exception thrown in AccountRequest: " + e.Message + "\nStack Trace: " + e.StackTrace);
                throw e;
            }
        }










        public static string GetSignature(string query)
        {
            byte[] keyBytes = Encoding.UTF8.GetBytes("<redacted for account safety reasons>");
            byte[] queryBytes = Encoding.UTF8.GetBytes(query);
            HMACSHA256 hmacsha256 = new HMACSHA256(keyBytes);

            byte[] bytes = hmacsha256.ComputeHash(queryBytes);

            return BitConverter.ToString(bytes).Replace("-", "").ToLower();
        }

        public static bool IsPostRequest(string requestPath)
        {
            switch (requestPath)
            {
                case "order/test?": return true;
                case "order?": return true;
                default: return false;
            }
        }

        public class Symbol
        {
            public string symbol;
            public double weight;
            public long LastTradeTime;
            public bool IsPriceLowerThanMA;
            public double OpeningPrice;

            public Symbol(string symbol, double weight = 1)
            {
                this.symbol = symbol;
                this.weight = weight;
                LastTradeTime = 0;
                IsPriceLowerThanMA = false;
            }

            public override string ToString()
            {
                return symbol;
            }
        }

        public class Order
        {
            public double price;
            public double quantity;

            public Order(string price, string quantity)
            {
                this.price = Convert.ToDouble(price);
                this.quantity = Convert.ToDouble(quantity);
            }

            public Order(double price, double quantity)
            {
                this.price = price;
                this.quantity = quantity;
            }
        }

        #endregion
    }
}
