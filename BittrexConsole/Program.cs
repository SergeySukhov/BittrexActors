using System;
using RulesLibrary;
namespace BittrexConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("!! Hello, start initialization...");

            RuleLibrary r = new RuleLibrary();

			var dataManager = new BittrexData.DataManager();

            //dataManager.actorProvider.TestAddingData();
            //dataManager.actorProvider.TestLoadingData();

            //dataManager.ActorProvider.TestSaving();


			Console.WriteLine("Finished!!");
			Console.ReadKey();
        }
    }
}
