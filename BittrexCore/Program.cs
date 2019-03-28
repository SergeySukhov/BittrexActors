﻿using System;
using Newtonsoft.Json;
using Bittrex.Api.Client.Models;
using Bittrex.Api.Client;
using System.Linq;
using BittrexModels.ActorModels;
using System.Timers;

namespace BittrexCore
{
    class Program
    {
        static void Main(string[] args)
        {
            h();

            var actor = new Actor("BTC-LTC", new TimeSpan(0, 0, 1));
            
            var rule = new Rule(new TimeSpan(0, 59, 0));
            rule.Conditions.Add(x => { return 0; });

            actor.Rules.Add(rule);

            actor.MakePrediction();

            Timer timer = new Timer();
            timer.Interval = actor.TickSpan.TotalMilliseconds;
            timer.Elapsed += actor.ActorTimer;
            timer.Elapsed += Timer_Elapsed;
            timer.Start();
            Console.ReadLine();
        }

        private static void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            
            Console.WriteLine("1");
            //throw new NotImplementedException();
        }


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

        public static async void h()
        {
            BittrexClient bittrexClient = new BittrexClient("6c58ca3f387b4581ab2ba324b7a78dd5", "");
            var a = await bittrexClient.GetOrderBook("BTC-LTC", BookOrderType.Buy, 10);
            a.Result.ToList().ForEach(x => Console.WriteLine("Size: " + String.Format("{0:f8}", x.Quantity) + "  Rate: " + String.Format("{0:f8}", x.Rate)));

            var b = await bittrexClient.GetMarketSummary("BTC-LTC");
            Console.WriteLine("Bid: " + b.Result.Bid);
            Console.WriteLine("Ask: " + b.Result.Ask);
            Console.WriteLine("BaseVol: " + b.Result.BaseVolume);
            Console.WriteLine("Vol: " + b.Result.Volume);
        }
    }
}
