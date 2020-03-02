using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BittrexCore
{
    public static class Const
    {
        public static readonly string ApiKey = "";
        public static readonly decimal TransactionSumBtc = 0.0001m;
        public static readonly decimal TransactionSumBtcCommision = 0.000001m;
		public static readonly double RuleChangeCoef = 0.1;
		public static readonly DateTime StartActorTime = new DateTime(2018, 6, 1, 1, 1, 1);
		public static readonly TimeSpan NewGenerationSpawnDelay = new TimeSpan(365, 0, 0, 0);
	}
}
