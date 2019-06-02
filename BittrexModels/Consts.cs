using System;

namespace BittrexModels
{
    public static class Consts
    {
        public static string LogFilePath = Environment.CurrentDirectory + "\\BittrexActorsLogs.txt";
        public static double StartActorBuyHesitation = 1.0;
        public static double StartActorSellHesitation = 1.0;
        public static double OperationCommisionPercent = 0.0125;
        public static decimal MinimalOperationSum = 0.0005m;


    }
}
