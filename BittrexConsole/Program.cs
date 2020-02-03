using System;

namespace BittrexConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("!! Hello, start initialization...");

            RulesLibrary.RulesLibrary r = new RulesLibrary.RulesLibrary();

			var dataManager = new BittrexData.DataManager();

			dataManager.actorProvider.TestAddingData();
			dataManager.actorProvider.TestLoadingData();




			Console.WriteLine("Finished!!");
			Console.ReadKey();
        }
    }
}
