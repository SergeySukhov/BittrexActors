using System;
using Newtonsoft.Json;
using Bittrex.Api.Client.Models;
using Bittrex.Api.Client;
using System.Linq;
using BittrexModels.ActorModels;
using System.Timers;
using System.Threading;
using System.Threading.Tasks;

namespace BittrexCore
{
    class Program
    {
        private static System.Timers.Timer MainTimer = new System.Timers.Timer(1000);

        static void Main(string[] args)
        {
            Task.WaitAll(InitComponents());
            Task.WaitAll(LoadActors());
            while(true)
            {
                var command = Console.ReadLine().ToLower();
                if (command.Contains("ex")) break;

                if (command.Contains("show -a")) {
                    if (command.Contains("-o"))
                        Actor.AllActors.ForEach(x => Console.WriteLine($"> Actor: {x.Guid.ToString().Substring(0, 4)} | observation count: {x.Observations.Count}"));

                    if (command.Contains("-v"))
                        Actor.AllActors.ForEach(x => Console.WriteLine($"> Actor: {x.Guid.ToString().Substring(0, 4)} | btc volume: {x.CountVolume.BtcCount}"));

                    if (command.Contains("-c"))
                    {
                        Task.WaitAll(CreateActors());
                    }
                }
                Console.WriteLine("--------------------");
            }
            
        }

        private static async Task InitComponents()
        {
            Thread.Sleep(1000);
            MainTimer.Elapsed += Timer_Elapsed;
            MainTimer.Start();
            Console.WriteLine("Components load");
        }

        private static async Task LoadActors()
        {
            await Task.Factory.StartNew(() =>
            {
                for (int i = 0; i < 1; i++)
                {
                    Console.WriteLine("i:" + (i + 1));
                    var actor = new Actor("BTC-LTC", 5);
                    var rule = new Rule(RuleType.ForBuy);
                    rule.Conditions.Add(x => { return 0; });

                    actor.Rules.Add(rule);


                    Thread.Sleep(1000);
                }

                foreach (var actor in Actor.AllActors)
                {
                    MainTimer.Elapsed += actor.ActorTimer;
                }
            });

            Console.WriteLine("Actors load");
        }

        //private Task RunActors()
        //{

        //}

        private static async Task CreateActors()
        {
            var newActor = new Actor("LTC");
            MainTimer.Elapsed += newActor.ActorTimer;

            Console.WriteLine($"Actor {newActor.Guid.ToString()} created");
        }

        private static void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
           // Actor.AllActors.ForEach(x => Console.WriteLine(">" + x.Observations.Count));
           // Console.WriteLine("--------------------");           
        }
        
    }
}
