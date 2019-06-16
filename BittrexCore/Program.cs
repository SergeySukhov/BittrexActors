using System;
using Newtonsoft.Json;
using Bittrex.Api.Client;
using System.Linq;
using BittrexModels.ActorModels;
using System.Timers;
using BittrexModels.Models;
using BittrexCore.Models;
using DataManager.Providers;
using DataManager.Models;

namespace BittrexCore
{
    class Program
    {
        static Actor tempActor;
        static BittrexDbProvider bdProvider;
        static void Main(string[] args)
        {
            //h(); TODO: need start manager and actor factory
            BittrexClient bittrexClient = new BittrexClient("6c58ca3f387b4581ab2ba324b7a78dd5", "");

            BittrexApiManager bittrexApiManager = new BittrexApiManager(bittrexClient);
            bdProvider = new BittrexDbProvider();

            TransactionManager transactionManager = new TransactionManager(bittrexApiManager, bdProvider);

            RulesLibrary rulesLibrary = new RulesLibrary();

            ActorManager actorManager = new ActorManager(bittrexApiManager, bdProvider, transactionManager);



            var actor = actorManager.CreateActor("BTC-LTC", new TimeSpan(0, 0, 7));
            actorManager.SetupActor(actor, RulesLibrary.AllRules[RulesLibrary.RuleNames.FirstRuleBuy30min]);

            tempActor = actor;

            Timer timer = new Timer();
            timer.Interval = Consts.MainTimerInterval;

            timer.Elapsed += transactionManager.ProcessTransaction;
            timer.Elapsed += bittrexApiManager.ProcessRequestTimerAction;
            timer.Elapsed += actorManager.ActorsTimerAction;

            timer.Elapsed += Timer_Elapsed;
            
            timer.Start();
            Console.ReadLine();
        }

        static int iii = 0;
        private static void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Console.WriteLine($"Actor: {tempActor.Guid.ToString().Substring(0, 3)}" +
                $" CountVol: {tempActor.CountVolume.BtcCount} | {tempActor.CountVolume.CurrencyCount} | Obs count: {tempActor.Observations.Count}");
            Console.WriteLine("================");
            iii++;
            // if (iii == 5) bdProvider.SaveActor(tempActor);
        }

        #region Examples
        // <summary>
        /// Execute a GET request for a public api end point
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        //private async Task<string> GetPublicAsync(string url)
        //{
        //    var request = new HttpRequestMessage(HttpMethod.Get, new Uri(url));

        //    var response = await httpClient.SendAsync(request).ConfigureAwait(false);
        //    response.EnsureSuccessStatusCode();

        //    return await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        //}

        //public static async void h()
        //{
        //    BittrexClient bittrexClient = new BittrexClient("6c58ca3f387b4581ab2ba324b7a78dd5", "");
        //    var a = await bittrexClient.GetOrderBook("BTC-LTC", BookOrderType.Buy, 10);
        //    a.Result.ToList().ForEach(x => Console.WriteLine("Size: " + String.Format("{0:f8}", x.Quantity) + "  Rate: " + String.Format("{0:f8}", x.Rate)));

        //    var b = await bittrexClient.GetMarketSummary("BTC-LTC");
        //    Console.WriteLine("Bid: " + b.Result.Bid);
        //    Console.WriteLine("Ask: " + b.Result.Ask);
        //    Console.WriteLine("BaseVol: " + b.Result.BaseVolume);
        //    Console.WriteLine("Vol: " + b.Result.Volume);
        //}
        #endregion
    }
}
